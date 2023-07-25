using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc.Credits.Domain.Model.Credits;

namespace Sc.Credits.DrivenAdapters.SqlServer.Configurations
{
    /// <summary>
    /// Source entity type configuration
    /// </summary>
    internal class SourceEntityTypeConfiguration
        : IEntityTypeConfiguration<Source>
    {
        /// <summary>
        ///  <see cref="IEntityTypeConfiguration{TEntity}.Configure(EntityTypeBuilder{TEntity})"/>
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<Source> builder)
        {
            builder.ToTable("Sources");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .ForSqlServerUseSequenceHiLo("SourcesSeq");

            builder.Property("Name")
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}