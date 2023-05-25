using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static SmartFxJournal.JournalDB.model.GlobalEnums;

namespace SmartFxJournal.JournalDB.model
{
    [Table("fxpositions")]
    public class FxPosition : FxOrderBase
    {

        [Key]
        [Column(TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long PositionId { get; set; }

        [Required]
        [Column(TypeName = "smallint")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PositionStatus PositionStatus { get; set; }

        public bool? IsGuaranteedSL { get; set; }

        public bool IsTrailingStopLoss { get; set; } = false;

        [Column(TypeName = "decimal(10,2)")]
        public decimal UsedMargin { get; set; } = decimal.Zero;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Tax { get; set; } = decimal.Zero;

        [Column(TypeName = "decimal(10,5)")]
        public decimal? StopLoss { get; set; }

        [Column(TypeName = "decimal(10,5)")]
        public decimal? TakeProfit { get; set; }

        internal static void OnModelCreate(ModelBuilder builder)
        {
        }
    }
}
