using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFxJournal.JournalDB.model
{
    public class GlobalEnums
    {
        public enum CurrencyType
        {
            EUR,
            USD
        }
        public enum ImportMode
        {
            CSV,
            cTrader
        }
        public enum TradeDirection
        {
            Buy = 1,
            Sell = 2
        }
        public enum PositionStatus
        {
            Open = 1,
            Closed = 2,
            Created = 3,
            Error = 4
        }
        public enum OrderType
        {
            Market = 1,
            Limit = 2,
            Stop = 3,
            StopLossTakeProfit = 4,
            MarketRange = 5,
            StopLimit = 6
        }
        public enum OrderStatus
        {
            Accepted = 1,
            Filled = 2,
            Rejected = 3,
            Expired = 4,
            Cancelled = 5
        }
    }
}
