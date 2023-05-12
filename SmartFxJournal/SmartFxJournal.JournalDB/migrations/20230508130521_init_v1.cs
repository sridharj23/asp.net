using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartFxJournal.JournalDB.migrations
{
    /// <inheritdoc />
    public partial class init_v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    account_no = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    account_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'Demo'::character varying"),
                    is_default = table.Column<bool>(type: "boolean", nullable: true),
                    nick_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    broker_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    start_balance = table.Column<decimal>(type: "numeric", nullable: false),
                    current_balance = table.Column<decimal>(type: "numeric", nullable: true),
                    currency_type = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValueSql: "'EUR'::character varying"),
                    opened_on = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "now()"),
                    last_imported_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    import_mode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValueSql: "'CSV'::character varying"),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_modified_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accounts", x => x.account_no);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accounts");
        }
    }
}
