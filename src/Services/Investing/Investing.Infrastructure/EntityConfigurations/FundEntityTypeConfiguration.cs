using CryptoTrading.Services.Investing.Domain.Model.Funds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Infrastructure.EntityConfigurations
{
    public class FundEntityTypeConfiguration : IEntityTypeConfiguration<Fund>
    {
        public void Configure(EntityTypeBuilder<Fund> fundConfiguration)
        {
            fundConfiguration.ToTable("funds", InvestingContext.DEFAULT_SCHEMA);

            fundConfiguration.HasKey(f => f.Id);

            fundConfiguration.Ignore(f => f.DomainEvents);

            fundConfiguration.Property(f => f.Id)
                .ForSqlServerUseSequenceHiLo("fundseq", InvestingContext.DEFAULT_SCHEMA);



            fundConfiguration.Property(f => f.FundId)
                .HasMaxLength(200)
                .IsRequired();

            fundConfiguration.HasIndex("FundId")
              .IsUnique(true);


            fundConfiguration.OwnsOne(f => f.Account);
            fundConfiguration.Property(r => r.Symbol).IsRequired();
            fundConfiguration.Property(r => r.FreeBalance).HasColumnType("decimal(20,8)").IsRequired();


            fundConfiguration.HasMany(r => r.InvestingFunds)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
