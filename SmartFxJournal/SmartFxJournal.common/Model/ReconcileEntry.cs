using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFxJournal.Common.Model
{
    public class ReconcileEntry
    {
        public long PositionId { get; set; }

        public List<long> Orders { get; set; }
    }
}
