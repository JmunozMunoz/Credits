using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc.Credits.Domain.Model.Locations;
using Sc.Credits.Domain.Model.Stores;

namespace Sc.Credits.DrivenAdapters.SqlServer.Configurations
{
    /// <summary>
    /// Store entity type configuration
    /// </summary>
    internal class StoreEntityTypeConfiguration
        : IEntityTypeConfiguration<Store>
    {
        /// <summary>
        ///  <see cref="IEntityTypeConfiguration{TEntity}.Configure(EntityTypeBuilder{TEntity})"/>
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<Store> builder)
        {
            builder.ToTable("Stores");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .HasMaxLength(50);

            builder.Property(b => b.StoreName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property<decimal>("MinimumFee")
                 .IsRequired();

            builder.Property<decimal>("DownPaymentPercentage")
                .IsRequired();

            builder.Property<bool>("MandatoryDownPayment")
                .IsRequired();

            builder.Property<int>("AssuranceType")
                .IsRequired();

            builder.Property<string>("VendorId")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property<int>("MonthLimit")
                .IsRequired();

            builder.Property<decimal>("EffectiveAnnualRate")
                .IsRequired();

            builder.Property<decimal>("AssurancePercentage")
                .IsRequired();

            builder.Property<int>("Status")
                .IsRequired();

            builder.Property<string>("Phone")
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property<bool>("SendTokenMail")
               .IsRequired();

            builder.Property<bool>("SendTokenSms")
               .IsRequired();

            builder.Property<string>("Nit")
               .IsRequired();

            builder.Property<int>("StoreProfileCode")
                .IsRequired();

            builder.Property<bool>("HasRiskCalculation")
                .IsRequired();

            builder.HasOne(s => s.CollectType)
                   .WithMany()
                   .HasForeignKey("CollectTypeId")
                   .IsRequired(true)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.BusinessGroup)
                   .WithMany()
                   .HasForeignKey("BusinessGroupId")
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.AssuranceCompany)
                   .WithMany()
                   .HasForeignKey("AssuranceCompanyId")
                   .IsRequired(true)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.PaymentType)
                   .WithMany()
                   .HasForeignKey("PaymentTypeId")
                   .IsRequired(true)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property<bool>("AllowPromissoryNoteSignature")
                .IsRequired();
            builder.HasOne<State>()
                   .WithMany()
                   .HasForeignKey("StateId")
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<City>()
                   .WithMany()
                   .HasForeignKey("CityId")
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.StoreCategory)
                   .WithMany()
                   .HasForeignKey("StoreCategoryId")
                   .IsRequired(true)
                   .OnDelete(DeleteBehavior.Restrict);


        }
    }
}