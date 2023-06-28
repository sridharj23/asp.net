using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartFxJournal.Common.Services;
using SmartFxJournal.JournalDB.model;

namespace SmartFxJournal.Controllers
{
    [ApiController]
    public class AnalysisController : ControllerBase
    {
        private readonly PositionAnalysisService _service;
        public AnalysisController(PositionAnalysisService service) { 
            _service = service;
        }

        [HttpGet("api/analysis/for_position/{positionId}")]
        public async Task<ActionResult<List<PositionAnalysisEntry>>> GetAnalyisForPosition(long positionId)
        {
            return await _service.GetAnalysisEntriesAsync(positionId);
        }

        [HttpPost("api/analysis/")]
        public ActionResult<bool> CreateAnalysisEntries(List<PositionAnalysisEntry> entries)
        {
            return _service.SaveAnalysisEntries(entries, true);
        }

        [HttpPut("api/analysis/{entryId}")]
        public ActionResult<bool> UpdateAnalysisEntries(long entryId, PositionAnalysisEntry entry)
        {
            var list = new List<PositionAnalysisEntry>();
            list.Add(entry);
            return _service.SaveAnalysisEntries(list, false);
        }
    }
}
