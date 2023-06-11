using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartFxJournal.Common.Model;
using SmartFxJournal.Common.Services;
using SmartFxJournal.JournalDB.model;

namespace SmartFxJournal.Controllers
{
    [ApiController]
    public class ReconciliationController : ControllerBase
    {
        private readonly OrderReconciliationService _service;
        public ReconciliationController(OrderReconciliationService service) { 
            _service = service;
        }

        [HttpGet("api/pendingreconcile/{AccountNo}")]
        public Task<List<ExecutedOrder>> OrderToReconcile(long AccountNo)
        {
            return _service.OrdersToReconcile(AccountNo);
        }

        [HttpPost("api/pendingreconcile/{AccountNo}")]
        public async Task<ReconciliationResult> Reconcile(long AccountNo, List<ReconcileEntry> positions)
        {
            return await _service.ReconcilePositions(AccountNo, positions);
        }
    }
}
 