using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartFxJournal.Common.Aggregators
{
    internal class AggregateItem
    {
        public string Category { get; set; } = string.Empty;

        public string Key { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DataType DataType { get; set; }
    }

    internal enum DataType
    {
        String = 750,
        Number = 751,
        Decimal = 752,
        Boolean = 753
    }
}
