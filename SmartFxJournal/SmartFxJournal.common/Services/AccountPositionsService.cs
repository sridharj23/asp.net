using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                JournalDbContext _context = serviceScope.ServiceProvider.GetService<JournalDbContext>() ?? throw new ArgumentNullException(nameof(serviceScope));
                TradingAccount acc = await _context.TradingAccounts.Include(a => a.Positions).FirstAsync(a => a.AccountNo == AccountNo);
                return acc.Positions.Where(o => o.PositionId > 0).OrderByDescending(o => o.OrderClosedAt).ToList();
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
    }
}
