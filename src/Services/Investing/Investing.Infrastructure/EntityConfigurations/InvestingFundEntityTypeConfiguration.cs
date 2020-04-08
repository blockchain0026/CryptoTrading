using CryptoTrading.Services.Investing.Domain.Model.Funds;
using CryptoTrading.Services.Investing.Domain.Model.Investments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.Investing.Infrastructure.EntityConfigurations
{
    public class InvestingFundEntityTypeConfiguration : IEntityTypeConfiguration<InvestingFund>
    {
        public void Configure(EntityTypeBuilder<InvestingFund> investingFundConfiguration)
        {
            investingFundConfiguration.ToTable("investingfunds", InvestingContext.DEFAULT_SCHEMA);

            investingFundConfiguration.HasKey(i => i.Id);

            investingFundConfiguration.Ignore(i => i.DomainEvents);

            investingFundConfiguration.Property(i => i.Id)
                .ForSqlServerUseSequenceHiLo("investmentroundtripseq", InvestingContext.DEFAULT_SCHEMA);



            /*investingFundConfiguration.Property(t => t.TradeAdviceId)
                .HasMaxLength(200)
                .IsRequired();
            investingFundConfiguration.HasIndex("TradeAdviceId")
                .IsUnique(true);*/

            investingFundConfiguration.Property(i => i.InitialQuantity).HasColumnType("decimal(20,8)").IsRequired();
            investingFundConfiguration.Property(i => i.CurrentQuantity).HasColumnType("decimal(20,8)").IsRequired();
            investingFundConfiguration.Property(i => i.EndQuantity).HasColumnType("decimal(20,8)").IsRequired(false);

            investingFundConfiguration.Property(i => i.InvestmentId).IsRequired();

            investingFundConfiguration.Property<int>("InvestingFundStatusId").IsRequired();






          

        }
    }
}
