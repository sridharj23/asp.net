using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFxJournal.JournalDB.valueconverters
{
    public class DateTimeOffsetToLongConverter : ValueConverter<DateTimeOffset, long>
    {
        public DateTimeOffsetToLongConverter() : base(
            v => v.ToUnixTimeMilliseconds(),
            v => DateTimeOffset.FromUnixTimeMilliseconds(v))
        {
        }
    }
}
