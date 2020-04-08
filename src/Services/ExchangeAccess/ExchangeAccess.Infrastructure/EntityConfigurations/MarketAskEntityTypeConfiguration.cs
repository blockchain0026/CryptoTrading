using CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.ExchangeAccess.Infrastructure.EntityConfigurations
{
    public class MarketAskEntityTypeConfiguration : IEntityTypeConfiguration<MarketAsk>
    {
        public void Configure(EntityTypeBuilder<MarketAsk> candleConfiguration)
        {
            candleConfiguration.ToTable("marketasks", ExchangeAccessContext.DEFAULT_SCHEMA);

            candleConfiguration.HasKey(s => s.Id);

            candleConfiguration.Ignore(s => s.DomainEvents);

            candleConfiguration.Property(s => s.Id)
                .ForSqlServerUseSequenceHiLo("marketaskseq", ExchangeAccessContext.DEFAULT_SCHEMA);



            candleConfiguration.Property(s => s.MarketId).IsRequired();

            candleConfiguration.Property(s => s.Quantity).HasColumnType("decimal(20,8)").IsRequired();
            candleConfiguration.Property(s => s.Price).HasColumnType("decimal(20,8)").IsRequired();

        }
    }
}
