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
    [Table("analysis_entries")]
    public class AnalysisEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long EntryId { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ParentId { get; set; }

        [Required]
        [Column(TypeName = "character varying(50)")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ArtifactType ParentType { get; set; }

        [Required]
        [Column(TypeName = "character varying(50)")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AnalyzedAspect AnalyzedAspect { get; set; }

        [Required]
        [Column(TypeName = "character varying(50)")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AnalysisScenario AnalysisScenario { get; set; }

        public bool IsValid { get; set; }

        public List<string> InvalidityReason { get; set; } = new List<string>();

        [Required]
        [Column(TypeName = "character varying(100)")]
        public string UsedSystem { get; set; } = null!;

        [Required]
        public List<string> UsedStrategy { get; set; } = new List<string>();

        [Required]
        [Column(TypeName = "character varying(100)")]
        public string UsedInidicator { get; set; } = null!;

        [Required]
        public List<string> IndicatorStatus { get; set; } = new List<string>();

        public List<string> ExecutionAccuracy { get; set; } = new List<string>();

        [Column(TypeName = "decimal(10,5)")]
        public decimal ExecutionPrice { get; set; }

        public DateTimeOffset ExecutionTime { get; set; }

        public long Volume { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal ProfitLoss { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal ProfitInPercent { get; set; }

        [Required]
        [Column(TypeName = "decimal(6,2)")]
        public decimal ProfitInPips { get; set; }

        internal static void OnModelCreate(ModelBuilder builder)
        {
            builder.Entity<AnalysisEntry>(e =>
            {
                e.Property(p => p.ParentType).HasConversion<string>(new EnumToStringConverter<ArtifactType>());
                e.Property(p => p.AnalysisScenario).HasConversion<string>(new EnumToStringConverter<AnalysisScenario>());
                e.Property(p => p.AnalyzedAspect).HasConversion<string>(new EnumToStringConverter<AnalyzedAspect>());
            });
        }
    }
}
