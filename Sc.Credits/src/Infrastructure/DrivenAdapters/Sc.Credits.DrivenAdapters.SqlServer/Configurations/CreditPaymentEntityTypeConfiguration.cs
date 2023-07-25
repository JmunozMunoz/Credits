using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc.Credits.Domain.Model.Credits;
using System;

namespace Sc.Credits.DrivenAdapters.SqlServer.Configurations
{
    /// <summary>
    /// Credit payment entity type configuration
    /// </summary>
    internal class CreditPaymentEntityTypeConfiguration
        : IEntityTypeConfiguration<CreditPayment>
    {
        /// <summary>
        ///  <see cref="IEntityTypeConfiguration{TEntity}.Configure(EntityTypeBuilder{TEntity})"/>
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<CreditPayment> builder)
        {
            builder.ToTable("Credits");

            builder.HasKey(b => b.Id);

            builder.Property<decimal>("TotalValuePaid")
                .IsRequired();

            builder.Property<int>("LastFee")
               .IsRequired();

            builder.Property<string>("TransactionTypeName")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property<string>("PaymentTypeName")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property<decimal>("CreditValuePaid")
                .IsRequired();

            builder.Property<decimal>("InterestValuePaid")
                .IsRequired();

            builder.Property<decimal>("ChargeValuePaid")
                .IsRequired();

            builder.Property<decimal>("ArrearsValuePaid")
                .IsRequired();

            builder.Property<decimal>("AssuranceValuePaid")
                .IsRequired();

            builder.Property<DateTime>("LastPaymentDate")
                .HasColumnType("date")
                .IsRequired();

            builder.Property<TimeSpan>("LastPaymentTime")
                .HasColumnType("time")
                .IsRequired();

            builder.Property<DateTime>("DueDate")
                .HasColumnType("date")
                .IsRequired();

            builder.Property<DateTime>("CalculationDate")
                .HasColumnType("date")
                .IsRequired();

            builder.Property<string>("BankAccount")
                .IsRequired(false);

            builder.Property<decimal>("ActiveFeeValuePaid")
                .IsRequired();

            builder.Property<decimal>("PreviousInterest")
                .IsRequired();

            builder.Property<decimal>("PreviousArrears")
                .IsRequired();

            builder.HasOne<Credit>()
                 .WithOne(c => c.CreditPayment)
                 .HasForeignKey<Credit>("Id");

            builder.HasOne<TransactionType>()
                .WithMany()
                .HasForeignKey("TransactionTypeId")
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<PaymentType>()
               .WithMany()
               .HasForeignKey("PaymentTypeId")
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}