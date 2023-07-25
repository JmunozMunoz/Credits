using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc.Credits.Domain.Model.Stores;

namespace Sc.Credits.DrivenAdapters.SqlServer.Configurations
{
    /// <summary>
    /// Store identification entity type configuration
    /// </summary>
    internal class StoreIdentificartionEntityTypeConfiguration
        : IEntityTypeConfiguration<StoreIdentification>
    {
        /// <summary>
        ///  <see cref="IEntityTypeConfiguration{TEntity}.Configure(EntityTypeBuilder{TEntity})"/>
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<StoreIdentification> builder)
        {
            builder.ToTable("StoreIdentifications");

            builder.HasKey(c => new { c.StoreId, c.ScCode });

            builder.Property("StoreId")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property("ScCode")
                .HasMaxLength(10)
                .IsRequired();
        }
    }
}