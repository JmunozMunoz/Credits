using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Refinancings;
using Sc.Credits.Domain.Model.Stores;
using System;

namespace Sc.Credits.DrivenAdapters.SqlServer.Configurations
{
    /// <summary>
    /// Refinancing log entity type configuration
    /// </summary>
    internal class RefinancingLogEntityTypeConfiguration
        : IEntityTypeConfiguration<RefinancingLog>
    {
        /// <summary>
        ///  <see cref="IEntityTypeConfiguration{TEntity}.Configure(EntityTypeBuilder{TEntity})"/>
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<RefinancingLog> builder)
        {
            builder.ToTable("RefinancingLogs");

            builder.HasKey(refinancing => refinancing.Id);

            builder.Property<DateTime>("Date")
                .IsRequired();

            builder.Property<decimal>("Value")
                .IsRequired();

            builder.Property<string>("ReferenceText")
                .IsRequired();

            builder.Property<string>("ReferenceCode")
                .IsRequired();

            builder.Property<string>("UserId")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property<string>("UserName")
                .HasMaxLength(100)
                .IsRequired();

            builder.HasOne<CreditMaster>()
                .WithMany()
                .HasForeignKey("CreditMasterId")
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<RefinancingApplication>()
                .WithMany()
                .HasForeignKey("ApplicationId")
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Store>()
                .WithMany()
                .HasForeignKey("StoreId")
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Customer>()
                .WithMany()
                .HasForeignKey("CustomerId")
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(refinancing => refinancing.Details)
                .WithOne()
                .HasForeignKey("RefinancingLogId")
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}