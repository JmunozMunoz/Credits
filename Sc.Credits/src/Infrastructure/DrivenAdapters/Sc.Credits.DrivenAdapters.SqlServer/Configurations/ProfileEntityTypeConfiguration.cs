using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc.Credits.Domain.Model.Customers;

namespace Sc.Credits.DrivenAdapters.SqlServer.Configurations
{
    /// <summary>
    /// Profile entity type configuration
    /// </summary>
    internal class ProfileEntityTypeConfiguration
        : IEntityTypeConfiguration<Profile>
    {
        /// <summary>
        ///  <see cref="IEntityTypeConfiguration{TEntity}.Configure(EntityTypeBuilder{TEntity})"/>
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<Profile> builder)
        {
            builder.ToTable("Profiles");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .ForSqlServerUseSequenceHiLo("ProfilesSeq");

            builder.Property("Name")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(p => p.MandatoryDownPayment)
              .IsRequired();
        }
    }
}