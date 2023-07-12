using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFxJournal.Common.Aggregators
{
    internal class AggregatorFactory
    {
        private static Dictionary<string, Func<IAggregator>> aggregators = new();
        public static Dictionary<string, IAggregator> GetAllAggregators ()
        {
            Dictionary<string, IAggregator> agg = new();
            aggregators.Keys.ToList().ForEach(k => agg.Add(k, aggregators[k]() ));
            return agg;
        }

        public static List<IAggregator> GetAggregators(string analysisType)
        {
            return new List<IAggregator> ()
            {
                aggregators[analysisType]()
            };
        }

        static AggregatorFactory()
        {
            aggregators.Add("Totals (All Trades)", TotalsAggregator.Create);
            aggregators.Add("Averages (All Trades)", AveragesAggregator.Create);
            aggregators.Add("Totals (Significant Trades)", TotalsAggregator.Create2);
            aggregators.Add("Averages (Significant Trades)", AveragesAggregator.Create2);

        }
    }
}
