 using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartFxJournal.JournalDB.valueconverters;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static SmartFxJournal.JournalDB.model.GlobalEnums;

namespace SmartFxJournal.JournalDB.model
{
    [Table("accounts")]
    public class FxAccount : Audited
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long AccountNo { get; set; }

        public long? CTraderAccountId { get; set; }

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

        public CTraderAccount? Parent { get; set; }

        public List<FxPosition> Positions { get; set; } = new List<FxPosition>();

        internal static void OnModelCreate(ModelBuilder builder)
        {
            builder.Entity<FxAccount>()
                   .ToTable(t => t.HasCheckConstraint("chk_positive", "account_no > 0"));

            builder.Entity<FxAccount>(entity =>
            {
                entity.HasMany(a => a.Positions).WithOne(p => p.Owner).HasForeignKey(p => p.AccountNo).IsRequired();

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
