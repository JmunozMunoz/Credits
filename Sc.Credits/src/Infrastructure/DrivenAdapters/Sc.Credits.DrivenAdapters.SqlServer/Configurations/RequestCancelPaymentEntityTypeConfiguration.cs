using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc.Credits.Domain.Model.Credits;
using System;

namespace Sc.Credits.DrivenAdapters.SqlServer.Configurations
{
    /// <summary>
    /// Request cancel payment entity type configuration
    /// </summary>
    internal class RequestCancelPaymentEntityTypeConfiguration
        : IEntityTypeConfiguration<RequestCancelPayment>
    {
        /// <summary>
        ///  <see cref="IEntityTypeConfiguration{TEntity}.Configure(EntityTypeBuilder{TEntity})"/>
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<RequestCancelPayment> builder)
        {
            builder.ToTable("RequestCancelPayments");

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

            builder.HasOne(item => item.RequestStatus)
                .WithMany()
                .HasForeignKey("RequestStatusId")
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(item => item.CreditMaster)
               .WithMany()
               .HasForeignKey("CreditMasterId")
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(item => item.Payment)
                .WithMany()
                .HasForeignKey("CreditId")
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(item => item.CanceledPayment)
                .WithMany()
                .HasForeignKey("CreditCancelId")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(item => item.Store)
                .WithMany()
                .HasForeignKey("StoreId")
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}