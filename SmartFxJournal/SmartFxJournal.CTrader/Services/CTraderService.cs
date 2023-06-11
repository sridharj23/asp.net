using OpenAPI.Net.Helpers;
using OpenAPI.Net;
using SmartFxJournal.CTrader.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Web;
using SmartFxJournal.JournalDB.model;
using OpenAPI.Net.Auth;
using System.Text.Json;
using static SmartFxJournal.JournalDB.model.GlobalEnums;
using SmartFxJournal.CTrader.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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

            acc.LastFetchedOn = DateTimeOffset.Now;
            acc.AuthToken = JsonSerializer.Serialize<Token>(token);
            acc.RefreshToken = token.RefreshToken;
            acc.AccessToken = token.AccessToken;
            acc.ExpiresOn = token.ExpiresIn;

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

            await ProcessAccountsDelta(cTraderId);
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
            int p = await ProcessAccountsDelta(cTraderId);
            return (p.ToString() + " accounts have been imported / updated.");
        }

        private async Task<int> ProcessAccountsDelta(string cTraderId)
        {
            if (!this._loginContexts.ContainsKey(cTraderId))
            {
                throw new Exception("CTrader ID : " + cTraderId + " is not yet onboarded !. Cannot import information.");
            }

            LoginContext ctx = _loginContexts[cTraderId];
            CTraderAccount ctAccount = ctx.CTraderAccount;
            OpenApiService apiService = await ctx.ConnectAsync();

            if (ctx.TradingAccountsService == null)
            {
                throw new Exception("CTrader ID : " + cTraderId + " is not yet connected !. Cannot import information.");
            }

            var protoAccounts = await ctx.TradingAccountsService.GetAccounts(ctAccount.AccessToken);

            int processedAccounts = 0;

            using (var serviceScope = _scopeFactory.CreateScope())
            {
                JournalDbContext dbContext = serviceScope.ServiceProvider.GetService<JournalDbContext>() ?? throw new ArgumentNullException(nameof(serviceScope));
                if (dbContext.CTraderAccounts != null)
                {
                    CTraderAccount? cta = dbContext.CTraderAccounts.Include(a => a.TradingAccounts).First(c => c.CTraderId == cTraderId) ?? 
                                                throw new Exception("CTrader ID : " + cTraderId + " is not yet onboarded !. Cannot import information.");

                    foreach (var act in protoAccounts)
                    {
                        TradingAccount account = await OpenApiImporter.ImportAccountAsync(act, ctx.OpenApiService, cta);
                        var accounts = dbContext.TradingAccounts.ToList();
                        if (accounts.FirstOrDefault(a => a.AccountNo == account.AccountNo) == null)
                        {
                            // new account
                            account.Positions.Add(OpenApiImporter.CreateReconcilePosition(account));
                        }
                        dbContext.SaveChanges();

                        DateTimeOffset to = DateTimeOffset.Now;
                        TradingAccount parent = dbContext.TradingAccounts.Include(a => a.ExecutedOrders).First(a => a.AccountNo == account.AccountNo);
                        var history = await ctx.OpenApiService.GetHistoricalTrades((long)act.CtidTraderAccountId, act.IsLive, parent.LastImportedOn, to);
                        await OpenApiImporter.ImportHistoryAsync(history, ctx.OpenApiService, parent);

                        parent.LastImportedOn = to;
                        
                        processedAccounts++;
                    }
                    dbContext.SaveChanges();
                }
            }
            return processedAccounts;
        }

        public async Task<ChartTrendBarSnapshot> GetTrendBarsAsync(long PositionId)
        {
            ChartTrendBarSnapshot snapshot = new(ChartPeriod.H1, Symbol.EURUSD);

            using (var serviceScope = _scopeFactory.CreateScope())
            {
                JournalDbContext dbContext = serviceScope.ServiceProvider.GetService<JournalDbContext>() ?? throw new ArgumentNullException(nameof(serviceScope));
                ClosedPosition? position = dbContext.ClosedPositions.Include(p => p.TradingAccount).First(p => p.PositionId == PositionId);

                if (position == null)
                {
                    return snapshot;
                }

                DateTimeOffset openedAt = position.OrderOpenedAt.Subtract(TimeSpan.FromDays(5));
                DateTimeOffset closedAt = position.OrderClosedAt.AddDays(5);

                if (closedAt > DateTimeOffset.Now)
                {
                    closedAt = DateTimeOffset.Now;
                }

                TradingAccount acc = position.TradingAccount;
                long symbol = (long)position.Symbol;

                LoginContext ctx = _loginContexts.First().Value;
                await ctx.ConnectAsync();

                ctx.OpenApiService.AuthorizeAccount((long)acc.CTraderAccountId, acc.IsLive, ctx.CTraderAccount.AccessToken);

                var trendBars = await ctx.OpenApiService.GetTrendbars((long)acc.CTraderAccountId, acc.IsLive, openedAt, closedAt, ProtoOATrendbarPeriod.H1, symbol);

                long digits = 100000;

                NumberFormatInfo precision = new();
                precision.NumberDecimalDigits = 5;
                List<string> ohlc;

                foreach (var bar in trendBars)
                {
                    ohlc = new List<string>();
                    ohlc.Add((((long)bar.UtcTimestampInMinutes) * 60000).ToString());
                    ohlc.Add(decimal.Divide(bar.Low + (long)bar.DeltaOpen, digits).ToString("N", precision));
                    ohlc.Add(decimal.Divide(bar.Low + (long)bar.DeltaHigh, digits).ToString("N", precision));
                    ohlc.Add(decimal.Divide(bar.Low, digits).ToString("N", precision));
                    ohlc.Add(decimal.Divide(bar.Low + (long)bar.DeltaClose, digits).ToString("N", precision));

                    snapshot.TrendBars.Add(ohlc.ToArray());
                }
            }

            return snapshot;
        }
    }

    internal class LoginContext
    {
        private static OpenClient LiveClientFactory() => new(ApiInfo.LiveHost, ApiInfo.Port, TimeSpan.FromSeconds(10));
        private static OpenClient DemoClientFactory() => new(ApiInfo.DemoHost, ApiInfo.Port, TimeSpan.FromSeconds(10));

        static readonly string openApiUrl = "https://openapi.ctrader.com/apps/auth?client_id={0}&redirect_uri={1}&scope=accounts";
        private readonly CTraderAccount forAccount;

        private bool isConnected = false;
        private Task? connection;

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

        public OpenApiService? OpenApiService { get; private set; }

        public TradingAccountsService? TradingAccountsService { get; private set; }

        public async Task<OpenApiService> ConnectAsync()
        {
            if (! this.isConnected)
            {
                ApiCredentials creds = new();
                creds.ClientId = forAccount.ClientId;
                creds.Secret = forAccount.ClientSecret;
                OpenApiService = new OpenApiService(LiveClientFactory, DemoClientFactory);
                await OpenApiService.Connect(creds);
                TradingAccountsService = new TradingAccountsService(OpenApiService);
                isConnected = true;
            };
            
            return OpenApiService;
        }
    }
}
