using CryptoTrading.Services.TrendAnalysis.Domain.Model.Strategies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Infrastructure.EntityConfigurations
{
    public class CandlePeriodEntityTypeConfiguration : IEntityTypeConfiguration<CandlePeriod>
    {
        public void Configure(EntityTypeBuilder<CandlePeriod> candlePeriodConfiguration)
        {
            candlePeriodConfiguration.ToTable("candleperiod", TrendAnalysisContext.DEFAULT_SCHEMA);

            candlePeriodConfiguration.HasKey(c => c.Id);

            candlePeriodConfiguration.Property(c => c.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            candlePeriodConfiguration.Property(c => c.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
