using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc.Credits.Domain.Model.Credits;
using System;

namespace Sc.Credits.DrivenAdapters.SqlServer.Configurations
{
    /// <summary>
    /// Credit entity type configuration
    /// </summary>
    internal class CreditEntityTypeConfiguration
        : IEntityTypeConfiguration<Credit>
    {
        /// <summary>
        ///  <see cref="IEntityTypeConfiguration{TEntity}.Configure(EntityTypeBuilder{TEntity})"/>
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<Credit> builder)
        {
            builder.ToTable("Credits");

            builder.HasKey(b => b.Id);

            builder.Property<decimal>("CreditValue")
                    .IsRequired();

            builder.Property<int>("Fees")
                .IsRequired();

            builder.Property<long>("PaymentNumber")
                .IsRequired();

            builder.HasIndex("PaymentNumber", "StoreId")
                   .IsUnique()
                   .HasFilter("[CreateDate] >= '2022-07-06' AND [TransactionTypeId]=2");

            builder.Property<decimal>("FeeValue")
                .IsRequired();

            builder.Property<decimal>("AssurancePercentage")
                .IsRequired();

            builder.Property<decimal>("AssuranceValue")
                .IsRequired();

            builder.Property<decimal>("AssuranceFee")
                .IsRequired();

            builder.Property<decimal>("InterestRate")
                .HasColumnType("decimal(37, 17)")
                .IsRequired();

            builder.Property<int>("Frequency")
                .IsRequired();

            builder.Property<decimal>("Balance")
               .IsRequired();

            builder.Property<DateTime>("CreateDate")
               .HasColumnType("date")
               .IsRequired();

            builder.Property<TimeSpan>("CreateTime")
                .HasColumnType("time")
               .IsRequired();

            builder.Property<DateTime>("TransactionDate")
                .HasColumnType("date")
               .IsRequired();

            builder.Property<TimeSpan>("TransactionTime")
                .HasColumnType("time")
               .IsRequired();

            builder.Property<string>("UserId")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property<string>("UserName")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property<string>("StatusName")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property<string>("SourceName")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property<string>("AuthMethodName")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property<string>("Location")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property<long>("CreditNumber")
                .IsRequired();

            builder.Property<bool>("AlternatePayment")
               .IsRequired();

            builder.Property<bool>("HasArrearsCharge")
                .IsRequired();

            builder.Property<decimal?>("ArrearsCharge")
               .IsRequired(false);

            builder.Property<decimal?>("ChargeValue")
               .IsRequired(false);

            builder.Property<decimal?>("UpdatedPaymentPlanValue")
               .IsRequired(false);

            builder.Property<int?>("ArrearsDays")
                .IsRequired(false);

            builder.Property<int>("ComputedArrearsDays")
                .HasComputedColumnSql("(CASE WHEN DATEDIFF(DAY, [LastPaymentDate], GETDATE()) <= 0 THEN 0 ELSE DATEDIFF(DAY, [LastPaymentDate], GETDATE()) END)");

            builder.Property<decimal>("AssuranceBalance")
                .IsRequired();

            builder.Property<decimal>("AssuranceTotalValue")
                .IsRequired();

            builder.Property<decimal>("DownPayment")
               .IsRequired();

            builder.Property<decimal>("TotalDownPayment")
               .IsRequired();

            builder.Property<decimal>("AssuranceTotalFeeValue")
               .IsRequired();

            builder.HasIndex("TransactionDate");

            builder.Property<int>("TransactionHour")
                .HasComputedColumnSql("DATEPART(HOUR, TransactionTime) PERSISTED");

            builder.Property<int>("TransactionMinute")
                .HasComputedColumnSql("DATEPART(MINUTE, TransactionTime) PERSISTED");

            builder.HasIndex("CreditMasterId", "TransactionDate", "TransactionHour", "TransactionMinute")
                .IsUnique()
                .HasFilter("[TransactionTypeId] = 2 AND [TransactionDate] > '2020-02-18'");

            builder.HasOne(c => c.Store)
                       .WithMany()
                       .IsRequired()
                       .HasForeignKey("StoreId");

            builder.HasOne(c => c.Customer)
                .WithMany()
                .IsRequired()
                .HasForeignKey("CustomerId");

            builder.HasOne(c => c.Status)
                .WithMany()
                .IsRequired()
                .HasForeignKey("StatusId");

            builder.HasOne(c => c.Source)
                .WithMany()
                .IsRequired()
                .HasForeignKey("SourceId");

            builder.HasOne(c => c.AuthMethod)
                .WithMany()
                .IsRequired()
                .HasForeignKey("AuthMethodId");
        }
    }
}