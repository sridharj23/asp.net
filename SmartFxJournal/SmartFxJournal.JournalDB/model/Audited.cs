using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFxJournal.JournalDB.model
{
    [NotMapped]
    public abstract class Audited
    {
        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public DateTime LastModifiedOn { get; set; }

        internal static void OnModelCreate(ModelBuilder builder)
        {
            builder.Entity<Account>(entity =>
            {
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
