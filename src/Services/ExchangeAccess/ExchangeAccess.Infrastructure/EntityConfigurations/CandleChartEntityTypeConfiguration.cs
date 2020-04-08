using CryptoTrading.Services.ExchangeAccess.Domain.Model.CandleCharts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.ExchangeAccess.Infrastructure.EntityConfigurations
{
    public class CandleChartEntityTypeConfiguration : IEntityTypeConfiguration<CandleChart>
    {
        public void Configure(EntityTypeBuilder<CandleChart> candleChartConfiguration)
        {
            candleChartConfiguration.ToTable("candlecharts", ExchangeAccessContext.DEFAULT_SCHEMA);

            candleChartConfiguration.HasKey(c => c.Id);

            candleChartConfiguration.Ignore(c => c.DomainEvents);

            candleChartConfiguration.Property(c => c.Id)
                .ForSqlServerUseSequenceHiLo("candlechartseq", ExchangeAccessContext.DEFAULT_SCHEMA);



            candleChartConfiguration.Property(c => c.CandleChartId)
                .HasMaxLength(200)
                .IsRequired();

            candleChartConfiguration.HasIndex("CandleChartId")
              .IsUnique(true);

            
            candleChartConfiguration.Property(m => m.MarketId).IsRequired();
            candleChartConfiguration.Property(m => m.BaseCurrency).IsRequired();
            candleChartConfiguration.Property(m => m.QuoteCurrency).IsRequired();
            candleChartConfiguration.Property(m => m.ExchangeId).IsRequired();


            candleChartConfiguration.Property<int>("CandlePeriodId").IsRequired();
            /*orderConfiguration.Property<int>("OrderStatusId").IsRequired();*/

            //candleChartConfiguration.OwnsMany(c => c.Candles).HasKey(a=>a.Timestamp);
            candleChartConfiguration.HasMany(c => c.Candles)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);



           

            candleChartConfiguration.HasOne(o => o.CandlePeriod)
                .WithMany()
                .HasForeignKey("CandlePeriodId");
        }
    }
}
