using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc.Credits.Domain.Model.Credits;

namespace Sc.Credits.DrivenAdapters.SqlServer.Configurations
{
    public class AgentAnalysisResultConfiguration
        : IEntityTypeConfiguration<AgentAnalysisResult>
    {
        public void Configure(EntityTypeBuilder<AgentAnalysisResult> builder)
        {
            builder.ToTable("AgentAnalysisResults");

            builder.HasKey(c => c.Id);

            builder.Property(b => b.Id)
                .ForSqlServerUseSequenceHiLo("AuthMethodsSeq");

            builder.Property(c => c.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}