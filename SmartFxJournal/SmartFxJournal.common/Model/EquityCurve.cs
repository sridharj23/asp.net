using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFxJournal.Common.Model
{
    public class EquityCurve
    {
        public EquityCurve(long AccountNo) { AccountNumber = AccountNo; }

        public long AccountNumber { get; private set; }

        public string EquityName { get; set; } = "Actual";

        public List<EquityDataPoint> DataPoints { get; private set; } = new ();
    }

    public class EquityDataPoint
    {
        public EquityDataPoint(decimal equity, long ts)
        {
            Equity = equity;
            TimeStamp = ts;
        }

        public decimal Equity { get; private set; }

        public long TimeStamp { get; private set; }
    }
}
