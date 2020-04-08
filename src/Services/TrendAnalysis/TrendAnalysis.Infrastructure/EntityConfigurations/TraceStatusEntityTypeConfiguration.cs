using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrading.Services.TrendAnalysis.Infrastructure.EntityConfigurations
{
    public class TraceStatusEntityTypeConfiguration : IEntityTypeConfiguration<TraceStatus>
    {
        public void Configure(EntityTypeBuilder<TraceStatus> tracestatusConfiguration)
        {
            tracestatusConfiguration.ToTable("tracestatus", TrendAnalysisContext.DEFAULT_SCHEMA);

            tracestatusConfiguration.HasKey(t => t.Id);

            tracestatusConfiguration.Property(t => t.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            tracestatusConfiguration.Property(t => t.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
