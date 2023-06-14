using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static SmartFxJournal.JournalDB.model.GlobalEnums;

namespace SmartFxJournal.CTrader.Models
{
    public class ChartTrendBarSnapshot
    {
        public ChartTrendBarSnapshot(ChartPeriod period, Symbol symbol) 
        { 
            this.TimePeriod = period;
            this.Symbol = symbol;
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ChartPeriod TimePeriod { get; init; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Symbol Symbol { get; init; }

        public long PositionOpenedAt { get; set; }

        public decimal PositionOpenPrice { get; set; }

        public long PositionClosedAt { get; set; }

        public decimal PositionClosePrice { get; set; }

        public List<string[]> TrendBars { get; set; } = new List<string[]>();
    }

    public class OHLC
    {
        public string Open { get; set; } = null!;
        public string High { get; set; } = null!;
        public string Low { get; set; } = null!;
        public string Close { get; set; } = null!;
        public DateTimeOffset OpenTimeStamp { get; set; }
    }
}
