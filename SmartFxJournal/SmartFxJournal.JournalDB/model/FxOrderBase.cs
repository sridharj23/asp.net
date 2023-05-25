using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static SmartFxJournal.JournalDB.model.GlobalEnums;

namespace SmartFxJournal.JournalDB.model
{
    [NotMapped]
    public class FxOrderBase
    {
        protected readonly decimal _lotSize = 10000000;

        [Required]
        public long AccountNo { get; set; }

        [Required]
        public FxAccount Owner { get; set; } = null!;

        [Required]
        [MaxLength(10)]
        public string SymbolName { get; set; } = null!;

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TradeDirection Direction { get; set; }

        [Required]
        public long Volume { get; set; }

        [NotMapped]
        public decimal VolumeInLots { 
            get
            {
                return Volume > 0 ? Volume / _lotSize : Decimal.Zero;
            }
         }

        [Column(TypeName = "decimal(10,5)")]
        public decimal? ExecutionPrice { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Commission { get; set; } = decimal.Zero;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Swap { get; set; } = decimal.Zero;

        public DateTimeOffset? LastUpdatedAt { get; set; }

        public DateTimeOffset? OrderOpenedAt { get; set; }

        [MaxLength(256)]
        public string? Comment { get; set; }

        [MaxLength(256)]
        public string? Label { get; set; }
    }
}
