using Microsoft.EntityFrameworkCore;
using SmartFxJournal.JournalDB.valueconverters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFxJournal.JournalDB.model
{
    [Table("ctraderaccounts")]
    public sealed class CTraderAccount : Audited
    {
        private string? _authToken;

        [Key]
        [MaxLength(256)]
        public string CTraderId { get; set; } = null!;

        [Required]
        [MaxLength(256)]
        public string ClientId { get; set; } = null!;

        [Required]
        [MaxLength(256)]
        public string ClientSecret { get; set; } = null!;

        [MaxLength(1000)]
        public string? AuthToken 
        {
            get { return _authToken ?? string.Empty; }
            set
            {
                _authToken = value;
                LastFetchedOn = DateTime.UtcNow.Subtract(new TimeSpan(0, 0, 100)); // Set the fetch time a bit in the past for timely refresh.
            }
        }

        [MaxLength(256)]
        public string? AccessToken { get; set; }

        [MaxLength(256)]
        public string? RefreshToken { get; set; }

        public DateTimeOffset? LastFetchedOn { get; set; }

        public DateTimeOffset? ExpiresOn { get; set; }

        public List<FxAccount> FxAccounts { get; set; } = new List<FxAccount>();

        internal static void OnModelCreate(ModelBuilder builder)
        {
            builder.Entity<CTraderAccount>(entity =>
            {
                entity.HasIndex(x => x.ClientId).IsUnique();
                entity.HasMany(x => x.FxAccounts).WithOne(a => a.Parent).HasForeignKey(a => a.CTraderId);
                entity.Property(x => x.LastFetchedOn).HasConversion(new DateTimeOffsetToLongConverter());
                entity.Property(x => x.ExpiresOn).HasConversion(new DateTimeOffsetToLongConverter());

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
