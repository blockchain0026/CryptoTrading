using CryptoTrading.Services.Investing.Domain.Model.Roundtrips;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Infrastructure.EntityConfigurations
{
    public class RoundtripStatusEntityTypeConfiguration : IEntityTypeConfiguration<RoundtripStatus>
    {
        public void Configure(EntityTypeBuilder<RoundtripStatus> investmentTypeConfiguration)
        {
            investmentTypeConfiguration.ToTable("roundtripstatus", InvestingContext.DEFAULT_SCHEMA);

            investmentTypeConfiguration.HasKey(r => r.Id);

            investmentTypeConfiguration.Property(r => r.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            investmentTypeConfiguration.Property(r => r.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
