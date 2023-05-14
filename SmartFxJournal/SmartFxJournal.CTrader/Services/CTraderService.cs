using OpenAPI.Net.Helpers;
using OpenAPI.Net;
using SmartFxJournal.CTrader.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Web;
using SmartFxJournal.JournalDB.model;
using OpenAPI.Net.Auth;
using System.Text.Json;
using System.Security.Principal;

namespace SmartFxJournal.CTrader.Services
{
    public class CTraderService
    {
        private static readonly HttpClient sharedClient = new();

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly Dictionary<String, LoginContext> _loginContexts = new Dictionary<string, LoginContext>();

        public CTraderService(IServiceScopeFactory scopeFactory) {
            _scopeFactory = scopeFactory;
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                JournalDbContext dbContext = serviceScope.ServiceProvider.GetService<JournalDbContext>() ?? throw new ArgumentNullException(nameof(serviceScope));
                if (dbContext.CTraderAccounts != null)
                {
                    var accounts = dbContext.CTraderAccounts.ToList<CTraderAccount>();
                    foreach (var account in accounts)
                    {
                        LoginContext ctx = new LoginContext(account);
                        ctx.OnBoardStatus = OnBoardStatus.Success;
                        _loginContexts.Add(account.CTraderId, ctx);
                    }
                }
            }
        }

