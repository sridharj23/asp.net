using SmartFxJournal.JournalDB.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static SmartFxJournal.JournalDB.model.GlobalEnums;

namespace SmartFxJournal.Common.Helpers
{
    internal class AnalysisEntryGenerator
    {
        private static readonly long PIP_MULTIPLIER = 10000;
        private static readonly long LOT_SIZE = 100000;
        private static readonly decimal PIP_DIVISOR = new decimal(0.0001);

        private static decimal GetPips(decimal from, decimal to) => Math.Abs(from - to) * PIP_MULTIPLIER;
        private static decimal PipValue(decimal price, long lotSize) => (PIP_DIVISOR / price) * lotSize;
        private static decimal Percent(decimal value, decimal baseValue) => (100 * value) / baseValue;
        private static decimal StartBalance(ClosedPosition pos) => pos.BalanceAfter - pos.GrossProfit;

        public PositionAnalysisEntry CreateEntry(ClosedPosition pos, AnalyzedAspect aspect)
        {
            return this.CreateEntry(pos, AnalysisScenario.Actual, aspect);
        }

        public PositionAnalysisEntry CreateEntry(ClosedPosition pos, AnalysisScenario scenario, AnalyzedAspect aspect )
        {
            PositionAnalysisEntry entry = CreateBaseEntry(pos, scenario, aspect);

            switch(aspect)
            {
                case AnalyzedAspect.Entry:
                    entry = ComputeEntryAnalysis(pos, entry);
                    break;
                case AnalyzedAspect.Exit:
                    entry = ComputeExitAnalysis(pos, entry);
                    break;
                case AnalyzedAspect.StopLoss:
                    entry = ComputeStopAnalysis(pos, entry);
                    break;
                case AnalyzedAspect.TakeProfit:
                    entry = ComputeTakeProfitAnalysis(pos, entry);
                    break;
            }
            return entry;
        }

        private PositionAnalysisEntry CreateBaseEntry (ClosedPosition pos, AnalysisScenario scenario, AnalyzedAspect aspect)
        {
            return new()
            {
                PositionId = pos.PositionId,
                AnalysisScenario = scenario,
                AnalyzedAspect = aspect,
                Volume = pos.Volume,
                UsedIndicator = "Unknown",
                UsedSystem = "Unknown"
            };
        }

        private PositionAnalysisEntry ComputeEntryAnalysis(ClosedPosition pos, PositionAnalysisEntry entry) 
        {
            entry.ExecutionPrice = pos.EntryPrice;
            entry.ExecutionTime = pos.OrderOpenedAt;
            return entry;
        }

        private PositionAnalysisEntry ComputeExitAnalysis(ClosedPosition pos, PositionAnalysisEntry entry)
        {
            decimal pl = GetPips(pos.EntryPrice, pos.ExitPrice);

            entry.ExecutionPrice = pos.ExitPrice;
            entry.ExecutionTime = pos.OrderClosedAt;
            entry.ProfitLoss = pos.GrossProfit;
            entry.ProfitInPips = pos.GrossProfit > 0 ? pl : -1 * pl;
            entry.ProfitInPercent = Percent(pos.GrossProfit, StartBalance(pos));
            return entry;
        }

        private PositionAnalysisEntry ComputeStopAnalysis(ClosedPosition pos, PositionAnalysisEntry entry)
        {
            decimal risk = Math.Round(StartBalance(pos) / 100, 2);
            bool isSell = pos.Direction == TradeDirection.SELL;
            decimal pipVal = PipValue(pos.EntryPrice, pos.Volume);
            decimal riskPips = Math.Round((risk / pipVal), 1);
            decimal riskPipDecimal = riskPips * PIP_DIVISOR;

            entry.ExecutionPrice = isSell ? pos.EntryPrice + riskPipDecimal : pos.EntryPrice - riskPipDecimal;
            entry.ExecutionTime = DateTimeOffset.UtcNow;
            entry.ProfitInPercent = -1;
            entry.ProfitInPips = -1 * riskPips;
            entry.ProfitLoss = -1 * risk;
            entry.UsedSystem = "OnePercent";

            return entry;
        }

        private PositionAnalysisEntry ComputeTakeProfitAnalysis(ClosedPosition pos, PositionAnalysisEntry entry)
        {
            decimal tp = Math.Round(StartBalance(pos) / 50, 2);
            bool isSell = pos.Direction == TradeDirection.SELL;
            decimal pipVal = PipValue(pos.EntryPrice, pos.Volume);
            decimal tpPips = Math.Round((tp / pipVal), 1);
            decimal tpPipDecimal = tpPips * PIP_DIVISOR;

            entry.ExecutionPrice = isSell ? pos.EntryPrice - tpPipDecimal : pos.EntryPrice + tpPipDecimal;
            entry.ExecutionTime = DateTimeOffset.UtcNow;
            entry.ProfitInPercent = 2;
            entry.ProfitInPips = tpPips;
            entry.ProfitLoss = tp;
            entry.UsedSystem = "TwoPercent";

            return entry;
        }
    }
}
