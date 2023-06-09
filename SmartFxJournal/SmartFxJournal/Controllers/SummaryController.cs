﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ObjectPool;
using SmartFxJournal.Common.Model;
using SmartFxJournal.Common.Services;
using SmartFxJournal.CTrader.Services;

namespace SmartFxJournal.Controllers
{
    [ApiController]
    public class SummaryController : ControllerBase
    {
        private readonly AccountSummaryService _service;

        public SummaryController(AccountSummaryService service)
        {
            _service = service;
        }

        [HttpGet("api/Summary/{AccountNo}/equity")]
        public async Task<ActionResult<EquityCurve>> GetEquityCurve(long AccountNo) 
        {
            return await _service.GetEquityCurveAsync(AccountNo);
        }

        [HttpGet("api/Summary/{AccountNo}/overview")]
        public async Task<ActionResult<List<SummaryAggregate>>> GetSummaryAggregates(long AccountNo)
        {
            return await _service.GetSummaryAggregatesAsync(AccountNo);
        }

        [HttpGet("api/Summary/{AccountNo}/aggregates")]
        public async Task<ActionResult<Dictionary<string, List<string[]>>>> GetAccountAnalysis(long AccountNo, string? analysisType)
        {
            return await _service.GetAccountAnalysis(AccountNo, analysisType);
        }
    }
}
