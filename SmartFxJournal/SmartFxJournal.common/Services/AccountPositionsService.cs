using Microsoft.EntityFrameworkCore;
using SmartFxJournal.Common.Model;
using SmartFxJournal.JournalDB.model;
using System.Globalization;
using System.Security.Principal;

namespace SmartFxJournal.Common.Services
{
    public class AccountPositionsService
    {
        private readonly JournalDbContext _context;

        public AccountPositionsService(JournalDbContext context) 
        { 
            _context = context;
        }
        public List<TradePosition> GetPositions(long AccountNo) 
        {
            FxAccount acc = _context.FxAccounts.Include(a => a.OrderHistory).First(a => a.AccountNo == AccountNo); 
            var positions = new List<TradePosition>();
            List<FxHistoricalTrade> trades = acc.OrderHistory.OrderBy(o => o.PositionId).ThenBy(o => o.OrderOpenedAt).ToList();

            TradePosition pos = null;
            decimal totSwap = decimal.Zero;
            decimal totComm = decimal.Zero;
            decimal totProfit = decimal.Zero;

            foreach(var tra in trades)
            {

                if (tra.IsClosing == false)
                {
                    if (pos != null)
                    {
                        pos.Commission = totComm;
                        pos.Swap = totSwap;
                        pos.GrossProfit = totProfit;
                        positions.Add(pos);
                        totProfit = 0;
                        totSwap = 0;
                        totComm = 0;
                    }

                    pos = new()
                    {
                        PositionId = tra.PositionId,
                        AccountNo = tra.AccountNo,
                        Symbol = tra.Symbol.ToString(),
                    };
                    TradeData opening = new()
                    {
                        OderId = tra.OrderId,
                        Direction = tra.Direction.ToString(),
                        Price = tra.ExecutionPrice,
                        Swap = tra.Swap,
                        Commission = tra.Commission,
                        FilledVolume = tra.VolumeInLots,
                        GrossProfit = tra.GrossProfit,
                        NetProfit = tra.GrossProfit + tra.Commission + tra.Swap,
                        BalanceAfterExecution = tra.BalanceAfterClose,
                        ExecutionTime = ((DateTimeOffset)tra.OrderOpenedAt).DateTime.ToString(new CultureInfo("de-DE"))
                    };
                    pos.OpenedOrders.Add(opening);
                } else if (pos != null)
                {
                    TradeData closed = new()
                    {
                        OderId = tra.OrderId,
                        Direction = tra.Direction.ToString(),
                        Price = tra.ExecutionPrice,
                        Swap = tra.Swap,
                        Commission = tra.Commission,
                        FilledVolume = tra.VolumeInLots,
                        GrossProfit = tra.GrossProfit,
                        NetProfit = tra.GrossProfit + tra.Commission + tra.Swap,
                        BalanceAfterExecution = tra.BalanceAfterClose,
                        ExecutionTime = ((DateTimeOffset)tra.OrderOpenedAt).DateTime.ToString(new CultureInfo("de-DE"))
            };

                    pos.ClosedOrders.Add(closed);
                }
                totSwap += tra.Swap;
                totComm += tra.Commission;
                totProfit += tra.GrossProfit;
            }

            // add the last position
            if (pos != null) 
            {
                pos.Commission = totComm;
                pos.Swap = totSwap;
                pos.GrossProfit = totProfit;
                positions.Add(pos); 
            } 

            return positions;
        }
    }
}
