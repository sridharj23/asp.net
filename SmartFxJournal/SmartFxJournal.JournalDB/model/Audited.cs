using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFxJournal.JournalDB.model
{
    [NotMapped]
    public abstract class Audited
    {
        public DateTime? CreatedOn { get; set; }

        public DateTime? LastModifiedOn { get; set; }
    }
}
