using CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.ExchangeAccess.Infrastructure.EntityConfigurations
{
    public class MarketEntityTypeConfiguration : IEntityTypeConfiguration<Market>
    {
        public void Configure(EntityTypeBuilder<Market> marketConfiguration)
        {
            marketConfiguration.ToTable("markets", ExchangeAccessContext.DEFAULT_SCHEMA);

            marketConfiguration.HasKey(m => m.Id);

            marketConfiguration.Ignore(m => m.DomainEvents);

            marketConfiguration.Property(m => m.Id)
                .ForSqlServerUseSequenceHiLo("marketseq", ExchangeAccessContext.DEFAULT_SCHEMA);



            marketConfiguration.Property(m => m.MarketId)
                .HasMaxLength(200)
                .IsRequired();

            marketConfiguration.HasIndex("MarketId")
              .IsUnique(true);

            marketConfiguration.OwnsOne(m => m.Exchange);
            marketConfiguration.Property(m => m.BaseCurrency).IsRequired();
            marketConfiguration.Property(m => m.QuoteCurrency).IsRequired();
            marketConfiguration.Property(m => m.OrderSizeLimit).IsRequired();

            /*marketConfiguration.OwnsMany<Ask>("Asks").HasKey(a => a.Price);
            marketConfiguration.OwnsMany<Bid>("Bids").HasKey(b => b.Price);*/

            marketConfiguration.HasMany(c => c.Asks)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
            marketConfiguration.HasMany(c => c.Bids)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
            /*orderConfiguration.Property<int>("OrderTypeId").IsRequired();
            orderConfiguration.Property<int>("OrderStatusId").IsRequired();*/





            /*orderConfiguration.HasMany(b => b.PaymentMethods)
               .WithOne()
               .HasForeignKey("BuyerId")
               .OnDelete(DeleteBehavior.Cascade);*/

            /*orderConfiguration.HasOne(o => o.OrderType)
                .WithMany()
                .HasForeignKey("OrderTypeId");

            orderConfiguration.HasOne(o => o.OrderStatus)
                .WithMany()
                .HasForeignKey("OrderStatusId");*/
        }

    }
}
