using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFxJournal.JournalDB.model
{
    [NotMapped]
    public class JournalEntry : Audited
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long JournalId { get; set; }

        [Required]
        public long ParentId { get; set; }

        [Required]
        public bool IsComplete { get; set; } = false;

        [Required]
        public string JournalText { get; set; } = string.Empty;
    }
}
