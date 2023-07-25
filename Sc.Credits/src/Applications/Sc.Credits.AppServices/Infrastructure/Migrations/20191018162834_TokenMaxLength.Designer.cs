﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sc.Credits.DrivenAdapters.SqlServer;

namespace Sc.Credits.Applications.AppServices.Infrastructure.Migrations
{
    [DbContext(typeof(CreditsContext))]
    [Migration("20191018162834_TokenMaxLength")]
    partial class TokenMaxLength
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("Relational:Sequence:.AssuranceCompaniesSeq", "'AssuranceCompaniesSeq', '', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("Relational:Sequence:.AuthMethodsSeq", "'AuthMethodsSeq', '', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("Relational:Sequence:.BusinessGroupSeq", "'BusinessGroupSeq', '', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("Relational:Sequence:.CollectTypesSeq", "'CollectTypesSeq', '', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("Relational:Sequence:.ParamtersSeq", "'ParamtersSeq', '', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("Relational:Sequence:.PaymentTypesSeq", "'PaymentTypesSeq', '', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("Relational:Sequence:.ProfilesSeq", "'ProfilesSeq', '', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("Relational:Sequence:.RequestStatusSeq", "'RequestStatusSeq', '', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("Relational:Sequence:.SourcesSeq", "'SourcesSeq', '', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("Relational:Sequence:.StatusSeq", "'StatusSeq', '', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("Relational:Sequence:.StoreIdentificationsSeq", "'StoreIdentificationsSeq', '', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("Relational:Sequence:.TransactionTypesSeq", "'TransactionTypesSeq', '', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Sc.Credits.Domain.Model.AuthMethods.AuthMethod", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "AuthMethodsSeq")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("AuthMethods");
                });

            modelBuilder.Entity("Sc.Credits.Domain.Model.Credits.Credit", b =>
                {
                    b.Property<Guid>("Id");

                    b.Property<bool>("AlternatePayment");

                    b.Property<decimal?>("ArrearsCharge")
                        .HasColumnType("decimal(26, 6)");

                    b.Property<int?>("ArrearsDays");

                    b.Property<decimal>("AssuranceBalance")
                        .HasColumnType("decimal(26, 6)");

                    b.Property<decimal>("AssuranceFee")
                        .HasColumnType("decimal(26, 6)");

                    b.Property<decimal>("AssurancePercentage")
                        .HasColumnType("decimal(26, 6)");

                    b.Property<decimal>("AssuranceTotalFeeValue")
                        .HasColumnType("decimal(26, 6)");

                    b.Property<decimal>("AssuranceTotalValue")
                        .HasColumnType("decimal(26, 6)");

                    b.Property<decimal>("AssuranceValue")
                        .HasColumnType("decimal(26, 6)");

                    b.Property<int>("AuthMethodId");

                    b.Property<string>("AuthMethodName")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<decimal>("Balance")
                        .HasColumnType("decimal(26, 6)");

                    b.Property<decimal?>("ChargeValue")
                        .HasColumnType("decimal(26, 6)");

                    b.Property<int>("ComputedArrearsDays")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasComputedColumnSql("(CASE WHEN DATEDIFF(DAY,[DueDate],GETDATE()) <= 0 THEN 0 ELSE DATEDIFF(DAY,[DueDate],GETDATE()) END)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("date");

                    b.Property<TimeSpan>("CreateTime")
                        .HasColumnType("time");

                    b.Property<Guid?>("CreditMasterId");

                    b.Property<long>("CreditNumber");

                    b.Property<decimal>("CreditValue")
                        .HasColumnType("decimal(26, 6)");

                    b.Property<Guid>("CustomerId");

                    b.Property<decimal>("DownPayment")
                        .HasColumnType("decimal(26, 6)");

                    b.Property<decimal>("FeeValue")
                        .HasColumnType("decimal(26, 6)");

                    b.Property<int>("Fees");

                    b.Property<int>("Frequency");

                    b.Property<bool>("HasUpdatedPaymentPlan");

                    b.Property<decimal>("InterestRate")
                        .HasColumnType("decimal(37, 17)");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Products")
                        .HasColumnType("nvarchar(MAX)");

                    b.Property<int>("SourceId");

                    b.Property<string>("SourceName")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<int>("StatusId");

                    b.Property<string>("StatusName")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("StoreId")
                        .IsRequired();

                    b.Property<decimal>("TotalDownPayment")
                        .HasColumnType("decimal(26, 6)");

                    b.Property<DateTime>("TransactionDate")
                        .HasColumnType("date");

                    b.Property<TimeSpan>("TransactionTime")
                        .HasColumnType("time");

                    b.Property<decimal?>("UpdatedPaymentPlanValue")
                        .HasColumnType("decimal(26, 6)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("AuthMethodId");

                    b.HasIndex("CreditMasterId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("SourceId");

                    b.HasIndex("StatusId");

                    b.HasIndex("StoreId");

                    b.ToTable("Credits");
                });

            modelBuilder.Entity("Sc.Credits.Domain.Model.Credits.CreditMaster", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CertifiedId")
                        .HasMaxLength(200);

                    b.Property<string>("CertifyingAuthority")
                        .HasMaxLength(200);

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("date");

                    b.Property<TimeSpan>("CreateTime")
                        .HasColumnType("time");

                    b.Property<DateTime>("CreditDate")
                        .HasColumnType("date");

                    b.Property<long>("CreditNumber");

                    b.Property<TimeSpan>("CreditTime")
                        .HasColumnType("time");

                    b.Property<Guid>("CustomerId");

                    b.Property<decimal>("EffectiveAnnualRate")
                        .HasColumnType("decimal(26, 6)");

                    b.Property<string>("Invoice")
                        .HasMaxLength(200);

                    b.Property<DateTime>("LastDate")
                        .HasColumnType("date");

                    b.Property<Guid>("LastId");

                    b.Property<TimeSpan>("LastTime")
                        .HasColumnType("time");

                    b.Property<string>("Products")
                        .HasColumnType("nvarchar(MAX)");

                    b.Property<string>("PromissoryNoteFileName")
                        .HasMaxLength(200);

                    b.Property<string>("Reason")
                        .HasMaxLength(1000);

                    b.Property<string>("ScCode")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<string>("Seller")
                        .HasMaxLength(200);

                    b.Property<int>("StatusId");

                    b.Property<string>("StoreId")
                        .IsRequired();

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.HasIndex("LastId")
                        .IsUnique();

                    b.HasIndex("StatusId");

                    b.HasIndex("StoreId");

                    b.HasIndex("CreditNumber", "StoreId")
                        .IsUnique();

                    b.ToTable("CreditsMaster");
                });

            modelBuilder.Entity("Sc.Credits.Domain.Model.Credits.CreditPayment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("ArrearsValuePaid")
                        .HasColumnType("decimal(26, 6)");

                    b.Property<decimal>("AssuranceValuePaid")
                        .HasColumnType("decimal(26, 6)");

                    b.Property<string>("BankAccount");

                    b.Property<DateTime>("CalculationDate")
                        .HasColumnType("date");

                    b.Property<decimal>("ChargeValuePaid")
                        .HasColumnType("decimal(26, 6)");

                    b.Property<decimal>("CreditValuePaid")
                        .HasColumnType("decimal(26, 6)");

                    b.Property<DateTime>("DueDate")
                        .HasColumnType("date");

                    b.Property<decimal>("InterestValuePaid")
                        .HasColumnType("decimal(26, 6)");

                    b.Property<int>("LastFee");

                    b.Property<DateTime>("LastPaymentDate")
                        .HasColumnType("date");

                    b.Property<TimeSpan>("LastPaymentTime")
                        .HasColumnType("time");

                    b.Property<long>("PaymentNumber");

                    b.Property<int>("PaymentTypeId");

                    b.Property<string>("PaymentTypeName")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<decimal>("TotalValuePaid")
                        .HasColumnType("decimal(26, 6)");

                    b.Property<int>("TransactionTypeId");

                    b.Property<string>("TransactionTypeName")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("PaymentTypeId");

                    b.HasIndex("TransactionTypeId");

                    b.ToTable("Credits");
                });

            modelBuilder.Entity("Sc.Credits.Domain.Model.Credits.PaymentType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "PaymentTypesSeq")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("PaymentTypes");
                });

            modelBuilder.Entity("Sc.Credits.Domain.Model.Credits.RequestCancelCredit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CreditMasterId");

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<DateTime?>("ProcessDate")
                        .HasColumnType("date");

                    b.Property<TimeSpan?>("ProcessTime")
                        .HasColumnType("time");

                    b.Property<string>("ProcessUserId")
                        .HasMaxLength(100);

                    b.Property<string>("ProcessUserName")
                        .HasMaxLength(100);

                    b.Property<string>("Reason")
                        .IsRequired();

                    b.Property<int>("RequestStatusId");

                    b.Property<string>("StoreId")
                        .IsRequired();

                    b.Property<TimeSpan>("Time")
                        .HasColumnType("time");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("CreditMasterId");

                    b.HasIndex("RequestStatusId");

                    b.HasIndex("StoreId");

                    b.ToTable("RequestCancelCredits");
                });

            modelBuilder.Entity("Sc.Credits.Domain.Model.Credits.RequestCancelPayment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("CreditCancelId");

                    b.Property<Guid>("CreditId");

                    b.Property<Guid>("CreditMasterId");

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<DateTime?>("ProcessDate")
                        .HasColumnType("date");

                    b.Property<TimeSpan?>("ProcessTime")
                        .HasColumnType("time");

                    b.Property<string>("ProcessUserId")
                        .HasMaxLength(100);

                    b.Property<string>("ProcessUserName")
                        .HasMaxLength(100);

                    b.Property<string>("Reason")
                        .IsRequired();

                    b.Property<int>("RequestStatusId");

                    b.Property<string>("StoreId")
                        .IsRequired();

                    b.Property<TimeSpan>("Time")
                        .HasColumnType("time");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("CreditCancelId");

                    b.HasIndex("CreditId");

                    b.HasIndex("CreditMasterId");

                    b.HasIndex("RequestStatusId");

                    b.HasIndex("StoreId");

                    b.ToTable("RequestCancelPayments");
                });

            modelBuilder.Entity("Sc.Credits.Domain.Model.Credits.RequestStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "RequestStatusSeq")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("RequestStatus");
                });

