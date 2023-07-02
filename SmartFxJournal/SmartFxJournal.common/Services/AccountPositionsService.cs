using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartFxJournal.Common.Analyzers;
using SmartFxJournal.Common.Model;
using SmartFxJournal.JournalDB.model;
using System.Globalization;
using System.Security.Principal;

namespace SmartFxJournal.Common.Services
{
    public class AccountPositionsService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public AccountPositionsService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<List<ClosedPosition>> GetPositionsAsync(long AccountNo) 
        {
            IAnalyzer analyzer = AnalyzerFactory.GetAnalyzer("1to1PercentPL");

            using (var serviceScope = _scopeFactory.CreateScope())
            {
                JournalDbContext _context = serviceScope.ServiceProvider.GetService<JournalDbContext>() ?? throw new ArgumentNullException(nameof(serviceScope));
                return await _context.ClosedPositions.Where(p => p.AccountNo == AccountNo && p.PositionId > 0).OrderByDescending(p => p.OrderClosedAt).ToListAsync();

                /* 
                 
                 var positions = await _context.ClosedPositions.Include(p => p.AnalysisEntries).Where(p => p.AccountNo == AccountNo && p.PositionId != -1*AccountNo).OrderByDescending(p => p.OrderClosedAt).ToListAsync();
                for(var i = positions.Count-1; i >= 0 ; i-- )
                {
                    analyzer.Analyze(positions[i]);
                }

                return positions;
                */


            }
        }

        public async Task<ClosedPosition> GetPositionAsync(long PositionId)
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                JournalDbContext _context = serviceScope.ServiceProvider.GetService<JournalDbContext>() ?? throw new ArgumentNullException(nameof(serviceScope));
                return await _context.ClosedPositions.Include(p => p.ExecutedOrders).Where(p => p.PositionId == PositionId).FirstAsync();
            }
        }

        public async Task<ClosedPosition> SavePosition(ClosedPosition position)
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                JournalDbContext _context = serviceScope.ServiceProvider.GetService<JournalDbContext>() ?? throw new ArgumentNullException(nameof(serviceScope));
                var pos = _context.ClosedPositions.Update(position);
                await _context.SaveChangesAsync();
                return pos.Entity;
            }

        }
    }
}
