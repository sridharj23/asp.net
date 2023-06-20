using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartFxJournal.Common.Services;
using SmartFxJournal.JournalDB.model;

namespace SmartFxJournal.Controllers
{
    [ApiController]
    public class AnalysisController : ControllerBase
    {
        private readonly AnalysisEntryService _service;
        public AnalysisController(AnalysisEntryService service) { 
            _service = service;
        }

        [HttpGet("api/analysis/for_position/{positionId}")]
        public async Task<ActionResult<List<AnalysisEntry>>> GetAnalyisForPosition(long positionId)
        {
            return await _service.GetAnalysisEntriesAsync(positionId);
        }
    }
}
