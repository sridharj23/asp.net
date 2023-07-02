using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFxJournal.Common.Analyzers
{
    internal class AnalyzerFactory
    {
        private static Dictionary<string, Func<IAnalyzer>> _analyzers = new();

        static AnalyzerFactory() {
            _analyzers.Add("1to1PercentPL", OneToOneProfitLossAnalyzer.CreateInstance );
        }

        public static IAnalyzer GetAnalyzer(string strategy)
        {
            return _analyzers[strategy]();
        }
    }
}
