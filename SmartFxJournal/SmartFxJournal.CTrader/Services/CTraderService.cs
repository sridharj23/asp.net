using Microsoft.Extensions.Configuration;
using OpenAPI.Net.Helpers;
using OpenAPI.Net;
using Samples.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Samples.Shared.Services;
using System.Net;
using System.Net.Http;
using System.Web;
using SmartFxJournal.JournalDB.model;
using OpenAPI.Net.Auth;

namespace SmartFxJournal.CTrader.Services
{
    public class CTraderService
    {   
        private readonly IServiceScopeFactory _scopeFactory;
        private ApiCredentials? _apiCredentials;
        private Dictionary<String, LoginContext> _loginContexts;

        private static HttpClient sharedClient = new()
        {
            BaseAddress = new Uri("https://openapi.ctrader.com")
        };

        public CTraderService(IServiceScopeFactory scopeFactory) {
            _scopeFactory = scopeFactory;

        }

        public void UseCredentials(String clientId, String clientSecret)
        {
            _apiCredentials = new ApiCredentials();
            _apiCredentials.ClientId = clientId;
            _apiCredentials.Secret = clientSecret;
        }

        public static void InitializeAsync(IServiceCollection services, IConfiguration configuration)
        {
            /*
            CTraderService service = new(null);
            var credentials = service.GetApiCredentials();
            OpenClient liveClientFactory() => new(ApiInfo.LiveHost, ApiInfo.Port, TimeSpan.FromSeconds(10));
            OpenClient demoClientFactory() => new(ApiInfo.DemoHost, ApiInfo.Port, TimeSpan.FromSeconds(10));

            services.AddSingleton(credentials);

            var apiService = new OpenApiService(liveClientFactory, demoClientFactory);
            services.AddSingleton<IOpenApiService>(apiService);
            services.AddSingleton<ITradingAccountsService>(new TradingAccountsService(apiService));
            await apiService.Connect(service.GetApiCredentials());
            */
            return ;
        }

        public ApiCredentials GetApiCredentials()
        {
            return new ApiCredentials();
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

        public LoginContext getLoginContext(String cTraderId)
        {
            if (this._loginContexts.ContainsKey(cTraderId)) {
                return this._loginContexts[cTraderId];
            }
            return new LoginContext(cTraderId);
        }

        public async Task<bool> ConsumeAuthCodeAsync(String code)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["grant_type"] = "authorization_code";
            query["code"] = code;
            query["redirect_uri"] = "https://localhost:5000/api/ctrader/auth";
            query["client_id"] = _apiCredentials.ClientId;
            query["client_secret"] = _apiCredentials.Secret;

            var uri = query.ToQueryString();

            using HttpResponseMessage response = await sharedClient.GetAsync("apps/token" + uri);
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();

            
            Console.WriteLine($"{jsonResponse}\n");

            return true;
        }
    }

    public class LoginContext
    {
        static readonly String openApiUrl = "https://openapi.ctrader.com/apps/auth?client_id={0}&redirect_uri={1}&scope=accounts";
        readonly String cTraderId;
        String? clientId;
        String? secret;
        String? redirectUri;

        public LoginContext(String cTraderId)
        {
            this.cTraderId = cTraderId;
        }

        public LoginContext ClientId(String clientId)
        {
            this.clientId = clientId;
            return this;
        }

        public LoginContext ClientSecret(String secret)
        {
            this.secret = secret;
            return this;
        }

        public LoginContext RedirectUri(String uri)
        {
            this.redirectUri = uri;
            return this;
        }

        public String buildAuthorizationUri()
        {
            if (this.clientId == null || this.redirectUri == null) 
            {
                throw new ArgumentNullException("Client Id and Redirect URL are mandatory.");
            }

            return String.Format(openApiUrl, this.clientId, this.redirectUri);
        }
    }
}
