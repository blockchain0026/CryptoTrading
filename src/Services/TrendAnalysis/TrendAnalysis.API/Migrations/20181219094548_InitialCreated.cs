using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoTrading.Services.TrendAnalysis.API.Migrations
{
    public partial class InitialCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "trendanalysis");

            migrationBuilder.CreateSequence(
                name: "traceseq",
                schema: "trendanalysis",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "tradeadviceeq",
                schema: "trendanalysis",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "tradestrategyseq",
                schema: "trendanalysis",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "ClientRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "candleperiod",
                schema: "trendanalysis",
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
                name: "tracestatus",
                schema: "trendanalysis",
                columns: table => new
                {
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Id = table.Column<int>(nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tracestatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tradingsignaltype",
                schema: "trendanalysis",
                columns: table => new
                {
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Id = table.Column<int>(nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tradingsignaltype", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "traces",
                schema: "trendanalysis",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    TraceId = table.Column<string>(maxLength: 200, nullable: false),
                    Investment_InvestmentId = table.Column<string>(nullable: true),
                    Investment_IsClosed = table.Column<bool>(nullable: false),
                    Market_ExchangeId = table.Column<int>(nullable: false),
                    Market_BaseCurrency = table.Column<string>(nullable: true),
                    Market_QuoteCurrency = table.Column<string>(nullable: true),
                    TraceStatusId = table.Column<int>(nullable: false),
                    DateStarted = table.Column<DateTime>(nullable: true),
                    DateClosed = table.Column<DateTime>(nullable: true),
                    IdealCandlePeriod = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_traces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_traces_tracestatus_TraceStatusId",
                        column: x => x.TraceStatusId,
                        principalSchema: "trendanalysis",
                        principalTable: "tracestatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tradeadvices",
                schema: "trendanalysis",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    TradeAdviceId = table.Column<string>(maxLength: 200, nullable: false),
                    TraceId = table.Column<string>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    TradingSignalTypeId = table.Column<int>(nullable: false),
                    TargetPrice = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    TraceId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tradeadvices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tradeadvices_traces_TraceId1",
                        column: x => x.TraceId1,
                        principalSchema: "trendanalysis",
                        principalTable: "traces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tradeadvices_tradingsignaltype_TradingSignalTypeId",
                        column: x => x.TradingSignalTypeId,
                        principalSchema: "trendanalysis",
                        principalTable: "tradingsignaltype",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tradestrategies",
                schema: "trendanalysis",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    StrategyId = table.Column<string>(maxLength: 200, nullable: false),
                    TraceId = table.Column<string>(nullable: false),
                    Strategy_Name = table.Column<string>(nullable: true),
                    Strategy_MinimumAmountOfCandles = table.Column<int>(nullable: false),
                    Strategy_CandlePeriodId = table.Column<int>(nullable: false),
                    Weight = table.Column<int>(nullable: false),
                    WarmUp = table.Column<int>(nullable: false),
                    TradeSignal_TraceId = table.Column<string>(nullable: true),
                    TradeSignal_SignalCandleDateTime = table.Column<DateTime>(nullable: false),
                    TradeSignal_Price = table.Column<decimal>(nullable: false),
                    TradeSignal_TradingSignalTypeId = table.Column<int>(nullable: false),
                    TraceId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tradestrategies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tradestrategies_candleperiod_Strategy_CandlePeriodId",
                        column: x => x.Strategy_CandlePeriodId,
                        principalSchema: "trendanalysis",
                        principalTable: "candleperiod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tradestrategies_tradingsignaltype_TradeSignal_TradingSignalTypeId",
                        column: x => x.TradeSignal_TradingSignalTypeId,
                        principalSchema: "trendanalysis",
                        principalTable: "tradingsignaltype",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tradestrategies_traces_TraceId1",
                        column: x => x.TraceId1,
                        principalSchema: "trendanalysis",
                        principalTable: "traces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_traces_TraceId",
                schema: "trendanalysis",
                table: "traces",
                column: "TraceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_traces_TraceStatusId",
                schema: "trendanalysis",
                table: "traces",
                column: "TraceStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_tradeadvices_TraceId1",
                schema: "trendanalysis",
                table: "tradeadvices",
                column: "TraceId1");

            migrationBuilder.CreateIndex(
                name: "IX_tradeadvices_TradeAdviceId",
                schema: "trendanalysis",
                table: "tradeadvices",
                column: "TradeAdviceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tradeadvices_TradingSignalTypeId",
                schema: "trendanalysis",
                table: "tradeadvices",
                column: "TradingSignalTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_tradestrategies_Strategy_CandlePeriodId",
                schema: "trendanalysis",
                table: "tradestrategies",
                column: "Strategy_CandlePeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_tradestrategies_TradeSignal_TradingSignalTypeId",
                schema: "trendanalysis",
                table: "tradestrategies",
                column: "TradeSignal_TradingSignalTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_tradestrategies_StrategyId",
                schema: "trendanalysis",
                table: "tradestrategies",
                column: "StrategyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tradestrategies_TraceId1",
                schema: "trendanalysis",
                table: "tradestrategies",
                column: "TraceId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientRequests");

            migrationBuilder.DropTable(
                name: "tradeadvices",
                schema: "trendanalysis");

            migrationBuilder.DropTable(
                name: "tradestrategies",
                schema: "trendanalysis");

            migrationBuilder.DropTable(
                name: "candleperiod",
                schema: "trendanalysis");

            migrationBuilder.DropTable(
                name: "tradingsignaltype",
                schema: "trendanalysis");

            migrationBuilder.DropTable(
                name: "traces",
                schema: "trendanalysis");

            migrationBuilder.DropTable(
                name: "tracestatus",
                schema: "trendanalysis");

            migrationBuilder.DropSequence(
                name: "traceseq",
                schema: "trendanalysis");

            migrationBuilder.DropSequence(
                name: "tradeadviceeq",
                schema: "trendanalysis");

            migrationBuilder.DropSequence(
                name: "tradestrategyseq",
                schema: "trendanalysis");
        }
    }
}
