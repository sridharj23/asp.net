using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartFxJournal.JournalDB.migrations
{
    /// <inheritdoc />
    public partial class AlterAnalysisEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "used_inidicator",
                table: "analysis_entries",
                newName: "used_indicator");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "used_indicator",
                table: "analysis_entries",
                newName: "used_inidicator");
        }
    }
}
