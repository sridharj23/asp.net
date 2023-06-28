 using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartFxJournal.JournalDB.valueconverters;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static SmartFxJournal.JournalDB.model.GlobalEnums;

namespace SmartFxJournal.JournalDB.model
{
    [Table("trading_accounts")]
    public class TradingAccount : Audited
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long AccountNo { get; set; }

        public long CTraderAccountId { get; set; } = 0;

        [Required]
        public bool IsDefault { get; set; } = false;

        [Required]
        public bool IsLive { get; set; } = false;

        [Required]
        public string Broker { get; set; } = null!;

        [Required]
        [Column(TypeName = "character varying(10)")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CurrencyType AccountCurrency { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal StartBalance { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal CurrentBalance { get; set; }

        [Required]
        public DateOnly OpenedOn { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        [Required]
        [Column(TypeName = "smallint")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ImportMode ImportMode { get; set; } = ImportMode.CSV;

        [Column(TypeName = "bigint")]
        public DateTimeOffset LastImportedOn { get; set; }

        public string? CTraderId { get; set; }

        public CTraderAccount? CTraderAccount { get; set; }

        public List<ClosedPosition> Positions { get; set; } = new List<ClosedPosition>();

        public List<ExecutedOrder> ExecutedOrders { get; set; } = new List<ExecutedOrder>();

        internal static void OnModelCreate(ModelBuilder builder)
        {
            builder.Entity<TradingAccount>()
                   .ToTable(t => t.HasCheckConstraint("chk_positive", "account_no > 0"));

            builder.Entity<TradingAccount>(entity =>
            {
                entity.HasMany(a => a.Positions)
                      .WithOne(p => p.TradingAccount)
                      .HasForeignKey(p => p.AccountNo)
                      .HasConstraintName("FK_parent_account")
                      .IsRequired();

                entity.HasMany(a => a.ExecutedOrders)
                      .WithOne(p => p.TradingAccount)
                      .HasForeignKey(p => p.AccountNo)
                      .HasConstraintName("FK_parent_account")
                      .IsRequired();

                entity.Property(e => e.AccountNo)
                .ValueGeneratedNever();

                entity
                .Property(p => p.AccountCurrency)
                .HasDefaultValueSql("'EUR'::character varying")
                .HasConversion<string>(new EnumToStringConverter<CurrencyType>());

                entity
                .Property(p => p.ImportMode)
                .HasDefaultValueSql("0")
                .HasConversion<short>();

                entity.Property(e => e.OpenedOn)
                .HasDefaultValueSql("now()");

                entity.Property(e => e.LastImportedOn)
                .HasConversion(new DateTimeOffsetToLongConverter());

                entity.Property(e => e.CreatedOn)
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("now()");

                entity.Property(e => e.LastModifiedOn)
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("now()");
            });
        }
    }
}
