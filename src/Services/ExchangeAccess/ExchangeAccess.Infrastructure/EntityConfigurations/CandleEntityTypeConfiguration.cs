using CryptoTrading.Services.ExchangeAccess.Domain.Model.CandleCharts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.ExchangeAccess.Infrastructure.EntityConfigurations
{
    public class CandleEntityTypeConfiguration : IEntityTypeConfiguration<Candle>
    {
        public void Configure(EntityTypeBuilder<Candle> candleConfiguration)
        {
            candleConfiguration.ToTable("candles", ExchangeAccessContext.DEFAULT_SCHEMA);

            candleConfiguration.HasKey(s => s.Id);

            candleConfiguration.Ignore(s => s.DomainEvents);

            candleConfiguration.Property(s => s.Id)
                .ForSqlServerUseSequenceHiLo("candleseq", ExchangeAccessContext.DEFAULT_SCHEMA);



            candleConfiguration.Property(s => s.Timestamp).IsRequired();

            candleConfiguration.Property(s => s.High).HasColumnType("decimal(20,8)").IsRequired();
            candleConfiguration.Property(s => s.Low).HasColumnType("decimal(20,8)").IsRequired();
            candleConfiguration.Property(s => s.Open).HasColumnType("decimal(20,8)").IsRequired();
            candleConfiguration.Property(s => s.Close).HasColumnType("decimal(20,8)").IsRequired();
            candleConfiguration.Property(s => s.Volume).HasColumnType("decimal(20,8)").IsRequired();

        }
    }
}

