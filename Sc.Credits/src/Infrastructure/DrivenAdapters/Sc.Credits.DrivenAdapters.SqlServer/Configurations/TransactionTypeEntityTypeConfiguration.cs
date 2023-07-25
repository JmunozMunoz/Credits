using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc.Credits.Domain.Model.Credits;

namespace Sc.Credits.DrivenAdapters.SqlServer.Configurations
{
    /// <summary>
    /// Transaction type entity type configuration
    /// </summary>
    internal class TransactionTypeEntityTypeConfiguration
        : IEntityTypeConfiguration<TransactionType>
    {
        /// <summary>
        ///  <see cref="IEntityTypeConfiguration{TEntity}.Configure(EntityTypeBuilder{TEntity})"/>
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<TransactionType> builder)
        {
            builder.ToTable("TransactionTypes");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .ForSqlServerUseSequenceHiLo("TransactionTypesSeq");

            builder.Property("Name")
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}