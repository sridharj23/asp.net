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

        public EquityCurve GetEquityCurve(long AccountNo)
        {
            EquityCurve curve = new(AccountNo);

            FxAccount acc = _context.FxAccounts.Include(a => a.OrderHistory).First(a => a.AccountNo == AccountNo);
            List<FxHistoricalTrade> trades = acc.OrderHistory.Where(o => o.IsClosing == true).OrderBy(o => o.OrderOpenedAt).ToList();

            decimal startBal = acc.StartBalance;
            DateOnly at = acc.OpenedOn;
            DateTime startDate = acc.OpenedOn.ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.FromHours(2)));
            DateTime point = at.ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.FromHours(2)));

            // Add account start balance first
            curve.DataPoints.Add(new(startBal, new DateTimeOffset(startDate).ToUnixTimeMilliseconds()));

            decimal totCommission = decimal.Zero;
            decimal totSwap = decimal.Zero;
            decimal totPL = decimal.Zero;
            decimal lastBal = decimal.Zero;

            foreach(FxHistoricalTrade tra in trades)
            {
                DateTimeOffset dt = (DateTimeOffset)tra.OrderOpenedAt;
                DateOnly cur = DateOnly.FromDateTime(dt.DateTime);

                if (! cur.Equals(at))
                {
                    point = at.ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.FromHours(2)));
                    curve.DataPoints.Add(new(lastBal, (new DateTimeOffset(point)).ToUnixTimeMilliseconds()));
                    at = cur;
                    totCommission = decimal.Zero;
                    totSwap = decimal.Zero;
                    totPL = decimal.Zero;
                }
                totCommission += tra.Commission;
                totSwap += tra.Swap;
                totPL += tra.GrossProfit;
                lastBal = tra.BalanceAfterClose;
                point = cur.ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.FromHours(2)));
            }

            curve.DataPoints.Add(new(lastBal, (new DateTimeOffset(point)).ToUnixTimeMilliseconds()));
            return curve;
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
