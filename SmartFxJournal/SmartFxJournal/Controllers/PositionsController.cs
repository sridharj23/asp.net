using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartFxJournal.Common.Model;
using SmartFxJournal.Common.Services;
using SmartFxJournal.JournalDB.model;

namespace SmartFxJournal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PositionsController : ControllerBase
    {
        private readonly AccountPositionsService _service;

        public PositionsController(AccountPositionsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<List<ClosedPosition>> GetPositionsAsync(long account)
        {
            if (account == 0) { account = 4091794; }

            return await _service.GetPositionsAsync(account);
        }

    }
}
