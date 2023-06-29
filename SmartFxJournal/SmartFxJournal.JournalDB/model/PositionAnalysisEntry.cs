using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static SmartFxJournal.JournalDB.model.GlobalEnums;

namespace SmartFxJournal.JournalDB.model
{
    [Table("analysis_for_positions")]
    public class PositionAnalysisEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long EntryId { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long PositionId { get; set; }

        [JsonIgnore]
        public ClosedPosition? Position { get; set; }

        [Required]
        [Column(TypeName = "character varying(50)")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AnalyzedAspect AnalyzedAspect { get; set; }

        [Required]
        [Column(TypeName = "character varying(50)")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AnalysisScenario AnalysisScenario { get; set; }

        public bool ValidTrade { get; set; } = true;

        public List<string> ReasonToTrade { get; set; } = new List<string>();

        public bool BettterAvoided { get; set; } = false;

        public List<string> ReasonToAvoid { get; set; } = new List<string>();

        [Required]
        [Column(TypeName = "character varying(100)")]
        public string UsedSystem { get; set; } = null!;

        [Required]
        public List<string> UsedStrategy { get; set; } = new List<string>();

        [Required]
        [Column(TypeName = "character varying(100)")]
        public string UsedIndicator { get; set; } = null!;

        [Required]
        public List<string> IndicatorStatus { get; set; } = new List<string>();

        public List<string> ExecutionAccuracy { get; set; } = new List<string>();

        [Column(TypeName = "decimal(10,5)")]
        public decimal ExecutionPrice { get; set; }

        public DateTimeOffset ExecutionTime { get; set; }

        public long Volume { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal ProfitLoss { get; set; } = decimal.Zero;

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal ProfitInPercent { get; set; } = decimal.Zero;

        [Required]
        [Column(TypeName = "decimal(6,2)")]
        public decimal ProfitInPips { get; set; } = Decimal.Zero;

        public AnalysisJournalEntry? Notes { get; set; }

        internal static void OnModelCreate(ModelBuilder builder)
        {
            builder.Entity<PositionAnalysisEntry>(e =>
            {
                e.Property(p => p.AnalysisScenario).HasConversion<string>(new EnumToStringConverter<AnalysisScenario>());
                e.Property(p => p.AnalyzedAspect).HasConversion<string>(new EnumToStringConverter<AnalyzedAspect>());

                builder.Entity<PositionAnalysisEntry>()
                       .HasOne(p => p.Notes)
                       .WithOne(j => j.AnalysisEntry)
                       .HasForeignKey<AnalysisJournalEntry>(j => j.ParentId)
                       .HasConstraintName("FK_analysis_journal")
                       .IsRequired();
            });
        }
    }
}
