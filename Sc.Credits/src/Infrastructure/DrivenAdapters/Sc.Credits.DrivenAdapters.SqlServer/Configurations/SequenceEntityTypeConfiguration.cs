using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc.Credits.Domain.Model.Sequences;

namespace Sc.Credits.DrivenAdapters.SqlServer.Configurations
{
    /// <summary>
    /// Sequence entity type configuration
    /// </summary>
    public class SequenceEntityTypeConfiguration
    : IEntityTypeConfiguration<Sequence>
    {
        /// <summary>
        ///  <see cref="IEntityTypeConfiguration{TEntity}.Configure(EntityTypeBuilder{TEntity})"/>
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<Sequence> builder)
        {
            builder.ToTable("Sequences");

            builder.HasKey(item => item.Id);

            builder.Property(item => item.LastNumber)
                .IsRequired();

            builder.Property(item => item.StoreId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(item => item.Type)
                .HasMaxLength(200)
                .IsRequired();

            builder.HasIndex(item => new { item.StoreId, item.Type })
                .IsUnique();
        }
    }
}