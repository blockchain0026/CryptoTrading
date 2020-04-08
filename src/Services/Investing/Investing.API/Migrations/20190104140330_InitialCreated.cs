using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoTrading.Services.Investing.API.Migrations
{
    public partial class InitialCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "investing");

            migrationBuilder.CreateSequence(
                name: "fundseq",
                schema: "investing",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "investmentroundtripseq",
                schema: "investing",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "investmentseq",
                schema: "investing",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "roundtripseq",
                schema: "investing",
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
                name: "funds",
                schema: "investing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    FundId = table.Column<string>(maxLength: 200, nullable: false),
                    Account_Username = table.Column<string>(nullable: true),
                    Symbol = table.Column<string>(nullable: false),
                    FreeBalance = table.Column<decimal>(type: "decimal(20,8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_funds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "investingfundstatus",
                schema: "investing",
                columns: table => new
                {
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Id = table.Column<int>(nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_investingfundstatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "investmentstatus",
                schema: "investing",
                columns: table => new
                {
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Id = table.Column<int>(nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_investmentstatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "investmenttype",
                schema: "investing",
                columns: table => new
                {
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Id = table.Column<int>(nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_investmenttype", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roundtripstatus",
                schema: "investing",
                columns: table => new
                {
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Id = table.Column<int>(nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roundtripstatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "investingfunds",
                schema: "investing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    InitialQuantity = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    CurrentQuantity = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    EndQuantity = table.Column<decimal>(type: "decimal(20,8)", nullable: true),
                    InvestmentId = table.Column<string>(nullable: false),
                    InvestingFundStatusId = table.Column<int>(nullable: false),
                    FundId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_investingfunds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_investingfunds_funds_FundId",
                        column: x => x.FundId,
                        principalSchema: "investing",
                        principalTable: "funds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_investingfunds_investingfundstatus_InvestingFundStatusId",
                        column: x => x.InvestingFundStatusId,
                        principalSchema: "investing",
                        principalTable: "investingfundstatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "investments",
                schema: "investing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    InvestmentId = table.Column<string>(maxLength: 200, nullable: false),
                    Trace_TraceId = table.Column<string>(nullable: true),
                    Account_Username = table.Column<string>(nullable: true),
                    InvestmentStatusId = table.Column<int>(nullable: false),
                    InvestmentTypeId = table.Column<int>(nullable: false),
                    Market_ExchangeId = table.Column<int>(nullable: false),
                    Market_BaseCurrency = table.Column<string>(nullable: true),
                    Market_QuoteCurrency = table.Column<string>(nullable: true),
                    DateStarted = table.Column<DateTime>(nullable: true),
                    DateClosed = table.Column<DateTime>(nullable: true),
                    InitialBalance = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    EndBalance = table.Column<decimal>(type: "decimal(20,8)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_investments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_investments_investmentstatus_InvestmentStatusId",
                        column: x => x.InvestmentStatusId,
                        principalSchema: "investing",
                        principalTable: "investmentstatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_investments_investmenttype_InvestmentTypeId",
                        column: x => x.InvestmentTypeId,
                        principalSchema: "investing",
                        principalTable: "investmenttype",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "roundtrips",
                schema: "investing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    RoundtripId = table.Column<string>(maxLength: 200, nullable: false),
                    InvestmentId = table.Column<string>(nullable: false),
                    RoundtripNumber = table.Column<int>(nullable: false),
                    RoundtripStatusId = table.Column<int>(nullable: false),
                    Market_ExchangeId = table.Column<int>(nullable: false),
                    Market_BaseCurrency = table.Column<string>(nullable: true),
                    Market_QuoteCurrency = table.Column<string>(nullable: true),
                    EntryAt = table.Column<DateTime>(nullable: true),
                    ExitAt = table.Column<DateTime>(nullable: true),
                    EntryBalance = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    ExitBalance = table.Column<decimal>(type: "decimal(20,8)", nullable: true),
                    TargetPrice = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    StopLossPrice = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    Transaction_BuyPrice = table.Column<decimal>(type: "decimal(20,8)", nullable: true),
                    Transaction_BuyAmount = table.Column<decimal>(type: "decimal(20,8)", nullable: true),
                    Transaction_SellPrice = table.Column<decimal>(type: "decimal(20,8)", nullable: true),
                    Transaction_SellAmount = table.Column<decimal>(type: "decimal(20,8)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roundtrips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_roundtrips_roundtripstatus_RoundtripStatusId",
                        column: x => x.RoundtripStatusId,
                        principalSchema: "investing",
                        principalTable: "roundtripstatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "investmentroundtrips",
                schema: "investing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    RoundtripNumber = table.Column<int>(nullable: false),
                    InvestmentId = table.Column<string>(nullable: false),
                    Market_ExchangeId = table.Column<int>(nullable: false),
                    Market_BaseCurrency = table.Column<string>(nullable: true),
                    Market_QuoteCurrency = table.Column<string>(nullable: true),
                    EntryBalance = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    ExitBalance = table.Column<decimal>(type: "decimal(20,8)", nullable: true),
                    InvestmentId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_investmentroundtrips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_investmentroundtrips_investments_InvestmentId1",
                        column: x => x.InvestmentId1,
                        principalSchema: "investing",
                        principalTable: "investments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_funds_FundId",
                schema: "investing",
                table: "funds",
                column: "FundId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_investingfunds_FundId",
                schema: "investing",
                table: "investingfunds",
                column: "FundId");

            migrationBuilder.CreateIndex(
                name: "IX_investingfunds_InvestingFundStatusId",
                schema: "investing",
                table: "investingfunds",
                column: "InvestingFundStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_investmentroundtrips_InvestmentId1",
                schema: "investing",
                table: "investmentroundtrips",
                column: "InvestmentId1");

            migrationBuilder.CreateIndex(
                name: "IX_investments_InvestmentId",
                schema: "investing",
                table: "investments",
                column: "InvestmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_investments_InvestmentStatusId",
                schema: "investing",
                table: "investments",
                column: "InvestmentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_investments_InvestmentTypeId",
                schema: "investing",
                table: "investments",
                column: "InvestmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_roundtrips_RoundtripId",
                schema: "investing",
                table: "roundtrips",
                column: "RoundtripId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roundtrips_RoundtripStatusId",
                schema: "investing",
                table: "roundtrips",
                column: "RoundtripStatusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientRequests");

            migrationBuilder.DropTable(
                name: "investingfunds",
                schema: "investing");

            migrationBuilder.DropTable(
                name: "investmentroundtrips",
                schema: "investing");

            migrationBuilder.DropTable(
                name: "roundtrips",
                schema: "investing");

            migrationBuilder.DropTable(
                name: "funds",
                schema: "investing");

            migrationBuilder.DropTable(
                name: "investingfundstatus",
                schema: "investing");

            migrationBuilder.DropTable(
                name: "investments",
                schema: "investing");

            migrationBuilder.DropTable(
                name: "roundtripstatus",
                schema: "investing");

            migrationBuilder.DropTable(
                name: "investmentstatus",
                schema: "investing");

            migrationBuilder.DropTable(
                name: "investmenttype",
                schema: "investing");

            migrationBuilder.DropSequence(
                name: "fundseq",
                schema: "investing");

            migrationBuilder.DropSequence(
                name: "investmentroundtripseq",
                schema: "investing");

            migrationBuilder.DropSequence(
                name: "investmentseq",
                schema: "investing");

            migrationBuilder.DropSequence(
                name: "roundtripseq",
                schema: "investing");
        }
    }
}
