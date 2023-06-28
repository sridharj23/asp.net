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
    public class PositionAnalysisService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly long Factor = 10000;

        public PositionAnalysisService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<List<PositionAnalysisEntry>> GenerateDefaultAnalysis(long PositionId)
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                JournalDbContext _context = serviceScope.ServiceProvider.GetService<JournalDbContext>() ?? throw new ArgumentNullException(nameof(serviceScope));
                var pos = await _context.ClosedPositions.Include(p => p.AnalysisEntries).FirstAsync(p => p.PositionId == PositionId);
                var entries = pos.AnalysisEntries.ToList();
                if (entries.Count == 0)
                {
                    decimal pl = Math.Abs(pos.EntryPrice - pos.ExitPrice) * Factor;
                    PositionAnalysisEntry EntryAnalysis = new()
                    {
                        PositionId = PositionId,
                        AnalysisScenario = GlobalEnums.AnalysisScenario.Actual,
                        AnalyzedAspect = GlobalEnums.AnalyzedAspect.Entry,
                        ExecutionPrice = pos.EntryPrice,
                        ExecutionTime = pos.OrderOpenedAt,
                        Volume = pos.Volume,
                        UsedIndicator = "Unknown",
                        UsedSystem = "Unknown"
                    };
                    PositionAnalysisEntry ExitAnalysis = new()
                    {
                        PositionId = PositionId,
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

                    pos.AnalysisEntries.Add(EntryAnalysis);
                    pos.AnalysisEntries.Add(ExitAnalysis);
                    _context.SaveChanges();

                    entries.Add(EntryAnalysis);
                    entries.Add(ExitAnalysis);
                }
                return entries;
            }
        }

        public async Task<List<PositionAnalysisEntry>> GetAnalysisEntriesAsync(long PositionId)
        {
            return await GenerateDefaultAnalysis(PositionId);
        }

        public PositionAnalysisEntry SaveAnalysisEntry(PositionAnalysisEntry entry, bool isNew) {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                JournalDbContext _context = serviceScope.ServiceProvider.GetService<JournalDbContext>() ?? throw new ArgumentNullException(nameof(serviceScope));
                if (isNew)
                {
                    _context.PositionAnalysisEntries.Add(entry);
                } else
                {
                    _context.PositionAnalysisEntries.Update(entry);
                }
                _context.SaveChanges();


            }

            return entry;
        }

            public bool SaveAnalysisEntries(List<PositionAnalysisEntry> entries, bool isNew)
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                JournalDbContext _context = serviceScope.ServiceProvider.GetService<JournalDbContext>() ?? throw new ArgumentNullException(nameof(serviceScope));

                if (isNew)
                {
                    _context.PositionAnalysisEntries.AddRange(entries);
                }
                else
                {
                    _context.PositionAnalysisEntries.UpdateRange(entries);
                }
            }
            return true;
        }
    }
}
