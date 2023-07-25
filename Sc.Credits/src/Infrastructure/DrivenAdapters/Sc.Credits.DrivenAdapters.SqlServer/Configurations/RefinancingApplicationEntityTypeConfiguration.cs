using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc.Credits.Domain.Model.Refinancings;
using System;

namespace Sc.Credits.DrivenAdapters.SqlServer.Configurations
{
    /// <summary>
    /// Refinancing application entity type configuration
    /// </summary>
    internal class RefinancingApplicationEntityTypeConfiguration
        : IEntityTypeConfiguration<RefinancingApplication>
    {
        /// <summary>
        ///  <see cref="IEntityTypeConfiguration{TEntity}.Configure(EntityTypeBuilder{TEntity})"/>
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<RefinancingApplication> builder)
        {
            builder.ToTable("RefinancingApplications");

            builder.HasKey(c => c.Id);

            builder.Property<string>("Name")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property<DateTime>("CreationDateFrom")
                .HasColumnType("date")
                .IsRequired();

            builder.Property<DateTime>("CreationDateTo")
                .HasColumnType("date")
                .IsRequired();

            builder.Property<int>("ArrearsDaysFrom")
                .IsRequired();

            builder.Property<int>("ArrearsDaysTo")
                .IsRequired();

            builder.Property<decimal>("ValueFrom")
                .IsRequired();

            builder.Property<decimal>("ValueTo")
                .IsRequired();

            builder.Property<bool>("AllowRefinancingCredits")
                .IsRequired();
        }
    }
}