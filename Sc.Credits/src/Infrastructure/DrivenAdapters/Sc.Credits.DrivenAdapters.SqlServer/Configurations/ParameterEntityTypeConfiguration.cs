using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc.Credits.Domain.Model.Parameters;

namespace Sc.Credits.DrivenAdapters.SqlServer.Configurations
{
    /// <summary>
    /// Parameter entity type configuration
    /// </summary>
    internal class ParameterEntityTypeConfiguration
        : IEntityTypeConfiguration<Parameter>
    {
        /// <summary>
        ///  <see cref="IEntityTypeConfiguration{TEntity}.Configure(EntityTypeBuilder{TEntity})"/>
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<Parameter> builder)
        {
            builder.ToTable("Parameters");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .ForSqlServerUseSequenceHiLo("ParamtersSeq");

            builder.Property<string>("Key")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property<string>("Value")
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}