using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc.Credits.Domain.Model.Credits;
using System;

namespace Sc.Credits.DrivenAdapters.SqlServer.Configurations
{
    /// <summary>
    /// Request cancel credit entity type configuration
    /// </summary>
    internal class RequestCancelCreditEntityTypeConfiguration
        : IEntityTypeConfiguration<RequestCancelCredit>
    {
        /// <summary>
        ///  <see cref="IEntityTypeConfiguration{TEntity}.Configure(EntityTypeBuilder{TEntity})"/>
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<RequestCancelCredit> builder)
        {
            builder.ToTable("RequestCancelCredits");

            builder.HasKey(p => p.Id);

            builder.Property<string>("Reason")
                .IsRequired(true);

            builder.Property<string>("UserName")
                .HasMaxLength(100)
                .IsRequired(true);

            builder.Property<string>("UserId")
                .HasMaxLength(100)
                .IsRequired(true);

            builder.Property<DateTime>("Date")
                .HasColumnType("date")
                .IsRequired(true);

            builder.Property<TimeSpan>("Time")
                .HasColumnType("time")
                .IsRequired(true);

            builder.Property<DateTime?>("ProcessDate")
                .HasColumnType("date")
                .IsRequired(false);

            builder.Property<TimeSpan?>("ProcessTime")
                .HasColumnType("time")
                .IsRequired(false);

            builder.Property<string>("ProcessUserName")
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property<string>("ProcessUserId")
                .HasMaxLength(100)
                .IsRequired(false);

            builder.HasIndex("ProcessDate");

            builder.HasOne<RequestStatus>()
                .WithMany()
                .HasForeignKey("RequestStatusId")
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(item => item.CreditMaster)
               .WithMany()
               .HasForeignKey("CreditMasterId")
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(item => item.Store)
                .WithMany()
                .HasForeignKey("StoreId")
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property<decimal>("ValueCancel")
                .IsRequired();

            builder.Property<int>("CancellationType")
            .HasDefaultValue("1")
            .IsRequired();
        }
    }
}