using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc.Credits.Domain.Model.Customers;
using System;

namespace Sc.Credits.DrivenAdapters.SqlServer.Configurations
{
    /// <summary>
    /// Customer entity type configuration
    /// </summary>
    internal class CustomerEntityTypeConfiguration
        : IEntityTypeConfiguration<Customer>
    {
        /// <summary>
        ///  <see cref="IEntityTypeConfiguration{TEntity}.Configure(EntityTypeBuilder{TEntity})"/>
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(b => b.Id);

            builder.Property(c => c.Id)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.IdDocument)
                            .HasMaxLength(50)
                            .IsRequired();

            builder.Property(c => c.DocumentType)
                .HasMaxLength(20)
                .IsRequired();

            builder.HasIndex(c => new { c.IdDocument, c.DocumentType })
                .IsUnique();

            builder.Property<decimal>("CreditLimit")
                .IsRequired();

            builder.Property<decimal>("AvailableCreditLimit")
                .IsRequired();

            builder.Property<bool>("ValidatedMail")
                .IsRequired();

            builder.Property<DateTime>("CreateDate")
                .HasColumnType("date")
                .IsRequired();

            builder.Property<TimeSpan>("CreateTime")
                .HasColumnType("time")
                .IsRequired();

            builder.Property<DateTime>("UpdateDate")
                .HasColumnType("date")
                .IsRequired();

            builder.Property<TimeSpan>("UpdateTime")
                .HasColumnType("time")
                .IsRequired();

            builder.Property<string>("FullName")
               .HasMaxLength(200)
               .IsRequired();

            builder.OwnsOne(c => c.Name)
                .Property<string>("FirstName")
                .HasMaxLength(50)
                .IsRequired();

            builder.OwnsOne(c => c.Name)
                .Property<string>("SecondName")
                .HasMaxLength(50)
                .IsRequired();

            builder.OwnsOne(c => c.Name)
                .Property<string>("FirstLastName")
                .HasMaxLength(50)
                .IsRequired();

            builder.OwnsOne(c => c.Name)
                .Property<string>("SecondLastName")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property<int>("ProfileId")
                .IsRequired();

            builder.Property<string>("Mobile")
               .HasMaxLength(20)
               .IsRequired();

            builder.Property<string>("Email")
               .HasMaxLength(100)
               .IsRequired();

            builder.Property<bool>("SendTokenMail")
                .IsRequired();

            builder.Property<bool>("SendTokenSms")
                .IsRequired();

            builder.Property<int>("Status")
                .IsRequired();

            builder.Property<DateTime>("CreditLimitUpdateDate")
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Ignore("CreditLimitUpdated");

            builder.HasOne(c => c.Profile)
                .WithMany()
                .IsRequired()
                .HasForeignKey("ProfileId");
        }
    }
}