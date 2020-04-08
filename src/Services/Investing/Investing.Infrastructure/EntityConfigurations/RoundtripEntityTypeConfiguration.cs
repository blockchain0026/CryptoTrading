using CryptoTrading.Services.Investing.Domain.Model.Roundtrips;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Infrastructure.EntityConfigurations
{

    public class RoundtripEntityTypeConfiguration : IEntityTypeConfiguration<Roundtrip>
    {
        public void Configure(EntityTypeBuilder<Roundtrip> roundtripConfiguration)
        {
            roundtripConfiguration.ToTable("roundtrips", InvestingContext.DEFAULT_SCHEMA);

            roundtripConfiguration.HasKey(r => r.Id);

            roundtripConfiguration.Ignore(r => r.DomainEvents);

            roundtripConfiguration.Property(r => r.Id)
                .ForSqlServerUseSequenceHiLo("roundtripseq", InvestingContext.DEFAULT_SCHEMA);



            roundtripConfiguration.Property(r => r.RoundtripId)
                .HasMaxLength(200)
                .IsRequired();

            roundtripConfiguration.HasIndex("RoundtripId")
              .IsUnique(true);

            roundtripConfiguration.Property(r => r.InvestmentId).IsRequired();

            roundtripConfiguration.Property<int>("RoundtripStatusId").IsRequired();

            roundtripConfiguration.OwnsOne(r => r.Market);

            roundtripConfiguration.Property(r => r.EntryAt).IsRequired(false);
            roundtripConfiguration.Property(r => r.ExitAt).IsRequired(false);

            roundtripConfiguration.Property(r => r.EntryBalance).HasColumnType("decimal(20,8)").IsRequired();
            roundtripConfiguration.Property(r => r.ExitBalance).HasColumnType("decimal(20,8)").IsRequired(false);
            roundtripConfiguration.Property(r => r.TargetPrice).HasColumnType("decimal(20,8)").IsRequired();
            roundtripConfiguration.Property(r => r.StopLossPrice).HasColumnType("decimal(20,8)").IsRequired();

            roundtripConfiguration.OwnsOne(r => r.Transaction,t=> {
                t.Property(p=>p.BuyAmount).HasColumnType("decimal(20,8)").IsRequired(false);
                t.Property(p => p.SellAmount).HasColumnType("decimal(20,8)").IsRequired(false);
                t.Property(p => p.BuyPrice).HasColumnType("decimal(20,8)").IsRequired(false);
                t.Property(p => p.SellPrice).HasColumnType("decimal(20,8)").IsRequired(false);
            });


            roundtripConfiguration.HasOne(i => i.RoundtripStatus)
                .WithMany()
                .HasForeignKey("RoundtripStatusId");
        }
    }
}
