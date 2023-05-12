using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartFxJournal.JournalDB.migrations
{
    /// <inheritdoc />
    public partial class AddTableCTraderAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "c_trader_account_c_trader_id",
                table: "accounts",
                type: "character varying(256)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ctraderaccounts",
                columns: table => new
                {
                    c_trader_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    client_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    client_secret = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    auth_token = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    refresh_token = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    last_fetched_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ctraderaccounts", x => x.c_trader_id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_accounts_c_trader_account_c_trader_id",
                table: "accounts",
                column: "c_trader_account_c_trader_id");

            migrationBuilder.CreateIndex(
                name: "ix_ctraderaccounts_client_id",
                table: "ctraderaccounts",
                column: "client_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_accounts_ctraderaccounts_c_trader_account_c_trader_id",
                table: "accounts",
                column: "c_trader_account_c_trader_id",
                principalTable: "ctraderaccounts",
                principalColumn: "c_trader_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_accounts_ctraderaccounts_c_trader_account_c_trader_id",
                table: "accounts");

            migrationBuilder.DropTable(
                name: "ctraderaccounts");

            migrationBuilder.DropIndex(
                name: "ix_accounts_c_trader_account_c_trader_id",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "c_trader_account_c_trader_id",
                table: "accounts");
        }
    }
}
