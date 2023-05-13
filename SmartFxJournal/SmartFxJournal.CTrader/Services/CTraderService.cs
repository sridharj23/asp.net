using OpenAPI.Net.Helpers;
using OpenAPI.Net;
using SmartFxJournal.CTrader.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Web;
using SmartFxJournal.JournalDB.model;
using OpenAPI.Net.Auth;
using System.Text.Json;

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
            return ctx.OnBoardingResult.Copy;
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

        public async Task<List<AccountEntry>> ProcessAccountsDelta (string cTraderId)
        {
            if (!this._loginContexts.ContainsKey(cTraderId))
            {
                throw new Exception("No pending authorization flows expected. Not enought information to process the authorization code.");
            }

            List<AccountEntry> accounts = new();
            LoginContext ctx = _loginContexts[cTraderId];
            CTraderAccount ctAccount = ctx.CTraderAccount;
            ApiCredentials creds = new();
            creds.ClientId = ctAccount.ClientId;
            creds.Secret = ctAccount.ClientSecret;
            Token token = TokenFactory.DeserializeToken(ctAccount.AuthToken);

            OpenClient liveClientFactory() => new(ApiInfo.LiveHost, ApiInfo.Port, TimeSpan.FromSeconds(10));
            OpenClient demoClientFactory() => new(ApiInfo.DemoHost, ApiInfo.Port, TimeSpan.FromSeconds(10));
            var apiService = new OpenApiService(liveClientFactory, demoClientFactory);
            await apiService.Connect(creds);

            var tradeService = new TradingAccountsService(apiService);
            var protoAccounts = await tradeService.GetAccounts(token.AccessToken);

            foreach(var act in protoAccounts)
            {
                AccountEntry entry = new();
                AccountModel model = await tradeService.GetAccountModelById((long)act.CtidTraderAccountId);
                entry.CTraderId = "";
                entry.AccountId = Convert.ToString(act.CtidTraderAccountId);
                entry.IsLive = act.IsLive;
                entry.BrokerName = model.Trader.BrokerName;
                entry.Balance = model.Balance;
            }

            return accounts;
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
