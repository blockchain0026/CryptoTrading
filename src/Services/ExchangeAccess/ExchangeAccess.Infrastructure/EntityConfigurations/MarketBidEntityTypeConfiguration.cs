using CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.ExchangeAccess.Infrastructure.EntityConfigurations
{
    public class MarketBidEntityTypeConfiguration : IEntityTypeConfiguration<MarketBid>
    {
        public void Configure(EntityTypeBuilder<MarketBid> candleConfiguration)
        {
            candleConfiguration.ToTable("marketbids", ExchangeAccessContext.DEFAULT_SCHEMA);

            candleConfiguration.HasKey(s => s.Id);

            candleConfiguration.Ignore(s => s.DomainEvents);

            candleConfiguration.Property(s => s.Id)
                .ForSqlServerUseSequenceHiLo("marketbidseq", ExchangeAccessContext.DEFAULT_SCHEMA);



            candleConfiguration.Property(s => s.MarketId).IsRequired();

            candleConfiguration.Property(s => s.Quantity).HasColumnType("decimal(20,8)").IsRequired();
            candleConfiguration.Property(s => s.Price).HasColumnType("decimal(20,8)").IsRequired();

        }
    }
}
