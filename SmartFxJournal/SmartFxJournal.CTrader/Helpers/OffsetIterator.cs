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
        private long _diff = 0;
        private List<TimeStampRange> _ranges = new List<TimeStampRange>();

        public OffsetIterator(long startTime)
        {
            _start = DateTimeOffset.FromUnixTimeMilliseconds(startTime);
            _diff = (DateTimeOffset.Now - _start).Ticks;

            for (long i = startTime; i <= _diff; i += _step) {
                var end = i + _step;
                if ( end > DateTimeOffset.Now.ToUnixTimeMilliseconds() )
                {
                    end = DateTimeOffset.Now.ToUnixTimeMilliseconds();
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
