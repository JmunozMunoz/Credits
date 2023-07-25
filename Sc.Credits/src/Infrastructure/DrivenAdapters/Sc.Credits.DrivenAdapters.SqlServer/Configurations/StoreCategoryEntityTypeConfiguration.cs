using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc.Credits.Domain.Model.Stores;

namespace Sc.Credits.DrivenAdapters.SqlServer.Configurations
{
    /// <summary>
    /// Store category entity type configuration
    /// </summary>
    internal class StoreCategoryEntityTypeConfiguration
        : IEntityTypeConfiguration<StoreCategory>
    {
        /// <summary>
        ///  <see cref="IEntityTypeConfiguration{TEntity}.Configure(EntityTypeBuilder{TEntity})"/>
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<StoreCategory> builder)
        {
            builder.ToTable("StoresCategories");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .UseSqlServerIdentityColumn();

            builder.Property<string>("Name")
              .HasMaxLength(200)
              .IsRequired();

            builder.Property<int>("RegularFeesNumber")
               .IsRequired();

            builder.Property<int>("MaximumFeesNumber")
              .IsRequired();

            builder.Property<decimal>("MinimumFeeValue")
                 .IsRequired();

            builder.Property<decimal>("MaximumCreditValue")
                 .IsRequired();

            builder.Property<decimal>("FeeCutoffValue")
                 .IsRequired();
        }
    }
}