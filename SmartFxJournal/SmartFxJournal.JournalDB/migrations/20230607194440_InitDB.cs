using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartFxJournal.JournalDB.migrations
{
    /// <inheritdoc />
    public partial class InitDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ctradermaster",
                columns: table => new
                {
                    c_trader_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    client_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    client_secret = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    auth_token = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    access_token = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    refresh_token = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    last_fetched_on = table.Column<long>(type: "bigint", nullable: true),
                    expires_on = table.Column<long>(type: "bigint", nullable: true),
                    created_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    last_modified_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ctradermaster", x => x.c_trader_id);
                });

            migrationBuilder.CreateTable(
                name: "tradingaccounts",
                columns: table => new
                {
                    account_no = table.Column<long>(type: "bigint", nullable: false),
                    c_trader_account_id = table.Column<long>(type: "bigint", nullable: true),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                    is_live = table.Column<bool>(type: "boolean", nullable: false),
                    broker = table.Column<string>(type: "text", nullable: false),
                    account_currency = table.Column<string>(type: "character varying(10)", nullable: false, defaultValueSql: "'EUR'::character varying"),
                    start_balance = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    current_balance = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    opened_on = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "now()"),
                    import_mode = table.Column<short>(type: "smallint", nullable: false, defaultValueSql: "0"),
                    last_imported_on = table.Column<long>(type: "bigint", nullable: false),
                    c_trader_id = table.Column<string>(type: "character varying(256)", nullable: true),
                    created_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    last_modified_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tradingaccounts", x => x.account_no);
                    table.CheckConstraint("chk_positive", "account_no > 0");
                    table.ForeignKey(
                        name: "fk_tradingaccounts_ctradermaster_c_trader_account_c_trader_id",
                        column: x => x.c_trader_id,
                        principalTable: "ctradermaster",
                        principalColumn: "c_trader_id");
                });

            migrationBuilder.CreateTable(
                name: "tradedpositions",
                columns: table => new
                {
                    position_id = table.Column<long>(type: "bigint", nullable: false),
                    position_status = table.Column<short>(type: "smallint", nullable: false),
                    account_no = table.Column<long>(type: "bigint", nullable: false),
                    symbol = table.Column<short>(type: "smallint", nullable: false),
                    direction = table.Column<int>(type: "integer", nullable: false),
                    volume = table.Column<long>(type: "bigint", nullable: false),
                    entry_price = table.Column<decimal>(type: "numeric(10,5)", nullable: false),
                    exit_price = table.Column<decimal>(type: "numeric(10,5)", nullable: false),
                    stop_loss = table.Column<decimal>(type: "numeric(10,5)", nullable: false),
                    take_profit = table.Column<decimal>(type: "numeric(10,5)", nullable: true),
                    commission = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    swap = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    gross_profit = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    net_profit = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    balance_after = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    last_updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    order_opened_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    order_closed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tradedpositions", x => x.position_id);
                    table.ForeignKey(
                        name: "fk_tradedpositions_tradingaccounts_trading_account_account_no",
                        column: x => x.account_no,
                        principalTable: "tradingaccounts",
                        principalColumn: "account_no",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "executedorders",
                columns: table => new
                {
                    deal_id = table.Column<long>(type: "bigint", nullable: false),
                    order_id = table.Column<long>(type: "bigint", nullable: false),
                    deal_status = table.Column<short>(type: "smallint", nullable: false),
                    account_no = table.Column<long>(type: "bigint", nullable: false),
                    position_id = table.Column<long>(type: "bigint", nullable: false),
                    symbol = table.Column<short>(type: "smallint", nullable: false),
                    direction = table.Column<int>(type: "integer", nullable: false),
                    filled_volume = table.Column<long>(type: "bigint", nullable: false),
                    closed_volume = table.Column<long>(type: "bigint", nullable: false),
                    is_closing = table.Column<bool>(type: "boolean", nullable: false),
                    execution_price = table.Column<decimal>(type: "numeric(10,5)", nullable: false),
                    commission = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    swap = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    gross_profit = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    balance_after = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    order_executed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_executedorders", x => x.deal_id);
                    table.ForeignKey(
                        name: "fk_executedorders_tradedpositions_position_id",
                        column: x => x.position_id,
                        principalTable: "tradedpositions",
                        principalColumn: "position_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_executedorders_tradingaccounts_trading_account_account_no",
                        column: x => x.account_no,
                        principalTable: "tradingaccounts",
                        principalColumn: "account_no",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ctradermaster_client_id",
                table: "ctradermaster",
                column: "client_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_executedorders_account_no",
                table: "executedorders",
                column: "account_no");

            migrationBuilder.CreateIndex(
                name: "ix_executedorders_position_id",
                table: "executedorders",
                column: "position_id");

            migrationBuilder.CreateIndex(
                name: "ix_tradedpositions_account_no",
                table: "tradedpositions",
                column: "account_no");

            migrationBuilder.CreateIndex(
                name: "ix_tradingaccounts_c_trader_id",
                table: "tradingaccounts",
                column: "c_trader_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "executedorders");

            migrationBuilder.DropTable(
                name: "tradedpositions");

            migrationBuilder.DropTable(
                name: "tradingaccounts");

            migrationBuilder.DropTable(
                name: "ctradermaster");
        }
    }
}
