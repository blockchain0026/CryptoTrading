using CryptoTrading.Services.Investing.Domain.Model.Investments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Infrastructure.EntityConfigurations
{
    public class InvestmentRoundtripEntityTypeConfiguration : IEntityTypeConfiguration<InvestmentRoundtrip>
    {
        public void Configure(EntityTypeBuilder<InvestmentRoundtrip> investmentRoundtripConfiguration)
        {
            investmentRoundtripConfiguration.ToTable("investmentroundtrips", InvestingContext.DEFAULT_SCHEMA);

            investmentRoundtripConfiguration.HasKey(i => i.Id);

            investmentRoundtripConfiguration.Ignore(i => i.DomainEvents);

            investmentRoundtripConfiguration.Property(i => i.Id)
                .ForSqlServerUseSequenceHiLo("investmentroundtripseq", InvestingContext.DEFAULT_SCHEMA);



            /*investmentRoundtripConfiguration.Property(t => t.TradeAdviceId)
                .HasMaxLength(200)
                .IsRequired();
            investmentRoundtripConfiguration.HasIndex("TradeAdviceId")
                .IsUnique(true);*/



            investmentRoundtripConfiguration.Property(i => i.RoundtripNumber).IsRequired();
            investmentRoundtripConfiguration.Property(i => i.InvestmentId).IsRequired();
            investmentRoundtripConfiguration.OwnsOne(i => i.Market);



            investmentRoundtripConfiguration.Property(i => i.EntryBalance).HasColumnType("decimal(20,8)").IsRequired();
            investmentRoundtripConfiguration.Property(i => i.ExitBalance).HasColumnType("decimal(20,8)").IsRequired(false);


        }
    }
}
