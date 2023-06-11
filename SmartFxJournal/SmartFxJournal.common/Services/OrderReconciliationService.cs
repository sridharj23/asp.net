using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartFxJournal.Common.Model;
using SmartFxJournal.JournalDB.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFxJournal.Common.Services
{
    public class OrderReconciliationService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public OrderReconciliationService(IServiceScopeFactory scopeFactory) { 
            _scopeFactory = scopeFactory;
        }

        public async Task<List<ExecutedOrder>> OrdersToReconcile(long AccountNo) { 
            List<ExecutedOrder> orders;

            using (var serviceScope = _scopeFactory.CreateScope())
            {
                JournalDbContext dbContext = serviceScope.ServiceProvider.GetService<JournalDbContext>() ?? throw new ArgumentNullException(nameof(serviceScope));
                orders = await dbContext.ExecutedOrders.Where(o => o.PositionId == -1* AccountNo).OrderBy(o => o.OrderExecutedAt).ToListAsync();
                foreach (var order in orders)
                {
                    order.PositionId = 0;
                }
            }

            return orders;
        }

        public async Task<ReconciliationResult> ReconcilePositions(long Account, List<ReconcileEntry> entries)
        {
            ReconciliationResult result = new();
            int total = entries.Count;
            int processed = 0;

            using (var serviceScope = _scopeFactory.CreateScope())
            {
                JournalDbContext dbContext = serviceScope.ServiceProvider.GetService<JournalDbContext>() ?? throw new ArgumentNullException(nameof(serviceScope));
                TradingAccount account = await dbContext.TradingAccounts.Include(a => a.Positions).Include(a => a.ExecutedOrders).FirstAsync(a => a.AccountNo == Account);

                int count = entries.Count;
                foreach (var entry in entries)
                {
                    ClosedPosition pos = new()
                    {
                        PositionId = entry.PositionId,
                        PositionStatus = GlobalEnums.PositionStatus.Closed,
                        AccountNo = Account,
                        IsMultiOrderPosition = entry.Orders.Count > 2
                    };

                    long closedVol = 0;
                    bool opened = false, closed = false;
                    foreach (var order in entry.Orders)
                    {
                        ExecutedOrder theOrder = account.ExecutedOrders.First(o => o.OrderId == order);
                        pos.ExecutedOrders.Add(theOrder);
                        if (theOrder != null)
                        {
                            pos.Commission += theOrder.Commission;
                            pos.Swap += theOrder.Swap;
                            decimal avgPrice = theOrder.ExecutionPrice;

                            if (theOrder.IsClosing)
                            {
                                decimal balance = theOrder.BalanceAfter;
                                if (closed)
                                {
                                    avgPrice = CalculateAveragePrice(theOrder.ClosedVolume, theOrder.ExecutionPrice, closedVol, pos.ExitPrice);
                                    if (theOrder.OrderExecutedAt > pos.OrderClosedAt)
                                    {
                                        // later close order so more latest balance
                                        balance = theOrder.BalanceAfter;
                                    }
                                }
                                closedVol += theOrder.ClosedVolume;
                                pos.ExitPrice = avgPrice;
                                pos.OrderClosedAt = closed ? MaxOfDate(theOrder.OrderExecutedAt, pos.OrderClosedAt) : theOrder.OrderExecutedAt;
                                pos.GrossProfit += theOrder.GrossProfit;
                                pos.BalanceAfter = balance;
                                closed = true;
                            } else
                            {
                                if (opened)
                                {
                                    avgPrice = CalculateAveragePrice(theOrder.FilledVolume, theOrder.ExecutionPrice, pos.Volume, pos.EntryPrice);
                                }
                                pos.Symbol = theOrder.Symbol;
                                pos.Direction = theOrder.Direction;
                                pos.Volume += theOrder.FilledVolume;
                                pos.EntryPrice = avgPrice;
                                pos.OrderOpenedAt = opened ? MinOfDate(pos.OrderOpenedAt, theOrder.OrderExecutedAt) : theOrder.OrderExecutedAt;
                                opened = true;
                            }
                        }
                    }
                    pos.NetProfit = pos.GrossProfit + pos.Commission + pos.Swap;
                    pos.LastUpdatedAt = DateTime.UtcNow;
                    account.Positions.Add(pos);
                    processed += 1;
                }
                dbContext.SaveChanges();
            }

            result.Result = "Successfully imported " + processed.ToString() +  " of  " + total.ToString() + " positions";
            return result;
        }


        private DateTimeOffset MaxOfDate(DateTimeOffset one, DateTimeOffset two)
        {
            return one > two ? one : two;
        }

        private DateTimeOffset MinOfDate(DateTimeOffset one, DateTimeOffset two)
        {
            return one > two ? two : one;
        }

        private decimal CalculateAveragePrice(long vol1, decimal price1, long vol2, decimal price2)
        {
            return ((vol1 * price1) + (vol2 * price2)) / (vol1 + vol2); 
        }
    }
}
