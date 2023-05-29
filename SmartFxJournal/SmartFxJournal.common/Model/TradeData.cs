using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFxJournal.Common.Model
{
    public class TradeData
    {
        public long OderId { get; set; }

        public String ExecutionTime { get; set; } = null!;

        public decimal FilledVolume { get; set; }

        public decimal BalanceAfterExecution { get; set; }

        public string Direction { get; set; } = null!;

        public decimal Price { get; set; }

        public decimal Swap { get; set; }

        public decimal Commission { get; set; }

        public decimal GrossProfit { get; set; }

        public decimal NetProfit { get; set; }
    }
}
