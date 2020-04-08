using CryptoTrading.Services.ExchangeAccess.Domain.Model.Balances;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.ExchangeAccess.Infrastructure.EntityConfigurations
{
    public class AssetEntityTypeConfiguration : IEntityTypeConfiguration<Asset>
    {
        public void Configure(EntityTypeBuilder<Asset> assetConfiguration)
        {
            assetConfiguration.ToTable("assets", ExchangeAccessContext.DEFAULT_SCHEMA);

            assetConfiguration.HasKey(a => a.Id);

            assetConfiguration.Ignore(a => a.DomainEvents);

            assetConfiguration.Property(a => a.Id)
                .ForSqlServerUseSequenceHiLo("assetseq", ExchangeAccessContext.DEFAULT_SCHEMA);

         

            assetConfiguration.Property(a => a.Symbol).IsRequired();
            assetConfiguration.Property(a=>a.Available).HasColumnType("decimal(20,8)").IsRequired();
            assetConfiguration.Property(a=>a.Locked).HasColumnType("decimal(20,8)").IsRequired();
        }
    }
    
}
