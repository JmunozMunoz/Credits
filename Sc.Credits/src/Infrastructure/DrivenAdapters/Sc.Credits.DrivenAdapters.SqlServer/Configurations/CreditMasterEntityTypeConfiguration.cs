using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc.Credits.Domain.Model.Credits;
using System;

namespace Sc.Credits.DrivenAdapters.SqlServer.Configurations
{
    /// <summary>
    /// Credit master entity type configuration
    /// </summary>
    internal class CreditMasterEntityTypeConfiguration
        : IEntityTypeConfiguration<CreditMaster>
    {
        /// <summary>
        ///  <see cref="IEntityTypeConfiguration{TEntity}.Configure(EntityTypeBuilder{TEntity})"/>
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<CreditMaster> builder)
        {
            builder.ToTable("CreditsMaster");

            builder.HasKey(b => b.Id)
                .ForSqlServerIsClustered(false);

            builder.Property<long>("ClusteredId")
                .UseSqlServerIdentityColumn()
                .IsRequired()
                .Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

            builder.HasIndex("ClusteredId")
                .IsUnique()
                .ForSqlServerIsClustered(true);

            builder.Property(c => c.CreateDate)
                    .HasColumnType("date")
                    .IsRequired();

            builder.Property(c => c.CreateTime)
                   .HasColumnType("time")
                   .IsRequired();

            builder.Property(c => c.LastDate)
                .HasColumnType("date")
                .IsRequired();

            builder.Property(c => c.LastTime)
                .HasColumnType("time")
                .IsRequired();

            builder.Property(c => c.LastId)
                .IsRequired();

            builder.Property<long>("CreditNumber")
                .IsRequired();

            builder.Property<string>("ScCode")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property<string>("Seller")
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property<string>("Products")
                .HasColumnType("nvarchar(MAX)")
                .IsRequired(false);

            builder.Property<string>("Invoice")
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property<string>("Reason")
               .HasMaxLength(1000)
               .IsRequired(false);

            builder.Property<DateTime>("CreditDate")
                .HasColumnType("date")
                .IsRequired();

            builder.Property<int>("Month")
                .HasComputedColumnSql("MONTH(CreditDate) PERSISTED");

            builder.Property<TimeSpan>("CreditTime")
                .HasColumnType("time")
                .IsRequired();

            builder.Property<string>("CertifyingAuthority")
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property<string>("CertifiedId")
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property<decimal>("EffectiveAnnualRate")
                .IsRequired();

            builder.Property<string>("Token")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property<string>("PromissoryNoteFileName")
                .HasMaxLength(200)
                .IsRequired(false);

            builder.HasIndex(c => c.LastId)
                .IsUnique();

            builder.HasIndex("CreditDate");

            builder.Property<int>("Hour")
              .HasComputedColumnSql("DATEPART(HOUR, CreditTime) PERSISTED");

            builder.Property<int>("Minute")
              .HasComputedColumnSql("DATEPART(MINUTE, CreditTime) PERSISTED");

            builder.HasIndex("CustomerId", "StoreId", "CreditDate", "Month", "Hour", "Minute")
                .IsUnique();

            builder.HasOne(c => c.Customer)
                .WithMany()
                .IsRequired()
                .HasForeignKey("CustomerId");

            builder.HasOne(c => c.Status)
                .WithMany()
                .IsRequired()
                .HasForeignKey("StatusId");

            builder.HasOne(c => c.Store)
                .WithMany()
                .IsRequired()
                .HasForeignKey("StoreId");

            builder.HasMany(c => c.History)
                .WithOne(c => c.CreditMaster)
                .IsRequired()
                .HasForeignKey("CreditMasterId");

            builder.HasIndex("CreditNumber", "StoreId")
                .IsUnique();

            builder.Property<string>("RiskLevel")
                .HasMaxLength(50)
                .HasDefaultValue("1")
                .IsRequired();
        }
    }
}