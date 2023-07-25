using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc.Credits.Domain.Model.Stores;

namespace Sc.Credits.DrivenAdapters.SqlServer.Configurations
{
    /// <summary>
    /// Assurance company entity type configuration
    /// </summary>
    internal class AssuranceCompanyEntityTypeConfiguration
        : IEntityTypeConfiguration<AssuranceCompany>
    {
        /// <summary>
        ///  <see cref="IEntityTypeConfiguration{TEntity}.Configure(EntityTypeBuilder{TEntity})"/>
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<AssuranceCompany> builder)
        {
            builder.ToTable("AssuranceCompanies");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ForSqlServerUseSequenceHiLo("AssuranceCompaniesSeq");

            builder.Property("Name")
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}