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
    [Table("executed_orders")]
    public class ExecutedOrder
    {
        [Key]
        [Column(TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long DealId { get; set; }

        [Column(TypeName = "bigint")]
        public long OrderId { get; set; }

        [Required]
        [Column(TypeName = "smallint")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DealStatus DealStatus { get; set; }

        [Required]
        public long AccountNo { get; set; }

        [Required]
        [JsonIgnore]
        public TradingAccount TradingAccount { get; set; } = null!;

        [Column(TypeName = "bigint")]
        public long PositionId { get; set; }

        [JsonIgnore]
        public ClosedPosition? Position { get; set; }

        [Required]
        [Column(TypeName = "smallint")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Symbol Symbol { get; set; }

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TradeDirection Direction { get; set; }

        public long FilledVolume { get; set; } = 0;

        public long ClosedVolume { get; set; } = 0;

        public bool IsClosing { get; set; }

        [Column(TypeName = "decimal(10,5)")]
        public decimal ExecutionPrice { get; set; }

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
        public decimal BalanceAfter { get; set; }

        public DateTimeOffset OrderExecutedAt { get; set; }

        public DateTimeOffset LastUpdatedAt { get; set; }

        public List<AnalysisEntry> AnalysisEntries { get; set; } = new List<AnalysisEntry>();

        internal static void OnModelCreate(ModelBuilder builder)
        {
            builder.Entity<ExecutedOrder>().HasAlternateKey(o => o.OrderId);

            builder.Entity<ExecutedOrder>(order => { 
                order.HasMany<AnalysisEntry>()
                     .WithOne()
                     .HasForeignKey(a => a.ParentId)
                     .HasPrincipalKey(o => o.OrderId)
                     .HasConstraintName("fk_Analysis_For_Orders")
                     .IsRequired();
            });
        }
    }
}
