using SmartFxJournal.JournalDB.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFxJournal.Common.Aggregators
{
    internal interface IAggregator
    {
        public void Aggregate(ClosedPosition position);

        public List<AggregateItem> AggregateItems { get; }
    }
}
