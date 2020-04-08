using CryptoTrading.Services.Investing.Domain.Model.Investments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Infrastructure.EntityConfigurations
{
    public class InvestmentEntityTypeConfiguration : IEntityTypeConfiguration<Investment>
    {
        public void Configure(EntityTypeBuilder<Investment> investmentConfiguration)
        {
            investmentConfiguration.ToTable("investments", InvestingContext.DEFAULT_SCHEMA);

            investmentConfiguration.HasKey(i => i.Id);

            investmentConfiguration.Ignore(i => i.DomainEvents);

            investmentConfiguration.Property(i => i.Id)
                .ForSqlServerUseSequenceHiLo("investmentseq", InvestingContext.DEFAULT_SCHEMA);



            investmentConfiguration.Property(i => i.InvestmentId)
                .HasMaxLength(200)
                .IsRequired();

            investmentConfiguration.HasIndex("InvestmentId")
              .IsUnique(true);

            investmentConfiguration.OwnsOne(i => i.Trace);
            investmentConfiguration.OwnsOne(i => i.Account);

            investmentConfiguration.Property<int>("InvestmentStatusId").IsRequired();
            investmentConfiguration.Property<int>("InvestmentTypeId").IsRequired();

            investmentConfiguration.OwnsOne(i => i.Market);

            investmentConfiguration.Property(i => i.DateStarted).IsRequired(false);
            investmentConfiguration.Property(i => i.DateClosed).IsRequired(false);
            investmentConfiguration.Property(i => i.InitialBalance).HasColumnType("decimal(20,8)").IsRequired();
            investmentConfiguration.Property(i => i.CurrentBalance).HasColumnType("decimal(20,8)").IsRequired();
            investmentConfiguration.Property(i => i.EndBalance).HasColumnType("decimal(20,8)").IsRequired(false);



            investmentConfiguration.HasMany(i => i.InvestmentRoundtrips)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);


            investmentConfiguration.HasOne(i => i.InvestmentStatus)
                .WithMany()
                .HasForeignKey("InvestmentStatusId");
            investmentConfiguration.HasOne(i => i.InvestmentType)
                .WithMany()
                .HasForeignKey("InvestmentTypeId");
        }
    }
}
