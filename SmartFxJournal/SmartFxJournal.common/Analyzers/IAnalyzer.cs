using SmartFxJournal.JournalDB.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFxJournal.Common.Analyzers
{
    internal interface IAnalyzer
    {
        public ClosedPosition Analyze(ClosedPosition position);
    }
}
