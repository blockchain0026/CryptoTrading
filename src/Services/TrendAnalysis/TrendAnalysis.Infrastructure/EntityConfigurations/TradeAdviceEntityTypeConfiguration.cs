using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Infrastructure.EntityConfigurations
{
    public class TradeAdviceEntityTypeConfiguration : IEntityTypeConfiguration<TradeAdvice>
    {
        public void Configure(EntityTypeBuilder<TradeAdvice> tradeAdviceConfiguration)
        {
            tradeAdviceConfiguration.ToTable("tradeadvices", TrendAnalysisContext.DEFAULT_SCHEMA);

            tradeAdviceConfiguration.HasKey(t => t.Id);

            tradeAdviceConfiguration.Ignore(t => t.DomainEvents);

            tradeAdviceConfiguration.Property(t => t.Id)
                .ForSqlServerUseSequenceHiLo("tradeadviceeq", TrendAnalysisContext.DEFAULT_SCHEMA);


         
            tradeAdviceConfiguration.Property(t => t.TradeAdviceId)
                .HasMaxLength(200)
                .IsRequired();
            tradeAdviceConfiguration.HasIndex("TradeAdviceId")
                .IsUnique(true);



            tradeAdviceConfiguration.Property(t => t.TraceId).IsRequired();

            tradeAdviceConfiguration.Property(t => t.DateCreated).IsRequired();


            tradeAdviceConfiguration.Property<int>("TradingSignalTypeId").IsRequired();


            tradeAdviceConfiguration.Property(t => t.TargetPrice).HasColumnType("decimal(20,8)").IsRequired();
            tradeAdviceConfiguration.Property(t => t.Price).HasColumnType("decimal(20,8)").IsRequired();

            tradeAdviceConfiguration.HasOne(t => t.TradingSignalType)
            .WithMany()
            .HasForeignKey("TradingSignalTypeId");
        }
    }
}