            modelBuilder.Entity("Sc.Credits.Domain.Model.Credits.Source", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "SourcesSeq")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("Sources");
                });

            modelBuilder.Entity("Sc.Credits.Domain.Model.Credits.Status", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "StatusSeq")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("Status");
                });

            modelBuilder.Entity("Sc.Credits.Domain.Model.Credits.TransactionType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "TransactionTypesSeq")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("TransactionTypes");
                });

            modelBuilder.Entity("Sc.Credits.Domain.Model.Customers.Customer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50);

                    b.Property<decimal>("AvailableCreditLimit")
                        .HasColumnType("decimal(26, 6)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("date");

                    b.Property<TimeSpan>("CreateTime")
                        .HasColumnType("time");

                    b.Property<decimal>("CreditLimit")
                        .HasColumnType("decimal(26, 6)");

                    b.Property<string>("DocumentType")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("IdDocument")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Mobile")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<int>("ProfileId");

                    b.Property<bool>("SendTokenMail");

                    b.Property<bool>("SendTokenSms");

                    b.Property<int>("Status");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("date");

                    b.Property<TimeSpan>("UpdateTime")
                        .HasColumnType("time");

                    b.Property<bool>("ValidatedMail");

                    b.HasKey("Id");

                    b.HasIndex("ProfileId");

                    b.HasIndex("IdDocument", "DocumentType")
                        .IsUnique();

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("Sc.Credits.Domain.Model.Customers.Profile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "ProfilesSeq")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<decimal>("AssurancePercentage")
                        .HasColumnType("decimal(26, 6)");

                    b.Property<decimal>("EffectiveAnnualRate")
                        .HasColumnType("decimal(26, 6)");

                    b.Property<int>("MandatoryDownPayment");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("Profiles");
                });

            modelBuilder.Entity("Sc.Credits.Domain.Model.Parameters.Parameter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "ParamtersSeq")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("Parameters");
                });

            modelBuilder.Entity("Sc.Credits.Domain.Model.Sequences.Sequence", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("LastNumber");

                    b.Property<string>("StoreId")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("StoreId", "Type")
                        .IsUnique();

                    b.ToTable("Sequences");
                });

            modelBuilder.Entity("Sc.Credits.Domain.Model.Stores.AssuranceCompany", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "AssuranceCompaniesSeq")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("AssuranceCompanies");
                });

            modelBuilder.Entity("Sc.Credits.Domain.Model.Stores.BusinessGroup", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "BusinessGroupSeq")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("BusinessGroup");
                });

            modelBuilder.Entity("Sc.Credits.Domain.Model.Stores.CollectType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "CollectTypesSeq")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("CollectTypes");
                });

            modelBuilder.Entity("Sc.Credits.Domain.Model.Stores.Store", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50);

                    b.Property<long>("AssuranceCompanyId");

                    b.Property<int>("AssuranceType");

                    b.Property<long?>("BusinessGroupId");

                    b.Property<int>("CollectTypeId");

                    b.Property<decimal>("DownPaymentPercentage")
                        .HasColumnType("decimal(26, 6)");

                    b.Property<bool>("MandatoryDownPayment");

                    b.Property<decimal>("MinimumFee")
                        .HasColumnType("decimal(26, 6)");

                    b.Property<int>("PaymentTypeId");

                    b.Property<string>("StoreName")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("VendorId")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("AssuranceCompanyId");

                    b.HasIndex("BusinessGroupId");

                    b.HasIndex("CollectTypeId");

                    b.HasIndex("PaymentTypeId");

                    b.ToTable("Stores");
                });

            modelBuilder.Entity("Sc.Credits.Domain.Model.Stores.StoreIdentification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "StoreIdentificationsSeq")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<string>("ScCode")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.Property<string>("StoreId")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("StoreIdentifications");
                });

            modelBuilder.Entity("Sc.Credits.Domain.Model.Credits.Credit", b =>
                {
                    b.HasOne("Sc.Credits.Domain.Model.AuthMethods.AuthMethod", "AuthMethod")
                        .WithMany()
                        .HasForeignKey("AuthMethodId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Sc.Credits.Domain.Model.Credits.CreditMaster")
                        .WithMany("History")
                        .HasForeignKey("CreditMasterId");

                    b.HasOne("Sc.Credits.Domain.Model.Customers.Customer", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Sc.Credits.Domain.Model.Credits.CreditPayment", "CreditPayment")
                        .WithOne()
                        .HasForeignKey("Sc.Credits.Domain.Model.Credits.Credit", "Id")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Sc.Credits.Domain.Model.Credits.Source", "Source")
                        .WithMany()
                        .HasForeignKey("SourceId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Sc.Credits.Domain.Model.Credits.Status", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Sc.Credits.Domain.Model.Stores.Store", "Store")
                        .WithMany()
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Sc.Credits.Domain.Model.Credits.CreditMaster", b =>
                {
                    b.HasOne("Sc.Credits.Domain.Model.Customers.Customer", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Sc.Credits.Domain.Model.Credits.Status", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Sc.Credits.Domain.Model.Stores.Store", "Store")
                        .WithMany()
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Sc.Credits.Domain.Model.Credits.CreditPayment", b =>
                {
                    b.HasOne("Sc.Credits.Domain.Model.Credits.PaymentType")
                        .WithMany()
                        .HasForeignKey("PaymentTypeId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Sc.Credits.Domain.Model.Credits.TransactionType")
                        .WithMany()
                        .HasForeignKey("TransactionTypeId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Sc.Credits.Domain.Model.Credits.RequestCancelCredit", b =>
                {
                    b.HasOne("Sc.Credits.Domain.Model.Credits.CreditMaster", "CreditMaster")
                        .WithMany()
                        .HasForeignKey("CreditMasterId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Sc.Credits.Domain.Model.Credits.RequestStatus")
                        .WithMany()
                        .HasForeignKey("RequestStatusId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Sc.Credits.Domain.Model.Stores.Store", "Store")
                        .WithMany()
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Sc.Credits.Domain.Model.Credits.RequestCancelPayment", b =>
                {
                    b.HasOne("Sc.Credits.Domain.Model.Credits.Credit")
                        .WithMany()
                        .HasForeignKey("CreditCancelId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Sc.Credits.Domain.Model.Credits.Credit", "Credit")
                        .WithMany()
                        .HasForeignKey("CreditId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Sc.Credits.Domain.Model.Credits.CreditMaster", "CreditMaster")
                        .WithMany()
                        .HasForeignKey("CreditMasterId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Sc.Credits.Domain.Model.Credits.RequestStatus")
                        .WithMany()
                        .HasForeignKey("RequestStatusId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Sc.Credits.Domain.Model.Stores.Store", "Store")
                        .WithMany()
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Sc.Credits.Domain.Model.Customers.Customer", b =>
                {
                    b.HasOne("Sc.Credits.Domain.Model.Customers.Profile", "Profile")
                        .WithMany()
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Sc.Credits.Domain.Model.Stores.Store", b =>
                {
                    b.HasOne("Sc.Credits.Domain.Model.Stores.AssuranceCompany", "AssuranceCompany")
                        .WithMany()
                        .HasForeignKey("AssuranceCompanyId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Sc.Credits.Domain.Model.Stores.BusinessGroup", "BusinessGroup")
                        .WithMany()
                        .HasForeignKey("BusinessGroupId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Sc.Credits.Domain.Model.Stores.CollectType", "CollectType")
                        .WithMany()
                        .HasForeignKey("CollectTypeId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Sc.Credits.Domain.Model.Credits.PaymentType", "PaymentType")
                        .WithMany()
                        .HasForeignKey("PaymentTypeId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
