using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartFxJournal.JournalDB.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFxJournal.Common.Services
{
    public class AnalysisEntryService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly long Factor = 10000;

        public AnalysisEntryService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<List<AnalysisEntry>> GenerateDefaultAnalysis(long PositionId)
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                JournalDbContext _context = serviceScope.ServiceProvider.GetService<JournalDbContext>() ?? throw new ArgumentNullException(nameof(serviceScope));
                var pos = await _context.ClosedPositions.Where(p => p.PositionId == PositionId).FirstAsync();
                var entries = await _context.AnalysisEntries.Where(a => a.ParentId == PositionId && a.ParentType == GlobalEnums.ArtifactType.ClosedPosition).ToListAsync();
                if (entries.Count == 0)
                {
                    decimal pl = Math.Abs(pos.EntryPrice - pos.ExitPrice) * Factor;
                    AnalysisEntry EntryAnalysis = new()
                    {
                        ParentId = PositionId,
                        ParentType = GlobalEnums.ArtifactType.ClosedPosition,
                        AnalysisScenario = GlobalEnums.AnalysisScenario.Actual,
                        AnalyzedAspect = GlobalEnums.AnalyzedAspect.Entry,
                        ExecutionPrice = pos.EntryPrice,
                        ExecutionTime = pos.OrderOpenedAt,
                        Volume = pos.Volume,
                        UsedIndicator = "Unknown",
                        UsedSystem = "Unknown"
                    };
                    AnalysisEntry ExitAnalysis = new()
                    {
                        ParentId = PositionId,
                        ParentType = GlobalEnums.ArtifactType.ClosedPosition,
                        AnalysisScenario = GlobalEnums.AnalysisScenario.Actual,
                        AnalyzedAspect = GlobalEnums.AnalyzedAspect.Exit,
                        ExecutionPrice = pos.ExitPrice,
                        ExecutionTime = pos.OrderClosedAt,
                        Volume = pos.Volume,
                        ProfitLoss = pos.GrossProfit,
                        ProfitInPips = pos.GrossProfit > 0 ? pl : -1 * pl,
                        ProfitInPercent = (pos.GrossProfit / (pos.BalanceAfter - pos.GrossProfit)) * 100,
                        UsedIndicator = "Unknown",
                        UsedSystem = "Unknown"
                    };

                    _context.AnalysisEntries.Add(EntryAnalysis);
                    _context.AnalysisEntries.Add(ExitAnalysis);
                    _context.SaveChanges();

                    entries.Add(EntryAnalysis);
                    entries.Add(ExitAnalysis);
                }
                return entries;
            }
        }

        public async Task<List<AnalysisEntry>> GetAnalysisEntriesAsync(long PositionId)
        {
            return await GenerateDefaultAnalysis(PositionId);
        }

        public bool SaveAnalysisEntries(List<AnalysisEntry> entries, bool isNew)
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                JournalDbContext _context = serviceScope.ServiceProvider.GetService<JournalDbContext>() ?? throw new ArgumentNullException(nameof(serviceScope));
                if (isNew)
                {
                    _context.AnalysisEntries.AddRange(entries);
                }
                else
                {
                    _context.AnalysisEntries.UpdateRange(entries);
                }

            }
            return true;
        }
    }
}
