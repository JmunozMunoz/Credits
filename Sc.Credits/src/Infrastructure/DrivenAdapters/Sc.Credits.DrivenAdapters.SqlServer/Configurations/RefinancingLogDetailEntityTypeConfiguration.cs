using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Refinancings;

namespace Sc.Credits.DrivenAdapters.SqlServer.Configurations
{
    /// <summary>
    /// Refinancing log detail entity type configuration
    /// </summary>
    internal class RefinancingLogDetailEntityTypeConfiguration
        : IEntityTypeConfiguration<RefinancingLogDetail>
    {
        /// <summary>
        ///  <see cref="IEntityTypeConfiguration{TEntity}.Configure(EntityTypeBuilder{TEntity})"/>
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<RefinancingLogDetail> builder)
        {
            builder.ToTable("RefinancingLogDetails");

            builder.HasKey(c => c.Id);

            builder.Property<decimal>("Value")
                .IsRequired();

            builder.HasOne<CreditMaster>()
                .WithMany()
                .HasForeignKey("CreditMasterId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Credit>()
                .WithMany()
                .HasForeignKey("CreditId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property<decimal>("Balance")
                .IsRequired();
        }
    }
}