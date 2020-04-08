﻿// <auto-generated />
using System;
using CryptoTrading.Services.TrendAnalysis.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CryptoTrading.Services.TrendAnalysis.API.Migrations
{
    [DbContext(typeof(TrendAnalysisContext))]
    [Migration("20181219094548_InitialCreated")]
    partial class InitialCreated
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("Relational:Sequence:trendanalysis.traceseq", "'traceseq', 'trendanalysis', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("Relational:Sequence:trendanalysis.tradeadviceeq", "'tradeadviceeq', 'trendanalysis', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("Relational:Sequence:trendanalysis.tradestrategyseq", "'tradestrategyseq', 'trendanalysis', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CryptoTrading.Services.TrendAnalysis.Domain.Model.Strategies.CandlePeriod", b =>
                {
                    b.Property<int>("Id")
                        .HasDefaultValue(1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("candleperiod","trendanalysis");
                });

            modelBuilder.Entity("CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces.Trace", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "traceseq")
                        .HasAnnotation("SqlServer:HiLoSequenceSchema", "trendanalysis")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<DateTime?>("DateClosed");

                    b.Property<DateTime?>("DateStarted");

                    b.Property<string>("IdealCandlePeriod");

                    b.Property<string>("TraceId")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<int>("TraceStatusId");

                    b.HasKey("Id");

                    b.HasIndex("TraceId")
                        .IsUnique();

                    b.HasIndex("TraceStatusId");

                    b.ToTable("traces","trendanalysis");
                });

            modelBuilder.Entity("CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces.TraceStatus", b =>
                {
                    b.Property<int>("Id")
                        .HasDefaultValue(1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("tracestatus","trendanalysis");
                });

            modelBuilder.Entity("CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces.TradeAdvice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "tradeadviceeq")
                        .HasAnnotation("SqlServer:HiLoSequenceSchema", "trendanalysis")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<DateTime>("DateCreated");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(20,8)");

                    b.Property<decimal>("TargetPrice")
                        .HasColumnType("decimal(20,8)");

                    b.Property<string>("TraceId")
                        .IsRequired();

                    b.Property<int?>("TraceId1");

                    b.Property<string>("TradeAdviceId")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<int>("TradingSignalTypeId");

                    b.HasKey("Id");

                    b.HasIndex("TraceId1");

                    b.HasIndex("TradeAdviceId")
                        .IsUnique();

                    b.HasIndex("TradingSignalTypeId");

                    b.ToTable("tradeadvices","trendanalysis");
                });

            modelBuilder.Entity("CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces.TradeStrategy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "tradestrategyseq")
                        .HasAnnotation("SqlServer:HiLoSequenceSchema", "trendanalysis")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<string>("StrategyId")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("TraceId")
                        .IsRequired();

                    b.Property<int?>("TraceId1");

                    b.Property<int>("WarmUp");

                    b.Property<int>("Weight");

                    b.HasKey("Id");

                    b.HasIndex("StrategyId")
                        .IsUnique();

                    b.HasIndex("TraceId1");

                    b.ToTable("tradestrategies","trendanalysis");
                });

            modelBuilder.Entity("CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces.TradingSignalType", b =>
                {
                    b.Property<int>("Id")
                        .HasDefaultValue(1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("tradingsignaltype","trendanalysis");
                });

            modelBuilder.Entity("CryptoTrading.Services.TrendAnalysis.Infrastructure.Idempotency.ClientRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<DateTime>("Time");

                    b.HasKey("Id");

                    b.ToTable("ClientRequests");
                });

            modelBuilder.Entity("CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces.Trace", b =>
                {
                    b.HasOne("CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces.TraceStatus", "TraceStatus")
                        .WithMany()
                        .HasForeignKey("TraceStatusId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.OwnsOne("CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces.Investment", "Investment", b1 =>
                        {
                            b1.Property<int?>("TraceId");

                            b1.Property<string>("InvestmentId");

                            b1.Property<bool>("IsClosed");

                            b1.ToTable("traces","trendanalysis");

                            b1.HasOne("CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces.Trace")
                                .WithOne("Investment")
                                .HasForeignKey("CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces.Investment", "TraceId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });

                    b.OwnsOne("CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces.Market", "Market", b1 =>
                        {
                            b1.Property<int?>("TraceId");

                            b1.Property<string>("BaseCurrency");

                            b1.Property<int>("ExchangeId");

                            b1.Property<string>("QuoteCurrency");

                            b1.ToTable("traces","trendanalysis");

                            b1.HasOne("CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces.Trace")
                                .WithOne("Market")
                                .HasForeignKey("CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces.Market", "TraceId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });

            modelBuilder.Entity("CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces.TradeAdvice", b =>
                {
                    b.HasOne("CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces.Trace")
                        .WithMany("TradeAdvices")
                        .HasForeignKey("TraceId1")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces.TradingSignalType", "TradingSignalType")
                        .WithMany()
                        .HasForeignKey("TradingSignalTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces.TradeStrategy", b =>
                {
                    b.HasOne("CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces.Trace")
                        .WithMany("TradeStrategies")
                        .HasForeignKey("TraceId1")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.OwnsOne("CryptoTrading.Services.TrendAnalysis.Domain.Model.Strategies.Strategy", "Strategy", b1 =>
                        {
                            b1.Property<int?>("TradeStrategyId");

                            b1.Property<int>("CandlePeriodId");

                            b1.Property<int>("MinimumAmountOfCandles");

                            b1.Property<string>("Name");

                            b1.HasIndex("CandlePeriodId");

                            b1.ToTable("tradestrategies","trendanalysis");

                            b1.HasOne("CryptoTrading.Services.TrendAnalysis.Domain.Model.Strategies.CandlePeriod", "CandlePeriod")
                                .WithMany()
                                .HasForeignKey("CandlePeriodId")
                                .OnDelete(DeleteBehavior.Cascade);

                            b1.HasOne("CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces.TradeStrategy")
                                .WithOne("Strategy")
                                .HasForeignKey("CryptoTrading.Services.TrendAnalysis.Domain.Model.Strategies.Strategy", "TradeStrategyId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });

                    b.OwnsOne("CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces.TradeSignal", "TradeSignal", b1 =>
                        {
                            b1.Property<int?>("TradeStrategyId");

                            b1.Property<decimal>("Price");

                            b1.Property<DateTime>("SignalCandleDateTime");

                            b1.Property<string>("TraceId");

                            b1.Property<int>("TradingSignalTypeId");

                            b1.HasIndex("TradingSignalTypeId");

                            b1.ToTable("tradestrategies","trendanalysis");

                            b1.HasOne("CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces.TradeStrategy")
                                .WithOne("TradeSignal")
                                .HasForeignKey("CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces.TradeSignal", "TradeStrategyId")
                                .OnDelete(DeleteBehavior.Cascade);

                            b1.HasOne("CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces.TradingSignalType", "TradingSignalType")
                                .WithMany()
                                .HasForeignKey("TradingSignalTypeId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
