using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc.Credits.Domain.Model.Credits;

namespace Sc.Credits.DrivenAdapters.SqlServer.Configurations
{
    /// <summary>
    /// Request status entity type configuration
    /// </summary>
    internal class RequestStatusEntityTypeConfiguration
        : IEntityTypeConfiguration<RequestStatus>
    {
        /// <summary>
        ///  <see cref="IEntityTypeConfiguration{TEntity}.Configure(EntityTypeBuilder{TEntity})"/>
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<RequestStatus> builder)
        {
            builder.ToTable("RequestStatus");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ForSqlServerUseSequenceHiLo("RequestStatusSeq");

            builder.Property("Name")
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}