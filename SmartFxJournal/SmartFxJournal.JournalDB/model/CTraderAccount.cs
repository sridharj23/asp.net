using Microsoft.EntityFrameworkCore;
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

        public DateTime? LastFetchedOn { get; set; }

        public long? ExpiresIn { get; set; }

        public List<Account> Accounts { get; set; } = new List<Account>();

        internal static void OnModelCreate(ModelBuilder builder)
        {
            Audited.OnModelCreate(builder);

            builder.Entity<CTraderAccount>().HasIndex(x => x.ClientId).IsUnique();

            builder.Entity<CTraderAccount>()
                    .HasMany(x => x.Accounts);
        }
    }
}