        public async Task<bool> HasLogonCredentials(String cTraderId)
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                JournalDbContext dbContext = serviceScope.ServiceProvider.GetService<JournalDbContext>() ?? throw new ArgumentNullException(nameof(serviceScope));
                if (dbContext.CTraderAccounts != null)
                {
                    var ctAccount = await dbContext.CTraderAccounts.FindAsync(cTraderId);
                    return ctAccount != null;
                }
            }
                return false;
        }

        public Uri PrepareNewCTraderAccountAuthorizationContext(string cTraderId, string client_id, string client_secret, string redirectUrl) {
            CTraderAccount account;
            LoginContext ctx;
            if (!this._loginContexts.ContainsKey(cTraderId))
            {
                account = new()
                {
                    CTraderId = cTraderId,
                    ClientId = client_id,
                    ClientSecret = client_secret
                };

                ctx = new LoginContext(account)
                {
                    RedirectUrl = redirectUrl
                };
                _loginContexts.Add(cTraderId, ctx);
                ctx.OnBoardStatus = OnBoardStatus.Pending;
            }
            else
            {
                ctx = _loginContexts[cTraderId];
            }

            return ctx.OAuthUri;
        }

        public async Task<OnBoardingResult> ProcessCTraderAccountAuthorization(string cTraderId, string authorizationCode)
        {
            if (!this._loginContexts.ContainsKey(cTraderId))
            {
                throw new Exception("No pending authorization flows expected. Not enought information to process the authorization code.");
            }
            LoginContext ctx = _loginContexts[cTraderId];
            CTraderAccount acc = ctx.CTraderAccount;

            App app = new(acc.ClientId, acc.ClientSecret, ctx.RedirectUrl);
            Token token = await TokenFactory.GetToken(authorizationCode, app, sharedClient);
            acc.LastFetchedOn = DateTime.Now;
            acc.AuthToken = JsonSerializer.Serialize<Token>(token);
            acc.RefreshToken = token.RefreshToken;
            acc.AccessToken = token.AccessToken;
            acc.ExpiresIn = token.ExpiresIn.ToUnixTimeMilliseconds();

            using (var serviceScope = _scopeFactory.CreateScope())
            {
                JournalDbContext dbContext = serviceScope.ServiceProvider.GetService<JournalDbContext>() ?? throw new ArgumentNullException(nameof(serviceScope));
                if (dbContext.CTraderAccounts != null)
                {
                    dbContext.CTraderAccounts.Add(acc);
                    dbContext.SaveChanges();
                }
            }

            ctx.OnBoardStatus = OnBoardStatus.Success;
            await ProcessAccountsDelta(cTraderId, token.AccessToken);
            return ctx.OnBoardingResult.Copy();
        }

        public bool FailCTraderAccountAuthFlow(string cTraderId, string error_code, string error_description)
        {
            if (!this._loginContexts.ContainsKey(cTraderId))
            {
                throw new Exception("No pending authorization flows expected. Not enought information to process the authorization code.");
            }
            LoginContext ctx = _loginContexts[cTraderId];
            ctx.OnBoardStatus = OnBoardStatus.Failed;
            ctx.OnBoardingResult.ErrorDescription = error_code + " : " + error_description;

            return true;
        }

        public OnBoardingResult OnBoardingResult(String cTraderId)
        {
            LoginContext? ctx = null;

            if (this._loginContexts.ContainsKey(cTraderId))
            {
                ctx = this._loginContexts[cTraderId];
            }

            OnBoardStatus status = OnBoardStatus.Unknown;
            if (ctx != null)
            {
                status = ctx.OnBoardStatus;
            }

            return new OnBoardingResult(cTraderId, status);
            
        }

        public async Task<string> ImportAccounts(string cTraderId)
        {
            int p = await ProcessAccountsDelta(cTraderId, null);
            return (p.ToString() + " accounts have been imported / updated.");
        }

        private async Task<int> ProcessAccountsDelta(string cTraderId, string? token)
        {
            if (!this._loginContexts.ContainsKey(cTraderId))
            {
                throw new Exception("CTrader ID : " + cTraderId + " is not yet onboarded !. Cannot import information.");
            }

            LoginContext ctx = _loginContexts[cTraderId];
            CTraderAccount ctAccount = ctx.CTraderAccount;
            if (token == null) { token = ctAccount.AccessToken; }
            ApiCredentials creds = new();
            creds.ClientId = ctAccount.ClientId;
            creds.Secret = ctAccount.ClientSecret;

            OpenClient liveClientFactory() => new(ApiInfo.LiveHost, ApiInfo.Port, TimeSpan.FromSeconds(10));
            OpenClient demoClientFactory() => new(ApiInfo.DemoHost, ApiInfo.Port, TimeSpan.FromSeconds(10));
            var apiService = new OpenApiService(liveClientFactory, demoClientFactory);
            await apiService.Connect(creds);

            var tradeService = new TradingAccountsService(apiService);
            var protoAccounts = await tradeService.GetAccounts(token);

            int processedAccounts = 0;

            using (var serviceScope = _scopeFactory.CreateScope())
            {
                JournalDbContext dbContext = serviceScope.ServiceProvider.GetService<JournalDbContext>() ?? throw new ArgumentNullException(nameof(serviceScope));
                if (dbContext.CTraderAccounts != null)
                {
                    CTraderAccount? cta = await dbContext.CTraderAccounts.FindAsync(cTraderId);
                    if (cta == null)
                    {
                        throw new Exception("CTrader ID : " + cTraderId + " is not yet onboarded !. Cannot import information.");
                    }
                    List<Account> accounts = cta.Accounts;
                    foreach (var act in protoAccounts)
                    {
                        processedAccounts++;
                        var trader = await apiService.GetTrader((long)act.CtidTraderAccountId, act.IsLive);
                        var assets = await apiService.GetAssets((long)act.CtidTraderAccountId, act.IsLive);
                        DateTimeOffset start = DateTimeOffset.FromUnixTimeMilliseconds(trader.RegistrationTimestamp);
                        DateTimeOffset end = start.Add(new TimeSpan(2, 0, 0));
                        string acno = Convert.ToString(act.TraderLogin);
                        Account? entry = accounts.Find(a => a.AccountNo.Equals(acno));
                        if (entry == null)
                        {
                            entry = new(acno);
                            entry.ImportMode = "cTrader";
                            entry.IsDefault = false;
                            entry.AccountType = act.IsLive ? "Live" : "Demo";
                            entry.CurrencyType = assets.First(iAsset => iAsset.AssetId == trader.DepositAssetId).Name;
                            entry.BrokerName = trader.BrokerName;
                            entry.OpenedOn = DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeMilliseconds(trader.RegistrationTimestamp).DateTime);
                            var transactions = await apiService.GetTransactions((long)act.CtidTraderAccountId, act.IsLive, start, end);
                            entry.StartBalance = (decimal)(transactions[0].Balance / 100);
                            entry.CreatedOn = DateTime.Now;
                            cta.Accounts.Add(entry);
                        }
                        entry.CurrentBalance = (decimal?)MonetaryConverter.FromMonetary(trader.Balance);
                        entry.LastModifiedOn = DateTime.Now;
                        
                    }
                    dbContext.SaveChanges();
                }
            }
            return processedAccounts;
        }
    }

    internal class LoginContext
    {
        static readonly string openApiUrl = "https://openapi.ctrader.com/apps/auth?client_id={0}&redirect_uri={1}&scope=accounts";
        private readonly CTraderAccount forAccount;

        public LoginContext(CTraderAccount account)
        {
            this.forAccount = account;
            OnBoardingResult = new(account.CTraderId);
        }

        public CTraderAccount CTraderAccount { get { return forAccount; } }

        public string? RedirectUrl { get; set; }

        public Uri OAuthUri { get { return new Uri(string.Format(openApiUrl, forAccount.ClientId, RedirectUrl)); } }

        public OnBoardingResult OnBoardingResult { get; private set; }
        public OnBoardStatus OnBoardStatus { 
            get { return OnBoardingResult.OnBoardingStatus;  } 
            set { OnBoardingResult.OnBoardingStatus = value; } 
        }
    }
}
