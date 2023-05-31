using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartFxJournal.Common.Model;
using SmartFxJournal.Common.Services;
using SmartFxJournal.CTrader.Services;

namespace SmartFxJournal.Controllers
{
    [ApiController]
    public class SummaryController : ControllerBase
    {
        private readonly AccountPositionsService _service;

        public SummaryController(AccountPositionsService service)
        {
            _service = service;
        }

        [HttpGet("api/Summary/{AccountNo}/equity")]
        public EquityCurve GetEquityCurve(long AccountNo) 
        {
            return _service.GetEquityCurve(AccountNo);
        }

    }
}
