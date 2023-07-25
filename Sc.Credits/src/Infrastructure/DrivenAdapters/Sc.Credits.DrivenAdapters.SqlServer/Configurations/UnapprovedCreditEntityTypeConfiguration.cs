using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc.Credits.Domain.Model.Credits;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.DrivenAdapters.SqlServer.Configurations
{
    internal class UnapprovedCreditEntityTypeConfiguration
         : IEntityTypeConfiguration<UnapprovedCredit>
    {
        public void Configure(EntityTypeBuilder<UnapprovedCredit> builder)
        {

            builder.ToTable("UnapprovedCredits");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.CustomerId)
                   .IsRequired();

            builder.Property(c => c.StoreId)
                       .IsRequired();

            builder.Property<DateTime>("TransactionDate")
             .HasColumnType("date")
             .IsRequired();

            builder.HasIndex("TransactionDate");

            builder.Property<TimeSpan>("TransactionTime")
                .HasColumnType("time")
               .IsRequired();

            builder.Property<decimal>("CreditValue")
                    .IsRequired();
        }
    }
}
