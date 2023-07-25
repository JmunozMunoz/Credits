using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc.Credits.Domain.Model.Credits;
using System;

namespace Sc.Credits.DrivenAdapters.SqlServer.Configurations
{
    public class CreditRequestAgentAnalysisEntityTypeConfiguration
        : IEntityTypeConfiguration<CreditRequestAgentAnalysis>
    {
        public void Configure(EntityTypeBuilder<CreditRequestAgentAnalysis> builder)
        {
            builder.ToTable("CreditRequestAgentAnalyses");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.CustomerId)
                   .IsRequired();

            builder.Property(c => c.CustomerIdDocument)
                    .HasMaxLength(50)
                    .IsRequired();

            builder.Property<decimal>("CreditValue")
                    .IsRequired();

            builder.Property(c => c.Observations)
                    .IsRequired();

            builder.Property(c => c.StoreId)
                   .IsRequired();

            builder.Property<DateTime>("TransactionDate")
             .HasColumnType("date")
             .IsRequired();

            builder.HasIndex("TransactionDate");

            builder.Property<TimeSpan>("TransactionTime")
                .HasColumnType("time")
               .IsRequired();

            builder.Property(c => c.AgentAnalysisResultId)
              .IsRequired();

            builder.Property(c => c.SourceId)
               .IsRequired();

            builder.HasOne(c => c.AgentAnalysisResult)
                  .WithMany()
                  .HasForeignKey(c => c.AgentAnalysisResultId)
                  .IsRequired(true)
                  .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Source)
                  .WithMany()
                  .HasForeignKey(c => c.SourceId)
                  .IsRequired(true)
                  .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}