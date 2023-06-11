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
    [Table("tradedpositions")]
    public class ClosedPosition
    {
        protected readonly decimal _lotSize = 10000000;

        [Key]
        [Column(TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long PositionId { get; set; }

        [Required]
        [Column(TypeName = "smallint")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PositionStatus PositionStatus { get; set; }

        [Required]
        public long AccountNo { get; set; }

        [Required]
        [JsonIgnore]
        public TradingAccount TradingAccount { get; set; } = null!;

        public List<ExecutedOrder> ExecutedOrders { get; set; } = new List<ExecutedOrder>();

        [Required]
        [Column(TypeName = "smallint")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Symbol Symbol { get; set; }

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TradeDirection Direction { get; set; }

        [Required]
        public long Volume { get; set; }

        [NotMapped]
        public decimal VolumeInLots
        {
            get
            {
                return Volume > 0 ? Volume / _lotSize : decimal.Zero;
            }
        }

        public bool IsMultiOrderPosition { get; set; } = false;

        [Required]
        [Column(TypeName = "decimal(10,5)")]
        public decimal EntryPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,5)")]
        public decimal ExitPrice { get; set; }

        [Column(TypeName = "decimal(10,5)")]
        public decimal StopLoss { get; set; }
        
        [Column(TypeName = "decimal(10,5)")]
        public decimal? TakeProfit { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Commission { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Swap { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal GrossProfit { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal NetProfit { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal BalanceAfter { get; set; }

        public DateTimeOffset LastUpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset OrderOpenedAt { get; set; }

        public DateTimeOffset OrderClosedAt { get; set; }

        internal static void OnModelCreate(ModelBuilder builder)
        {
            
        }
    }
}
