using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc.Credits.Domain.Model.Stores;

namespace Sc.Credits.DrivenAdapters.SqlServer.Configurations
{
    /// <summary>
    /// Collect type entity type configuration
    /// </summary>
    internal class CollectTypeEntityTypeConfiguration
        : IEntityTypeConfiguration<CollectType>
    {
        /// <summary>
        ///  <see cref="IEntityTypeConfiguration{TEntity}.Configure(EntityTypeBuilder{TEntity})"/>
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<CollectType> builder)
        {
            builder.ToTable("CollectTypes");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ForSqlServerUseSequenceHiLo("CollectTypesSeq");

            builder.Property("Name")
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}