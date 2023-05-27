using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFxJournal.CTrader.helpers
{
    internal class OffsetIterator
    {
        private DateTimeOffset _start;
        private long _step = 604800000;
        private long _now = 0;
        private List<TimeStampRange> _ranges = new List<TimeStampRange>();

        public OffsetIterator(long startTime)
        {
            _start = DateTimeOffset.FromUnixTimeMilliseconds(startTime);
            _now = DateTimeOffset.Now.ToUnixTimeMilliseconds() ;

            for (long i = startTime; i <= _now; i += _step) {
                var end = i + _step;
                if ( end > _now )
                {
                    end = _now;
                }
                _ranges.Add(new TimeStampRange(i, end));
            }
        }

        public List<TimeStampRange> TimeStampRanges { get => _ranges; }

        internal class TimeStampRange
        {
            public TimeStampRange(long from, long to)
            {
                this.From = DateTimeOffset.FromUnixTimeMilliseconds(from);
                this.To = DateTimeOffset.FromUnixTimeMilliseconds(to);
            }

            public DateTimeOffset From { get; init; }
            public DateTimeOffset To { get; init; }
        }
    }
}
