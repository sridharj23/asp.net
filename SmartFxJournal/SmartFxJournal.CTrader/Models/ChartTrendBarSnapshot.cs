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

        public Symbol Symbol { get; init; }

        public List<OHLC> TrendBars { get; set; } = new List<OHLC>();
    }

    public class OHLC
    {
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public DateTimeOffset OpenTimeStamp { get; set; }
    }
}
