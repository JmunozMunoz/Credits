using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc.Credits.Helpers.Commons.Messaging;

namespace Sc.Credits.DrivenAdapters.SqlServer.Configurations
{
    /// <summary>
    /// Message error log entity type configuration
    /// </summary>
    internal class MessageErrorLogEntityTypeConfiguration
        : IEntityTypeConfiguration<MessageErrorLog>
    {
        /// <summary>
        ///  <see cref="IEntityTypeConfiguration{TEntity}.Configure(EntityTypeBuilder{TEntity})"/>
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<MessageErrorLog> builder)
        {
            builder.ToTable("MessageErrorLogs");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Attempts)
                .IsRequired();

            builder.Property(b => b.Date)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(b => b.Json)
                .HasColumnType("nvarchar(MAX)")
                .IsRequired();

            builder.Property(b => b.Key)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(b => b.LastAttemptDate)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(b => b.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(b => b.Processed)
                .IsRequired();

            builder.Property(b => b.ResourceName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(b => b.ResourceType)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(b => new { b.LastAttemptDate, b.Attempts, b.Processed });

            builder.HasIndex(b => b.Key);

            builder.HasIndex(b => b.Name);
        }
    }
}