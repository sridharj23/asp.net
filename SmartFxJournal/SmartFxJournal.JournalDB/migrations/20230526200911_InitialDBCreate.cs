using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartFxJournal.JournalDB.migrations
{
    /// <inheritdoc />
    public partial class InitialDBCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ctraderaccounts",
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
                    table.PrimaryKey("pk_ctraderaccounts", x => x.c_trader_id);
                });

            migrationBuilder.CreateTable(
                name: "accounts",
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
                    table.PrimaryKey("pk_accounts", x => x.account_no);
                    table.CheckConstraint("chk_positive", "account_no > 0");
                    table.ForeignKey(
                        name: "fk_accounts_ctraderaccounts_parent_c_trader_id",
                        column: x => x.c_trader_id,
                        principalTable: "ctraderaccounts",
                        principalColumn: "c_trader_id");
                });

            migrationBuilder.CreateTable(
                name: "fxpositions",
                columns: table => new
                {
                    position_id = table.Column<long>(type: "bigint", nullable: false),
                    position_status = table.Column<short>(type: "smallint", nullable: false),
                    is_guaranteed_sl = table.Column<bool>(type: "boolean", nullable: true),
                    is_trailing_stop_loss = table.Column<bool>(type: "boolean", nullable: false),
                    used_margin = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    tax = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    stop_loss = table.Column<decimal>(type: "numeric(10,5)", nullable: true),
                    take_profit = table.Column<decimal>(type: "numeric(10,5)", nullable: true),
                    account_no = table.Column<long>(type: "bigint", nullable: false),
                    symbol = table.Column<short>(type: "smallint", nullable: false),
                    direction = table.Column<int>(type: "integer", nullable: false),
                    volume = table.Column<long>(type: "bigint", nullable: false),
                    execution_price = table.Column<decimal>(type: "numeric(10,5)", nullable: false),
                    commission = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    swap = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    gross_profit = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    last_updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    order_opened_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    comment = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    label = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fxpositions", x => x.position_id);
                    table.ForeignKey(
                        name: "fk_fxpositions_accounts_owner_account_no",
                        column: x => x.account_no,
                        principalTable: "accounts",
                        principalColumn: "account_no",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "historytrades",
                columns: table => new
                {
                    deal_id = table.Column<long>(type: "bigint", nullable: false),
                    order_id = table.Column<long>(type: "bigint", nullable: false),
                    position_id = table.Column<long>(type: "bigint", nullable: false),
                    filled_volume = table.Column<long>(type: "bigint", nullable: false),
                    closed_volume = table.Column<long>(type: "bigint", nullable: false),
                    balance_after_close = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    deal_status = table.Column<int>(type: "integer", nullable: false),
                    is_closing = table.Column<bool>(type: "boolean", nullable: false),
                    account_no = table.Column<long>(type: "bigint", nullable: false),
                    symbol = table.Column<short>(type: "smallint", nullable: false),
                    direction = table.Column<int>(type: "integer", nullable: false),
                    volume = table.Column<long>(type: "bigint", nullable: false),
                    execution_price = table.Column<decimal>(type: "numeric(10,5)", nullable: false),
                    commission = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    swap = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    gross_profit = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    last_updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    order_opened_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    comment = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    label = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_historytrades", x => x.deal_id);
                    table.ForeignKey(
                        name: "fk_historytrades_accounts_owner_account_no",
                        column: x => x.account_no,
                        principalTable: "accounts",
                        principalColumn: "account_no",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_accounts_c_trader_id",
                table: "accounts",
                column: "c_trader_id");

            migrationBuilder.CreateIndex(
                name: "ix_ctraderaccounts_client_id",
                table: "ctraderaccounts",
                column: "client_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_fxpositions_account_no",
                table: "fxpositions",
                column: "account_no");

            migrationBuilder.CreateIndex(
                name: "ix_historytrades_account_no",
                table: "historytrades",
                column: "account_no");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fxpositions");

            migrationBuilder.DropTable(
                name: "historytrades");

            migrationBuilder.DropTable(
                name: "accounts");

            migrationBuilder.DropTable(
                name: "ctraderaccounts");
        }
    }
}
