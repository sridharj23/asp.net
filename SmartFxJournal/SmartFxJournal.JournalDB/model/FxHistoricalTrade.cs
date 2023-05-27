using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartFxJournal.JournalDB.model.GlobalEnums;

namespace SmartFxJournal.JournalDB.model
{
    [Table("historytrades")]
    public class FxHistoricalTrade : FxOrderBase
    {

        [Key]
        [Column(TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long DealId { get; set; }

        [Column(TypeName = "bigint")]
        public long OrderId { get; set; }

        [Column(TypeName = "bigint")]
        public long PositionId { get; set; }

        public long FilledVolume { get; set; }

        public long ClosedVolume { get; set; }

        [Precision(10, 2)]
        public decimal BalanceAfterClose { get; set; }

        public DealStatus DealStatus { get; set; }

        public bool IsClosing { get; set; }

        internal static void OnModelCreate(ModelBuilder modelBuilder)
        {
        }
    }
}
