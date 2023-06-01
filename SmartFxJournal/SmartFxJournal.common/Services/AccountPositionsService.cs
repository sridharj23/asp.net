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

        public List<SummaryAggregate> GetSummaryAggregates(long AccountNo) 
        {
            Dictionary<int, SummaryAggregate> aggregates = new();

            // Prepare aggregates
            int curYear = DateTime.Now.Year;
            int curMonth = DateTime.Now.Month;
            int prevYear = curYear - 1;

            aggregates.Add(curYear, new SummaryAggregate(curYear.ToString()));
            aggregates.Add(prevYear, new SummaryAggregate(prevYear.ToString()));

            for (int i = 1; i <= curMonth; i++) 
            {
                string key = new DateTime(curYear, i, 1).ToString("MMM", CultureInfo.InvariantCulture) + "-" + curYear.ToString();
                aggregates.Add(i, new SummaryAggregate(key));
            }

            // Collect aggregate data
            FxAccount acc = _context.FxAccounts.Include(a => a.OrderHistory).First(a => a.AccountNo == AccountNo);
            List<FxHistoricalTrade> trades = acc.OrderHistory.Where(o => o.IsClosing == true && o.OrderOpenedAt?.Year >= prevYear).OrderBy(o => o.OrderOpenedAt).ToList();


            foreach(FxHistoricalTrade tr in trades) {
                int month = tr.OrderOpenedAt?.Month ?? 0;
                if (tr.OrderOpenedAt?.Year == prevYear) {
                    CollectAggregate(tr, aggregates[prevYear]);
                } else if (tr.OrderOpenedAt?.Year == curYear) {
                    CollectAggregate(tr, aggregates[curYear]);
                    if (month > 0) {
                        CollectAggregate(tr, aggregates[month]);
                    }
                }
            }

            return aggregates.Values.ToList().FindAll(a => a.TotalPL != 0);
        }

        private void CollectAggregate(FxHistoricalTrade tr, SummaryAggregate agg) {
            decimal tot = tr.GrossProfit + tr.Commission + tr.Swap;
            agg.TotalPL += tot;
            if (tr.Direction == GlobalEnums.TradeDirection.BUY) {
                agg.PLFromLongs += tot;
            } else {
                agg.PLFromShorts += tot;
            }
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
                        ExecutionTime = tra.OrderOpenedAt?.DateTime.ToString(new CultureInfo("de-DE"))
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
                        ExecutionTime = tra.OrderOpenedAt?.DateTime.ToString(new CultureInfo("de-DE"))
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
