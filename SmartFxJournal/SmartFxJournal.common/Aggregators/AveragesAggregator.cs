using SmartFxJournal.JournalDB.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFxJournal.Common.Aggregators
{
    internal class AveragesAggregator : TotalsAggregator
    {
        public static new IAggregator Create() { return new AveragesAggregator(); }

        public static new IAggregator Create2() { return new AveragesAggregator(true); }

        public AveragesAggregator(bool significant = false) : base(significant) {
        }

        public override List<string[]> GetAggregateItems()
        {
            var items = new List<string[]>()
            {
                new string[4] { "", "All", "Longs", "Shorts" },
                new string[4] { "Avg. Won (€)", ToStr(winEuro[0] / trades[0]), ToStr(winEuro[1] / trades[1]), ToStr(winEuro[2] / trades[2]) },
                new string[4] { "Avg. Lost (€)", ToStr(lossEuro[0] / trades[0]), ToStr(lossEuro[1] / trades[1]), ToStr(lossEuro[2] / trades[2]) },
                new string[4] { "Avg. PIPs Won", ToStr(winPIPs[0] / trades[0]), ToStr(winPIPs[1] / trades[1]), ToStr(winPIPs[2] / trades[2])},
                new string[4] { "Avg. PIPs Lost", ToStr(lossPIPs[0] / trades[0]), ToStr(lossPIPs[1] / trades[1]), ToStr(lossPIPs[2] / trades[2]) }
            };

            return items;
        }
    }
}
