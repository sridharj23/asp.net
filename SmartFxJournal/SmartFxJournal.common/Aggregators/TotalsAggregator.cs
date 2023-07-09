using SmartFxJournal.JournalDB.model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFxJournal.Common.Aggregators
{
    internal class TotalsAggregator : IAggregator
    {
        private static int PIP_SIZE = 10000;

        public static TotalsAggregator Create() { return new TotalsAggregator(); }

        public static TotalsAggregator Create2() { return new TotalsAggregator(true); }

        protected int[] trades = new int[3] {0, 0, 0};
        protected int[] winners = new int[3] { 0, 0, 0 };
        protected int[] loosers = new int[3] { 0, 0, 0 };
        protected decimal[] profit = new decimal[3] { decimal.Zero, decimal.Zero, decimal.Zero };
        protected decimal[] winEuro = new decimal[3] { decimal.Zero, decimal.Zero, decimal.Zero };
        protected decimal[] lossEuro = new decimal[3] { decimal.Zero, decimal.Zero, decimal.Zero };
        protected decimal[] winPIPs = new decimal[3] { decimal.Zero, decimal.Zero, decimal.Zero };
        protected decimal[] lossPIPs = new decimal[3] { decimal.Zero, decimal.Zero, decimal.Zero };

        protected decimal[] swap = new decimal[3] { decimal.Zero, decimal.Zero, decimal.Zero };
        protected decimal[] commission = new decimal[3] { decimal.Zero, decimal.Zero, decimal.Zero };

        private bool significantOnly = false;

        public TotalsAggregator(bool onlySignificant = false)
        {
            significantOnly = onlySignificant;
        }

        protected virtual void ProcessTotals(ClosedPosition pos, bool isWin)
        {
            trades[0]++;
            profit[0] += pos.NetProfit;
            swap[0] += pos.Swap;
            commission[0] += pos.Commission;

            if (isWin)
            {
                winners[0]++;
                winEuro[0] += pos.NetProfit;
                winPIPs[0] += Math.Abs(pos.EntryPrice - pos.ExitPrice) * PIP_SIZE;
            }
            else
            {
                loosers[0]++;
                lossEuro[0] += pos.NetProfit;
                lossPIPs[0] -= Math.Abs(pos.EntryPrice - pos.ExitPrice) * PIP_SIZE;
            }
        }

        protected virtual void ProcessLong(ClosedPosition pos, bool isWin) 
        {
            trades[1]++;
            profit[1] += pos.NetProfit;
            swap[1] += pos.Swap;
            commission[1] += pos.Commission;

            if (isWin)
            {
                winners[1]++;
                winEuro[1] += pos.NetProfit;
                winPIPs[1] += Math.Abs(pos.EntryPrice - pos.ExitPrice) * PIP_SIZE;

            }
            else
            {
                loosers[1]++;
                lossEuro[1] += pos.NetProfit;
                lossPIPs[1] -= Math.Abs(pos.EntryPrice - pos.ExitPrice) * PIP_SIZE;
            }
        }

        protected virtual void ProcessShort(ClosedPosition pos, bool isWin) 
        {
            trades[2]++;
            profit[2] += pos.NetProfit;
            swap[2] += pos.Swap;
            commission[2] += pos.Commission;

            if (isWin)
            {
                winners[2]++;
                winEuro[2] += pos.NetProfit;
                winPIPs[2] += Math.Abs(pos.EntryPrice - pos.ExitPrice) * PIP_SIZE;
            }
            else
            {
                loosers[2]++;
                lossEuro[2] += pos.NetProfit;
                lossPIPs[2] -= Math.Abs(pos.EntryPrice - pos.ExitPrice) * PIP_SIZE;
            }
        }

        public virtual void Aggregate(ClosedPosition position)
        {
            if (this.significantOnly && Math.Abs(position.NetProfit) < 10) return;

            bool isWin = position.NetProfit > 0;

            ProcessTotals(position, isWin);

            if (position.Direction == GlobalEnums.TradeDirection.BUY)
            {
                ProcessLong(position, isWin);
            } else {
                ProcessShort(position, isWin);
            }
        }

        public virtual List<string[]> GetAggregateItems()
        {
            var items = new List<string[]>()
            {
                new string[4] { "", "All", "Longs", "Shorts" },
                new string[4] { "Trades", trades[0].ToString(), trades[1].ToString(), trades[2].ToString() },
                new string[4] { "Won", winners[0].ToString(), winners[1].ToString(), winners[2].ToString() },
                new string[4] { "Lost", loosers[0].ToString(), loosers[1].ToString(), loosers[2].ToString() },
                new string[4] { "Net Profit", ToStr(profit[0]), ToStr(profit[1]), ToStr(profit[2])},
                new string[4] { "Won (€)", ToStr(winEuro[0]), ToStr(winEuro[1]), ToStr(winEuro[2])},
                new string[4] { "Lost (€)", ToStr(lossEuro[0]), ToStr(lossEuro[1]), ToStr(lossEuro[2]) },
                new string[4] { "PIPs Won", ToStr(winPIPs[0]), ToStr(winPIPs[1]), ToStr(winPIPs[2]) },
                new string[4] { "PIPs Lost", ToStr(lossPIPs[0]), ToStr(lossPIPs[1]), ToStr(lossPIPs[2]) },
                new string[4] { "Commission", ToStr(commission[0]), ToStr(commission[1]), ToStr(commission[2]) },
                new string[4] { "Swap", ToStr(swap[0]), ToStr(swap[1]), ToStr(swap[2]) }
            };
            
            return items;
        }
        protected string ToStr(decimal d) { return d.ToString("0.00", CultureInfo.InvariantCulture); }
    }
}
