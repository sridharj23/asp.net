using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Samples.Shared.Models;
using SmartFxJournal.CTrader.Services;
using SmartFxJournal.JournalDB.model;

namespace SmartFxJournal.Controllers
{
    [ApiController]
    public class CtraderController : ControllerBase
    {
        private readonly CTraderService _service;
        private readonly String redirectUri = "https://localhost:5000/api/ctrader/auth";

        public CtraderController(CTraderService service) {
            _service = service;
        }

        [HttpGet("api/ctrader/credentials")]
        public ActionResult<ApiCredentials?> Credentials()
        {
            return _service.GetApiCredentials();
        }

        [HttpGet("api/ctrader/auth")]
        public async Task<ActionResult<String?>> RetrieveAuthCodeAsync(String code)
        {
            return await _service.ConsumeAuthCodeAsync(code) ? "You can close this browser window !" : "Something went wrong !" ;
        }

        [HttpGet("api/ctrader/haslogin/{ctraderid}")]
        public async Task<ActionResult<Boolean>> HasLoginCredentials(String ctraderid)
        {
            return await _service.HasLogonCredentials(ctraderid);
        }

        [HttpGet("api/ctrader/login")]
        public ActionResult<bool> AttemptLogin(String cTraderId, String client_id)
        {
            String authUrl = _service.getLoginContext(cTraderId).ClientId(client_id).RedirectUri(redirectUri).buildAuthorizationUri();
            return Redirect(authUrl);
        }
    }
}
