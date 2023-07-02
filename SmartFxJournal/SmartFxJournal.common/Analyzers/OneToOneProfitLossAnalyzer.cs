using SmartFxJournal.JournalDB.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartFxJournal.JournalDB.model.GlobalEnums;

namespace SmartFxJournal.Common.Analyzers
{
    internal class OneToOneProfitLossAnalyzer : IAnalyzer
    {

        public static OneToOneProfitLossAnalyzer CreateInstance() 
        { 
            return new OneToOneProfitLossAnalyzer(); 
        }

        private decimal runningBalance = default;

        public ClosedPosition Analyze(ClosedPosition position)
        {
            decimal balanceBeforePL = runningBalance == default ? (position.BalanceAfter - position.NetProfit) : runningBalance;
            decimal onePercent = Math.Round( balanceBeforePL / 100, 2);
            PositionAnalysisEntry? entry = GetEntry(position.AnalysisEntries, AnalysisScenario.Actual, AnalyzedAspect.MaxProfit);

            if (position.GrossProfit > onePercent) 
            {
                // Actual profit was more than 1%
                position.NetProfit = onePercent;
                position.BalanceAfter = balanceBeforePL + onePercent;
            } else if (entry != default && entry.ProfitLoss > onePercent) {
                // Max profit was greater than 1%
                // Actual profit was more than 1%
                position.NetProfit = onePercent;
                position.BalanceAfter = balanceBeforePL + onePercent;
            } else {
                // No max profit analysis. So 1% loss
                position.NetProfit = -1 * onePercent;
                position.BalanceAfter = balanceBeforePL - onePercent;

            }

            runningBalance = position.BalanceAfter;
            return position;
        }

        private PositionAnalysisEntry? GetEntry(List<PositionAnalysisEntry> entries, AnalysisScenario scenario, AnalyzedAspect aspect) 
        {
            PositionAnalysisEntry? entry = default;

            entries.ForEach(e =>
            {
                if (e.AnalysisScenario == scenario && e.AnalyzedAspect == aspect)
                {
                    entry = e;
                    return;
                }
            });

            return entry;
        }
    }
}
