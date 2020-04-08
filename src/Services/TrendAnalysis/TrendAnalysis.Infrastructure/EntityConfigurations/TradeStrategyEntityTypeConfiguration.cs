using CryptoTrading.Services.TrendAnalysis.Domain.Model.Strategies;
using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Infrastructure.EntityConfigurations
{
    public class TradeStrategyEntityTypeConfiguration : IEntityTypeConfiguration<TradeStrategy>
    {
        public void Configure(EntityTypeBuilder<TradeStrategy> tradeStrategyConfiguration)
        {
            tradeStrategyConfiguration.ToTable("tradestrategies", TrendAnalysisContext.DEFAULT_SCHEMA);

            tradeStrategyConfiguration.HasKey(t => t.Id);

            tradeStrategyConfiguration.Ignore(t => t.DomainEvents);

            tradeStrategyConfiguration.Property(t => t.Id)
                .ForSqlServerUseSequenceHiLo("tradestrategyseq", TrendAnalysisContext.DEFAULT_SCHEMA);


            //No need for entity.
            tradeStrategyConfiguration.Property(t => t.StrategyId)
                .HasMaxLength(200)
                .IsRequired();

            tradeStrategyConfiguration.HasIndex("StrategyId")
              .IsUnique(true);

            tradeStrategyConfiguration.Property(t => t.StrategyId).IsRequired();

            tradeStrategyConfiguration.Property(t => t.TraceId).IsRequired();


            tradeStrategyConfiguration.OwnsOne(t => t.Strategy, s =>
            {

                s.Property<int>("CandlePeriodId").IsRequired();
                s.HasOne(c => c.CandlePeriod)
                .WithMany()
                .HasForeignKey("CandlePeriodId");
            });

            tradeStrategyConfiguration.Property(t => t.Weight).IsRequired();
            tradeStrategyConfiguration.Property(t => t.WarmUp).IsRequired();

            tradeStrategyConfiguration.OwnsOne(t => t.TradeSignal, ts =>
            {
                ts.Property<int>("TradingSignalTypeId");
                ts.HasOne(tst => tst.TradingSignalType)
                .WithMany()
                .HasForeignKey("TradingSignalTypeId");
            });
        }

    }
}
