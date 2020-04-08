using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Infrastructure.EntityConfigurations
{
    public class TradingSignalTypeEntityTypeConfiguration : IEntityTypeConfiguration<TradingSignalType>
    {
        public void Configure(EntityTypeBuilder<TradingSignalType> tradingSignalTypeConfiguration)
        {
            tradingSignalTypeConfiguration.ToTable("tradingsignaltype", TrendAnalysisContext.DEFAULT_SCHEMA);

            tradingSignalTypeConfiguration.HasKey(t => t.Id);

            tradingSignalTypeConfiguration.Property(t => t.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            tradingSignalTypeConfiguration.Property(t => t.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
