using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc.Credits.Domain.Model.Credits;

namespace Sc.Credits.DrivenAdapters.SqlServer.Configurations
{
    /// <summary>
    /// Payment type entity type configuration
    /// </summary>
    internal class PaymentTypeEntityTypeConfiguration
        : IEntityTypeConfiguration<PaymentType>
    {
        /// <summary>
        ///  <see cref="IEntityTypeConfiguration{TEntity}.Configure(EntityTypeBuilder{TEntity})"/>
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<PaymentType> builder)
        {
            builder.ToTable("PaymentTypes");

            builder.HasKey(p => p.Id);

            builder.Property("Type")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(p => p.Id)
                .ForSqlServerUseSequenceHiLo("PaymentTypesSeq");

            builder.Property("Name")
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}