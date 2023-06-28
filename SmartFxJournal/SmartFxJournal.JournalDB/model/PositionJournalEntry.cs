using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartFxJournal.JournalDB.model
{
    [Table("journals_for_positions")]
    public class PositionJournalEntry : JournalEntry
    {
        [JsonIgnore]
        public ClosedPosition Position { get; set; } = null!;

        internal static void OnModelCreate(ModelBuilder builder)
        {
            builder.Entity<PositionJournalEntry>(entity =>
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
