using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartFxJournal.CTrader.Models;
using SmartFxJournal.CTrader.Services;
using SmartFxJournal.JournalDB.model;

namespace SmartFxJournal.Controllers
{
    [ApiController]
    public class CtraderController : ControllerBase
    {
        private readonly CTraderService _service;
        private readonly String redirectUri = "https://localhost:5000/api/ctrader/auth";
        private static String cTraderId = "";

        public CtraderController(CTraderService service) {
            _service = service;
        }


        [HttpGet("api/ctrader/auth")]
        public async Task<ActionResult<OnBoardingResult?>> ConsumeAuthCodeAsync(String? code, String error = "", String error_description = "")
        {
            if (CtraderController.cTraderId == "")
            {
                return NotFound("No active authorization flows detected !");
            } else if (code != null)
            {
                var result = await _service.ProcessCTraderAccountAuthorization(cTraderId, code);
                result.ErrorDescription = result.OnBoardingStatus == OnBoardStatus.Success ? "You can close this browser window !" : result.ErrorDescription;
                return result;
            } else if (error != null)
            {
                _service.FailCTraderAccountAuthFlow(CtraderController.cTraderId, error, error_description);
                CtraderController.cTraderId = "";
                return Ok("Athorization aborted : " + error + " - " + error_description);
            }
            throw new Exception("Authorization flow failed with reason : " + error_description);
        }


        [HttpGet("api/ctrader/onboardresult/{ctraderid}")]
        public ActionResult<OnBoardingResult> GetOnBoardingResult(String ctraderid)
        {
            return _service.OnBoardingResult(ctraderid);
        }


        [HttpGet("api/ctrader/haslogin/{ctraderid}")]
        public async Task<ActionResult<Boolean>> HasLoginCredentials(String ctraderid)
        {
            return await _service.HasLogonCredentials(ctraderid);
        }


        [HttpGet("api/ctrader/login")]
        public ActionResult<string> AttemptLogin(String cTraderId, String client_id, String client_secret)
        {
            CtraderController.cTraderId = cTraderId;
            Uri uri = _service.PrepareNewCTraderAccountAuthorizationContext(cTraderId, client_id, client_secret, redirectUri);
            return uri.ToString();
        }

        [HttpPost("api/ctrader/import/{ctraderid}")]
        public async Task<ActionResult<string>> ImportAccounts(string ctraderid, long accountNo)
        {
            return await _service.ImportAccounts(ctraderid, accountNo);
        }

        [HttpGet("api/ctrader/trendbars/{positionid}")]
        public async Task<ActionResult<ChartTrendBarSnapshot>> GetTrendBarSnapshot(long positionid)
        {
            return await _service.GetTrendBarsAsync(positionid);
        }
    }
}
