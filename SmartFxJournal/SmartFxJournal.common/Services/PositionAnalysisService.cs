using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartFxJournal.Common.Helpers;
using SmartFxJournal.JournalDB.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartFxJournal.JournalDB.model.GlobalEnums;

namespace SmartFxJournal.Common.Services
{
    public class PositionAnalysisService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        private static readonly AnalyzedAspect[] ToGenerate = { AnalyzedAspect.Entry, AnalyzedAspect.Exit, AnalyzedAspect.StopLoss, AnalyzedAspect.TakeProfit };

        public PositionAnalysisService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        private bool isActualAspect(PositionAnalysisEntry p, AnalyzedAspect aspect)
        {
            return (p.AnalysisScenario == AnalysisScenario.Actual && p.AnalyzedAspect == aspect);
        }

        public async Task<List<PositionAnalysisEntry>> GenerateDefaultAnalysis(long PositionId)
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                JournalDbContext _context = serviceScope.ServiceProvider.GetService<JournalDbContext>() ?? throw new ArgumentNullException(nameof(serviceScope));
                var pos = await _context.ClosedPositions.Include(p => p.AnalysisEntries).FirstAsync(p => p.PositionId == PositionId);
                var entries = pos.AnalysisEntries.ToList();

                AnalysisEntryGenerator generator = new AnalysisEntryGenerator();

                foreach(AnalyzedAspect aspect in  ToGenerate)
                {
                    if(entries.FirstOrDefault(e => isActualAspect(e, aspect)) == null)
                    {
                        pos.AnalysisEntries.Add(generator.CreateEntry(pos, aspect));
                    }
                }

                if (pos.AnalysisStatus == AnalysisStatus.Pending)
                {
                    pos.AnalysisStatus = AnalysisStatus.Partial;
                }

                if (_context.ChangeTracker.HasChanges())
                {
                    _context.SaveChanges();
                }

                return pos.AnalysisEntries.ToList();
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
    }
}
