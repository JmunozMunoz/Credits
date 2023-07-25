using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc.Credits.Domain.Model.Stores;

namespace Sc.Credits.DrivenAdapters.SqlServer.Configurations
{
    /// <summary>
    /// Business group entity type configuration
    /// </summary>
    internal class BusinessGroupEntityTypeConfiguration
        : IEntityTypeConfiguration<BusinessGroup>
    {
        /// <summary>
        ///  <see cref="IEntityTypeConfiguration{TEntity}.Configure(EntityTypeBuilder{TEntity})"/>
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<BusinessGroup> builder)
        {
            builder.ToTable("BusinessGroup");

            builder.HasKey(c => c.Id);

            builder.Property("Name")
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}