using CryptoTrading.Services.ExchangeAccess.Domain.Model.Balances;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.ExchangeAccess.Infrastructure.EntityConfigurations
{
    public class BalanceEntityTypeConfiguration : IEntityTypeConfiguration<Balance>
    {
        public void Configure(EntityTypeBuilder<Balance> balanceConfiguration)
        {
            balanceConfiguration.ToTable("balances", ExchangeAccessContext.DEFAULT_SCHEMA);

            balanceConfiguration.HasKey(b => b.Id);

            balanceConfiguration.Ignore(b => b.DomainEvents);

            balanceConfiguration.Property(b => b.Id)
                .ForSqlServerUseSequenceHiLo("balanceseq", ExchangeAccessContext.DEFAULT_SCHEMA);

            balanceConfiguration.OwnsOne(b => b.Account);
            balanceConfiguration.Property(b => b.BalanceId).IsRequired();
            balanceConfiguration.Property(b => b.ExchangeId).IsRequired();

            balanceConfiguration.HasMany(s => s.Assets)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
