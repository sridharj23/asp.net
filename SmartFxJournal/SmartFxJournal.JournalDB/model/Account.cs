using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartFxJournal.JournalDB.model
{
    [Table("accounts")]
    public partial class Account : Audited
    {
        public Account(String accountNo) { 
            AccountNo = accountNo;
        }

        [Key]
        [MaxLength(20)]
        public string AccountNo { get; private set; } = null!;

        [Required]
        [MaxLength(20)]
        public string AccountType { get; set; } = null!;

        public bool? IsDefault { get; set; }

        [MaxLength(100)]
        public string? NickName { get; set; }

        [MaxLength(100)]
        public string? BrokerName { get; set; }

        [Required]
        public decimal StartBalance { get; set; }

        public decimal? CurrentBalance { get; set; }

        [Required]
        [MaxLength(3)]
        public string CurrencyType { get; set; } = null!;

        public DateOnly? OpenedOn { get; set; }

        public DateTime? LastImportedOn { get; set; }

        [Required]
        [MaxLength(10)]
        public string ImportMode { get; set; } = null!;

        // Initialize the table properties
        internal static void OnModelCreate(ModelBuilder builder)
        {
            builder.Entity<Account>(entity =>
            {
                entity
                .Property(p => p.AccountType)
                .HasDefaultValueSql("'Demo'::character varying");
                //.HasConversion(new EnumToStringConverter<AccountTypeEnum>());

                entity
                .Property(p => p.CurrencyType)
                .HasDefaultValueSql("'EUR'::character varying");
                //.HasConversion<string>();

                entity
                .Property(p => p.ImportMode)
                .HasDefaultValueSql("'CSV'::character varying");
                //.HasConversion(new EnumToStringConverter<ImportModeEnum>());

                entity.Property(e => e.OpenedOn)
                .HasDefaultValueSql("now()");

                entity.Property(e => e.LastImportedOn)
                .HasColumnType("timestamp without time zone");
            });
        }
    }

    public enum AccountTypeEnum
    {
        Demo,
        Live
    }

    public enum CurrencyTypeEnum
    {
        EUR,
        USD
    }

    public enum ImportModeEnum
    {
        CSV,
        cTrader
    }

}
