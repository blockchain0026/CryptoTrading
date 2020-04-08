using CryptoTrading.Services.Investing.Domain.Model.Investments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Infrastructure.EntityConfigurations
{
    public class InvestmentTypeEntityTypeConfiguration : IEntityTypeConfiguration<InvestmentType>
    {
        public void Configure(EntityTypeBuilder<InvestmentType> investmentTypeConfiguration)
        {
            investmentTypeConfiguration.ToTable("investmenttype", InvestingContext.DEFAULT_SCHEMA);

            investmentTypeConfiguration.HasKey(i => i.Id);

            investmentTypeConfiguration.Property(i => i.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            investmentTypeConfiguration.Property(i => i.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
