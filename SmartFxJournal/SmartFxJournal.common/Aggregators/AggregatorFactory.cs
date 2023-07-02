using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFxJournal.Common.Aggregators
{
    internal class AggregatorFactory
    {
        public static List<IAggregator> Aggregators { get; } = new List<IAggregator>();

        static AggregatorFactory()
        {

        } 
    }
}
