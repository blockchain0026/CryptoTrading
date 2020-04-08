using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoTrading.Services.ExchangeAccess.API.Migrations
{
    public partial class InitialCraeted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "exchangeaccess");

            migrationBuilder.CreateSequence(
                name: "assetseq",
                schema: "exchangeaccess",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "balanceseq",
                schema: "exchangeaccess",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "candlechartseq",
                schema: "exchangeaccess",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "candleseq",
                schema: "exchangeaccess",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "marketaskseq",
                schema: "exchangeaccess",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "marketbidseq",
                schema: "exchangeaccess",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "marketseq",
                schema: "exchangeaccess",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "balances",
                schema: "exchangeaccess",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    BalanceId = table.Column<string>(nullable: false),
                    ExchangeId = table.Column<int>(nullable: false),
                    Account_Username = table.Column<string>(nullable: true),
                    Account_ApiKey = table.Column<string>(nullable: true),
                    Account_ApiSecret = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_balances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "candleperiod",
                schema: "exchangeaccess",
                columns: table => new
                {
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Id = table.Column<int>(nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_candleperiod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "markets",
                schema: "exchangeaccess",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    MarketId = table.Column<string>(maxLength: 200, nullable: false),
                    Exchange_ExchangeId = table.Column<int>(nullable: false),
                    Exchange_ExchangeName = table.Column<string>(nullable: true),
                    BaseCurrency = table.Column<string>(nullable: false),
                    QuoteCurrency = table.Column<string>(nullable: false),
                    OrderSizeLimit = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_markets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "assets",
                schema: "exchangeaccess",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Symbol = table.Column<string>(nullable: false),
                    Available = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    Locked = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    BalanceId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_assets_balances_BalanceId",
                        column: x => x.BalanceId,
                        principalSchema: "exchangeaccess",
                        principalTable: "balances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "candlecharts",
                schema: "exchangeaccess",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    CandleChartId = table.Column<string>(maxLength: 200, nullable: false),
                    MarketId = table.Column<string>(nullable: false),
                    CandlePeriodId = table.Column<int>(nullable: false),
                    BaseCurrency = table.Column<string>(nullable: false),
                    QuoteCurrency = table.Column<string>(nullable: false),
                    ExchangeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_candlecharts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_candlecharts_candleperiod_CandlePeriodId",
                        column: x => x.CandlePeriodId,
                        principalSchema: "exchangeaccess",
                        principalTable: "candleperiod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "marketasks",
                schema: "exchangeaccess",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    MarketId = table.Column<string>(nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    MarketId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_marketasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_marketasks_markets_MarketId1",
                        column: x => x.MarketId1,
                        principalSchema: "exchangeaccess",
                        principalTable: "markets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "marketbids",
                schema: "exchangeaccess",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    MarketId = table.Column<string>(nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    MarketId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_marketbids", x => x.Id);
                    table.ForeignKey(
                        name: "FK_marketbids_markets_MarketId1",
                        column: x => x.MarketId1,
                        principalSchema: "exchangeaccess",
                        principalTable: "markets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "candles",
                schema: "exchangeaccess",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    High = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    Low = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    Open = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    Close = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    CandleChartId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_candles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_candles_candlecharts_CandleChartId",
                        column: x => x.CandleChartId,
                        principalSchema: "exchangeaccess",
                        principalTable: "candlecharts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_assets_BalanceId",
                schema: "exchangeaccess",
                table: "assets",
                column: "BalanceId");

            migrationBuilder.CreateIndex(
                name: "IX_candlecharts_CandleChartId",
                schema: "exchangeaccess",
                table: "candlecharts",
                column: "CandleChartId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_candlecharts_CandlePeriodId",
                schema: "exchangeaccess",
                table: "candlecharts",
                column: "CandlePeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_candles_CandleChartId",
                schema: "exchangeaccess",
                table: "candles",
                column: "CandleChartId");

            migrationBuilder.CreateIndex(
                name: "IX_marketasks_MarketId1",
                schema: "exchangeaccess",
                table: "marketasks",
                column: "MarketId1");

            migrationBuilder.CreateIndex(
                name: "IX_marketbids_MarketId1",
                schema: "exchangeaccess",
                table: "marketbids",
                column: "MarketId1");

            migrationBuilder.CreateIndex(
                name: "IX_markets_MarketId",
                schema: "exchangeaccess",
                table: "markets",
                column: "MarketId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "assets",
                schema: "exchangeaccess");

            migrationBuilder.DropTable(
                name: "candles",
                schema: "exchangeaccess");

            migrationBuilder.DropTable(
                name: "marketasks",
                schema: "exchangeaccess");

            migrationBuilder.DropTable(
                name: "marketbids",
                schema: "exchangeaccess");

            migrationBuilder.DropTable(
                name: "balances",
                schema: "exchangeaccess");

            migrationBuilder.DropTable(
                name: "candlecharts",
                schema: "exchangeaccess");

            migrationBuilder.DropTable(
                name: "markets",
                schema: "exchangeaccess");

            migrationBuilder.DropTable(
                name: "candleperiod",
                schema: "exchangeaccess");

            migrationBuilder.DropSequence(
                name: "assetseq",
                schema: "exchangeaccess");

            migrationBuilder.DropSequence(
                name: "balanceseq",
                schema: "exchangeaccess");

            migrationBuilder.DropSequence(
                name: "candlechartseq",
                schema: "exchangeaccess");

            migrationBuilder.DropSequence(
                name: "candleseq",
                schema: "exchangeaccess");

            migrationBuilder.DropSequence(
                name: "marketaskseq",
                schema: "exchangeaccess");

            migrationBuilder.DropSequence(
                name: "marketbidseq",
                schema: "exchangeaccess");

            migrationBuilder.DropSequence(
                name: "marketseq",
                schema: "exchangeaccess");
        }
    }
}
