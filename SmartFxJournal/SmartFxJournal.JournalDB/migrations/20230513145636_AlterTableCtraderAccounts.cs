using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartFxJournal.JournalDB.migrations
{
    /// <inheritdoc />
    public partial class AlterTableCtraderAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "access_token",
                table: "ctraderaccounts",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "expires_in",
                table: "ctraderaccounts",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "access_token",
                table: "ctraderaccounts");

            migrationBuilder.DropColumn(
                name: "expires_in",
                table: "ctraderaccounts");
        }
    }
}
