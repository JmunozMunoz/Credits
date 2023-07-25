using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc.Credits.Domain.Model.Credits;

namespace Sc.Credits.DrivenAdapters.SqlServer.Configurations
{
    /// <summary>
    /// Auth method entity type configuration
    /// </summary>
    internal class AuthMethodEntityTypeConfiguration
        : IEntityTypeConfiguration<AuthMethod>
    {
        /// <summary>
        ///  <see cref="IEntityTypeConfiguration{TEntity}.Configure(EntityTypeBuilder{TEntity})"/>
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<AuthMethod> builder)
        {
            builder.ToTable("AuthMethods");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .ForSqlServerUseSequenceHiLo("AuthMethodsSeq");

            builder.Property("Name")
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}