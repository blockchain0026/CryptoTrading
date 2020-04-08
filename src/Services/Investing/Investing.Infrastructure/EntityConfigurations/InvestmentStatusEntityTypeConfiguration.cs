using CryptoTrading.Services.Investing.Domain.Model.Investments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Infrastructure.EntityConfigurations
{

    public class InvestmentStatusEntityTypeConfiguration : IEntityTypeConfiguration<InvestmentStatus>
    {
        public void Configure(EntityTypeBuilder<InvestmentStatus> investmentStatusConfiguration)
        {
            investmentStatusConfiguration.ToTable("investmentstatus", InvestingContext.DEFAULT_SCHEMA);

            investmentStatusConfiguration.HasKey(i => i.Id);

            investmentStatusConfiguration.Property(i => i.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            investmentStatusConfiguration.Property(i => i.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
