using OpenAPI.Net.Helpers;
using OpenAPI.Net;
using SmartFxJournal.CTrader.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Web;
using SmartFxJournal.JournalDB.model;
using OpenAPI.Net.Auth;
using System.Text.Json;
using System.Security.Principal;
using static SmartFxJournal.JournalDB.model.GlobalEnums;
using SmartFxJournal.CTrader.Helpers;
using Microsoft.EntityFrameworkCore;
using SmartFxJournal.CTrader.helpers;
using static SmartFxJournal.CTrader.helpers.OffsetIterator;
using System;

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
                    CTraderAccount? cta = dbContext.CTraderAccounts.Include(a => a.FxAccounts).First(c => c.CTraderId == cTraderId) ?? 
                                                throw new Exception("CTrader ID : " + cTraderId + " is not yet onboarded !. Cannot import information.");

                    foreach (var act in protoAccounts)
                    {
                        FxAccount account = await OpenApiImporter.ImportAccountAsync(act, ctx.OpenApiService, cta);
                        dbContext.SaveChanges();

                        FxAccount parent = dbContext.FxAccounts.Include(a => a.Positions).First(a => a.AccountNo == account.AccountNo);
                        parent.Positions.RemoveAll(p => true);

                        var openOrders = await ctx.OpenApiService.GetAccountOrders((long)act.CtidTraderAccountId, act.IsLive);

                        foreach (var position in openOrders.Position)
                        {
                            await OpenApiImporter.ImportPositionAsync(position, ctx.OpenApiService, parent);
                        }

                        DateTimeOffset to = DateTimeOffset.Now;
                        parent = dbContext.FxAccounts.Include(a => a.OrderHistory).First(a => a.AccountNo == account.AccountNo);
                        var history = await ctx.OpenApiService.GetHistoricalTrades((long)act.CtidTraderAccountId, act.IsLive, parent.LastImportedOn, to);
                        //var orders = await ctx.OpenApiService.GetHistoricalOrders((long)act.CtidTraderAccountId, act.IsLive, parent.LastImportedOn, to);
                        await OpenApiImporter.ImportHistoryAsync(history, ctx.OpenApiService, parent);

                        var newOrders = parent.OrderHistory.Where(o => o.PositionId == 0).OrderBy(o => o.DealId).ToList();

                        long deal = 0;
                        long vol = 0;
                        FxHistoricalTrade prev = null;
                        List<FxHistoricalTrade> unmatched = new ();
                        foreach (FxHistoricalTrade trade in newOrders)
                        {
                            if (trade.PositionId > 0)
                            {
                                continue;
                            } else if (trade.IsClosing == false)
                            {
                                if (vol > 0 && prev != null) { unmatched.Add(prev); }
                                prev = trade;
                                trade.PositionId = trade.DealId;
                                deal = trade.DealId;
                                vol = trade.Volume;
                            } else if (vol > 0)  
                            {
                                trade.PositionId = deal;
                                vol -= trade.ClosedVolume;
                            } else
                            {
                                foreach(FxHistoricalTrade unm in unmatched)
                                {
                                    if (unm.Volume == trade.Volume && unm.Direction != trade.Direction)
                                    {
                                        trade.PositionId = unm.PositionId;
                                        unmatched.Remove(unm);
                                        break;
                                    }
                                }
                            }
                        }

                        parent.LastImportedOn = to;
                        
                        processedAccounts++;
                    }
                    dbContext.SaveChanges();
                }
            }
            return processedAccounts;
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
