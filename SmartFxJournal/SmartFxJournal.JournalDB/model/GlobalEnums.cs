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
        public enum Symbol
        {
            EURUSD = 1
        }
        public enum ImportMode
        {
            CSV,
            cTrader
        }
        public enum TradeDirection
        {
            BUY = 1,
            SELL = 2
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
        public enum DealStatus
        {
            FILLED = 2,
            PARTIALLY_FILLED = 3,
            REJECTED = 4,
            INTERNALLY_REJECTED = 5,
            ERROR = 6,
            MISSED = 7
        }

        public enum ChartPeriod
        {
            H1 = 60,
            H4 = 240,
            D1 = 1440
        }

        public enum ArtifactType 
        {
            CTraderAccount = 1001,
            TradingAccount = 1002,
            ClosedPosition = 1003,
            ExecutedOrder = 1004,
            NotesEntry = 1005
        }

        public enum AnalyzedAspect
        {
            Entry = 101,
            Exit = 102,
            StopLoss = 103,
            TakeProfit = 104,
            MaxProfit = 105
        }

        public enum AnalysisScenario
        {
            Actual = 201,
            Ideal = 202,
            WhatIf = 203
        }
    }
}
