using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFxJournal.Common.Model
{
    public class TradePosition
    {
        public long PositionId { get; set; }

        public long AccountNo { get; set; }

        public string Symbol { get; set; } = null!;

        public decimal GrossProfit { get; set; }

        public decimal Commission { get; set;}

        public decimal Swap { get; set; }

        public decimal Fees { get => Commission + Swap; }

        public decimal NetProfit { get => GrossProfit + Fees; }

        public List<TradeData> OpenedOrders { get; } = new List<TradeData>();

        public List<TradeData> ClosedOrders { get; } = new List<TradeData>();
    }
}
