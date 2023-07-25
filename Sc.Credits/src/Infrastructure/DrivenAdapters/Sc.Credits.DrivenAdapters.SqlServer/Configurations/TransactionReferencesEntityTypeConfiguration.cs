using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc.Credits.Domain.Model.Credits;

namespace Sc.Credits.DrivenAdapters.SqlServer.Configurations
{
    /// <summary>
    /// Transaction references entity type configuration
    /// </summary>
    internal class TransactionReferencesEntityTypeConfiguration
        : IEntityTypeConfiguration<TransactionReference>
    {
        /// <summary>
        /// <see cref="IEntityTypeConfiguration{TEntity}.Configure(EntityTypeBuilder{TEntity})"/>
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<TransactionReference> builder)
        {
            builder.ToTable("TransactionReferences");

            builder.HasKey(c => new { c.CreditId, c.TransactionId });

            builder.Property(c => c.CreditId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.TransactionId)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}