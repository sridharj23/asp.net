﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SmartFxJournal.JournalDB.Migrations
{
    /// <inheritdoc />
    public partial class InitDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ctrader_master",
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
                    table.PrimaryKey("pk_ctrader_master", x => x.c_trader_id);
                });

            migrationBuilder.CreateTable(
                name: "trading_accounts",
                columns: table => new
                {
                    account_no = table.Column<long>(type: "bigint", nullable: false),
                    c_trader_account_id = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("pk_trading_accounts", x => x.account_no);
                    table.CheckConstraint("chk_positive", "account_no > 0");
                    table.ForeignKey(
                        name: "FK_ctrader_parent",
                        column: x => x.c_trader_id,
                        principalTable: "ctrader_master",
                        principalColumn: "c_trader_id");
                });

            migrationBuilder.CreateTable(
                name: "traded_positions",
                columns: table => new
                {
                    position_id = table.Column<long>(type: "bigint", nullable: false),
                    position_status = table.Column<short>(type: "smallint", nullable: false),
                    account_no = table.Column<long>(type: "bigint", nullable: false),
                    symbol = table.Column<short>(type: "smallint", nullable: false),
                    direction = table.Column<int>(type: "integer", nullable: false),
                    volume = table.Column<long>(type: "bigint", nullable: false),
                    is_multi_order_position = table.Column<bool>(type: "boolean", nullable: false),
                    entry_price = table.Column<decimal>(type: "numeric(10,5)", nullable: false),
                    exit_price = table.Column<decimal>(type: "numeric(10,5)", nullable: false),
                    commission = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    swap = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    gross_profit = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    net_profit = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    balance_after = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    last_updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    order_opened_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    order_closed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    analysis_status = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_traded_positions", x => x.position_id);
                    table.ForeignKey(
                        name: "FK_parent_account",
                        column: x => x.account_no,
                        principalTable: "trading_accounts",
                        principalColumn: "account_no",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "analysis_for_positions",
                columns: table => new
                {
                    entry_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    position_id = table.Column<long>(type: "bigint", nullable: false),
                    analyzed_aspect = table.Column<string>(type: "character varying(50)", nullable: false),
                    analysis_scenario = table.Column<string>(type: "character varying(50)", nullable: false),
                    valid_trade = table.Column<bool>(type: "boolean", nullable: false),
                    reason_to_trade = table.Column<List<string>>(type: "text[]", nullable: false),
                    bettter_avoided = table.Column<bool>(type: "boolean", nullable: false),
                    reason_to_avoid = table.Column<List<string>>(type: "text[]", nullable: false),
                    used_system = table.Column<string>(type: "character varying(100)", nullable: false),
                    used_strategy = table.Column<List<string>>(type: "text[]", nullable: false),
                    used_indicator = table.Column<string>(type: "character varying(100)", nullable: false),
                    indicator_status = table.Column<List<string>>(type: "text[]", nullable: false),
                    execution_accuracy = table.Column<List<string>>(type: "text[]", nullable: false),
                    execution_price = table.Column<decimal>(type: "numeric(10,5)", nullable: false),
                    execution_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    volume = table.Column<long>(type: "bigint", nullable: false),
                    profit_loss = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    profit_in_percent = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    profit_in_pips = table.Column<decimal>(type: "numeric(6,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_analysis_for_positions", x => x.entry_id);
                    table.ForeignKey(
                        name: "FK_analyzed_position",
                        column: x => x.position_id,
                        principalTable: "traded_positions",
                        principalColumn: "position_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "executed_orders",
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
                    table.PrimaryKey("pk_executed_orders", x => x.deal_id);
                    table.UniqueConstraint("ak_executed_orders_order_id", x => x.order_id);
                    table.ForeignKey(
                        name: "FK_parent_account",
                        column: x => x.account_no,
                        principalTable: "trading_accounts",
                        principalColumn: "account_no",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_parent_position",
                        column: x => x.position_id,
                        principalTable: "traded_positions",
                        principalColumn: "position_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "journals_for_positions",
                columns: table => new
                {
                    journal_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    last_modified_on = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    parent_id = table.Column<long>(type: "bigint", nullable: false),
                    is_complete = table.Column<bool>(type: "boolean", nullable: false),
                    journal_text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_journals_for_positions", x => x.journal_id);
                    table.ForeignKey(
                        name: "FK_parent_position",
                        column: x => x.parent_id,
                        principalTable: "traded_positions",
                        principalColumn: "position_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "journals_for_analysis",
                columns: table => new
                {
                    journal_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_modified_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    parent_id = table.Column<long>(type: "bigint", nullable: false),
                    is_complete = table.Column<bool>(type: "boolean", nullable: false),
                    journal_text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_journals_for_analysis", x => x.journal_id);
                    table.ForeignKey(
                        name: "FK_analysis_journal",
                        column: x => x.parent_id,
                        principalTable: "analysis_for_positions",
                        principalColumn: "entry_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_analysis_for_positions_position_id",
                table: "analysis_for_positions",
                column: "position_id");

            migrationBuilder.CreateIndex(
                name: "ix_ctrader_master_client_id",
                table: "ctrader_master",
                column: "client_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_executed_orders_account_no",
                table: "executed_orders",
                column: "account_no");

            migrationBuilder.CreateIndex(
                name: "ix_executed_orders_position_id",
                table: "executed_orders",
                column: "position_id");

            migrationBuilder.CreateIndex(
                name: "ix_journals_for_analysis_parent_id",
                table: "journals_for_analysis",
                column: "parent_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_journals_for_positions_parent_id",
                table: "journals_for_positions",
                column: "parent_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_traded_positions_account_no",
                table: "traded_positions",
                column: "account_no");

            migrationBuilder.CreateIndex(
                name: "ix_trading_accounts_c_trader_id",
                table: "trading_accounts",
                column: "c_trader_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "executed_orders");

            migrationBuilder.DropTable(
                name: "journals_for_analysis");

            migrationBuilder.DropTable(
                name: "journals_for_positions");

            migrationBuilder.DropTable(
                name: "analysis_for_positions");

            migrationBuilder.DropTable(
                name: "traded_positions");

            migrationBuilder.DropTable(
                name: "trading_accounts");

            migrationBuilder.DropTable(
                name: "ctrader_master");
        }
    }
}
