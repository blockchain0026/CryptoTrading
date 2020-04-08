using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Infrastructure.EntityConfigurations
{
    public class TraceEntityTypeConfiguration : IEntityTypeConfiguration<Trace>
    {
        public void Configure(EntityTypeBuilder<Trace> traceConfiguration)
        {
            traceConfiguration.ToTable("traces", TrendAnalysisContext.DEFAULT_SCHEMA);

            traceConfiguration.HasKey(t => t.Id);

            traceConfiguration.Ignore(t => t.DomainEvents);

            traceConfiguration.Property(t => t.Id)
                .ForSqlServerUseSequenceHiLo("traceseq", TrendAnalysisContext.DEFAULT_SCHEMA);



            traceConfiguration.Property(t => t.TraceId)
                .HasMaxLength(200)
                .IsRequired();

            traceConfiguration.HasIndex("TraceId")
              .IsUnique(true);

            traceConfiguration.OwnsOne(t => t.Investment);
            traceConfiguration.OwnsOne(t => t.Market);
            traceConfiguration.Property<int>("TraceStatusId").IsRequired();
            traceConfiguration.Property(t => t.DateStarted).IsRequired(false);
            traceConfiguration.Property(t => t.DateClosed).IsRequired(false);
            traceConfiguration.Property(t => t.IdealCandlePeriod).IsRequired(false);
            

            traceConfiguration.HasMany(t => t.TradeStrategies)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
            traceConfiguration.HasMany(t => t.TradeAdvices)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);



            /*orderConfiguration.HasMany(b => b.PaymentMethods)
               .WithOne()
               .HasForeignKey("BuyerId")
               .OnDelete(DeleteBehavior.Cascade);*/

            traceConfiguration.HasOne(t => t.TraceStatus)
                .WithMany()
                .HasForeignKey("TraceStatusId");
        }

    }
}
