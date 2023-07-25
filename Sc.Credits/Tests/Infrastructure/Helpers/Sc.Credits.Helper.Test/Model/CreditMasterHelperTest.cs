namespace Sc.Credits.Helper.Test.Model
{
    using Domain.Model.Credits;
    using Sc.Credits.Domain.Model.Base;
    using Sc.Credits.Domain.Model.Common;
    using Sc.Credits.Domain.Model.Credits.Builders;
    using Sc.Credits.Domain.Model.Credits.Events;
    using Sc.Credits.Domain.Model.Enums;
    using Sc.Credits.Domain.Model.Stores;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Credit Master Helper Test
    /// </summary>
    public static class CreditMasterHelperTest
    {
        /// <summary>
        /// Get Credit Master List
        /// </summary>
        /// <returns></returns>
        public static List<CreditMaster> GetCreditMasterList()
        {
            return new List<CreditMaster>
            {
                BuilderDefault(SourceHelperTest.GetCredinetSource(), customerId: new Guid("527af1e7-bcbf-4559-bc38-054f785976f5"),
                    creditValue: 500000, creditNumber: 1, interestRate: 0.1M, effectiveAnnualRate: 0.283200M,
                    creditDate: DateTime.Now, StoreHelperTest.GetStore(), fees: 15, Frequencies.Monthly,
                    feeValue: 30000, StatusHelperTest.GetActiveStatus(), AuthMethodHelperTest.GetTokenAuthMethod(),
                    downPayment: 50000, totalDownPayment: 52300, TransactionTypeHelperTest.GetCreateCreditType(),
                    PaymentTypeHelperTest.GetOrdinaryPaymentType(), dueDate: DateTime.Today, assurancePercentage: 0.1M,
                    assuranceValue: 40000, assuranceFee: 2000, assuranceTotalFeeValue: 2300,
                    assuranceTotalValue: 47600)
                .Build(),
                BuilderDefault(SourceHelperTest.GetCredinetSource(), customerId: new Guid("527af1e7-bcbf-4559-bc38-054f785976f5"),
                    creditValue: 200000, creditNumber: 2, interestRate: 0.0209971486861902M, effectiveAnnualRate: 0.283200M,
                    creditDate: DateTime.Now, StoreHelperTest.GetStore(), fees: 10, Frequencies.Monthly,
                    feeValue: 20591.0M, StatusHelperTest.GetActiveStatus(), AuthMethodHelperTest.GetTokenAuthMethod(),
                    downPayment: 20000, totalDownPayment: 22300, TransactionTypeHelperTest.GetCreateCreditType(),
                    PaymentTypeHelperTest.GetOrdinaryPaymentType(), dueDate: DateTime.Today, assurancePercentage: 0.1M,
                    assuranceValue: 23800.0M, assuranceFee: 345.0M,  assuranceTotalFeeValue: 2300,
                    assuranceTotalValue: 28322)
                .Build(),
                BuilderDefault(SourceHelperTest.GetCredinetSource(), customerId: new Guid("527af1e7-bcbf-4559-bc38-054f785976f5"),
                    creditValue: 500000, creditNumber: 3, interestRate: 0.1M, effectiveAnnualRate: 0.283200M,
                    creditDate: DateTime.Now, StoreHelperTest.GetStore(), fees: 15, Frequencies.Monthly,
                    feeValue: 30000, StatusHelperTest.GetActiveStatus(), AuthMethodHelperTest.GetTokenAuthMethod(),
                    downPayment: 50000, totalDownPayment: 52300, TransactionTypeHelperTest.GetCreateCreditType(),
                    PaymentTypeHelperTest.GetOrdinaryPaymentType(), dueDate: DateTime.Today, assurancePercentage: 0.1M,
                    assuranceValue: 40000, assuranceFee: 2000,  assuranceTotalFeeValue: 2300,
                    assuranceTotalValue: 47600)
                .Build()
                .SetId(default)
                .SetCreditId(default)
            };
        }

        /// <summary>
        /// Get credit master List with same customerId
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public static List<CreditMaster> GetCreditMasterList(Guid customerId)
        {
            List<CreditMaster> creditMasterList = new List<CreditMaster>
            {
                BuilderDefault(SourceHelperTest.GetCredinetSource(), customerId,
                    creditValue: 500000, creditNumber: 1, interestRate: 0.1M, effectiveAnnualRate: 0.283200M,
                    creditDate: new DateTime(2019, 06, 15, 11, 11, 11), StoreHelperTest.GetStores()[1], fees: 15, Frequencies.Monthly,
                    feeValue: 30000, StatusHelperTest.GetActiveStatus(), AuthMethodHelperTest.GetTokenAuthMethod(),
                    downPayment: 50000, totalDownPayment: 52300, TransactionTypeHelperTest.GetCreateCreditType(),
                    PaymentTypeHelperTest.GetOrdinaryPaymentType(), dueDate: DateTime.Today, assurancePercentage: 0.1M,
                    assuranceValue: 40000, assuranceFee: 2000, assuranceTotalFeeValue: 2300,
                    assuranceTotalValue: 47600)
                .Build(),
                BuilderDefault(SourceHelperTest.GetCredinetSource(), customerId: new Guid("527af1e7-bcbf-4559-bc38-054f785976f5"),
                    creditValue: 200000, creditNumber: 2, interestRate: 0.0209971486861902M, effectiveAnnualRate: 0.283200M,
                    creditDate: new DateTime(2019, 05, 15, 10, 10, 10), StoreHelperTest.GetStores()[1], fees: 10, Frequencies.Monthly,
                    feeValue: 20591.0M, StatusHelperTest.GetActiveStatus(), AuthMethodHelperTest.GetTokenAuthMethod(),
                    downPayment: 20000, totalDownPayment: 22300, TransactionTypeHelperTest.GetCreateCreditType(),
                    PaymentTypeHelperTest.GetOrdinaryPaymentType(), dueDate: DateTime.Now.AddDays(30), assurancePercentage: 0.1M,
                    assuranceValue: 23800.0M, assuranceFee: 345.0M,  assuranceTotalFeeValue: 2300,
                    assuranceTotalValue: 28322)
                .Build(),
                BuilderDefault(SourceHelperTest.GetCredinetSource(), customerId,
                    creditValue: 500000, creditNumber: 1, interestRate: 0.1M, effectiveAnnualRate: 0.283200M,
                    creditDate: new DateTime(2019, 06, 01, 04, 11, 11), StoreHelperTest.GetStores()[1], fees: 15, Frequencies.Monthly,
                    feeValue: 30000, StatusHelperTest.GetCancelRequestStatus(), AuthMethodHelperTest.GetTokenAuthMethod(),
                    downPayment: 50000, totalDownPayment: 52300, TransactionTypeHelperTest.GetCreateCreditType(),
                    PaymentTypeHelperTest.GetOrdinaryPaymentType(), dueDate: DateTime.Today, assurancePercentage: 0.1M,
                    assuranceValue: 40000, assuranceFee: 2000, assuranceTotalFeeValue: 2300,
                    assuranceTotalValue: 47600)
                .Build(),
                BuilderDefault(SourceHelperTest.GetCredinetSource(), customerId,
                    creditValue: 500000, creditNumber: 1, interestRate: 0.1M, effectiveAnnualRate: 0.283200M,
                    creditDate: new DateTime(2019, 06, 21, 06, 11, 11), StoreHelperTest.GetStores()[1], fees: 15, Frequencies.Monthly,
                    feeValue: 30000, StatusHelperTest.GetCancelRequestStatus(), AuthMethodHelperTest.GetTokenAuthMethod(),
                    downPayment: 50000, totalDownPayment: 52300, TransactionTypeHelperTest.GetCreateCreditType(),
                    PaymentTypeHelperTest.GetOrdinaryPaymentType(), dueDate: DateTime.Today, assurancePercentage: 0.1M,
                    assuranceValue: 40000, assuranceFee: 2000, assuranceTotalFeeValue: 2300,
                    assuranceTotalValue: 47600)
                .Build(),
                BuilderDefault(SourceHelperTest.GetCredinetSource(), customerId,
                    creditValue: 500000, creditNumber: 1, interestRate: 0.1M, effectiveAnnualRate: 0.283200M,
                    creditDate: new DateTime(2019, 05, 17, 05, 11, 11), StoreHelperTest.GetStores()[1], fees: 15, Frequencies.Monthly,
                    feeValue: 30000, StatusHelperTest.GetCancelRequestStatus(), AuthMethodHelperTest.GetTokenAuthMethod(),
                    downPayment: 50000, totalDownPayment: 52300, TransactionTypeHelperTest.GetCreateCreditType(),
                    PaymentTypeHelperTest.GetOrdinaryPaymentType(), dueDate: DateTime.Today, assurancePercentage: 0.1M,
                    assuranceValue: 40000, assuranceFee: 2000, assuranceTotalFeeValue: 2300,
                    assuranceTotalValue: 47600)
                .Build(),
                BuilderDefault(SourceHelperTest.GetCredinetSource(), customerId,
                    creditValue: 500000, creditNumber: 1, interestRate: 0.1M, effectiveAnnualRate: 0.283200M,
                    creditDate: new DateTime(2019, 06, 20, 03, 11, 11), StoreHelperTest.GetStores()[1], fees: 15, Frequencies.Monthly,
                    feeValue: 30000, StatusHelperTest.GetCancelRequestStatus(), AuthMethodHelperTest.GetTokenAuthMethod(),
                    downPayment: 50000, totalDownPayment: 52300, TransactionTypeHelperTest.GetCreateCreditType(),
                    PaymentTypeHelperTest.GetOrdinaryPaymentType(), dueDate: DateTime.Today, assurancePercentage: 0.1M,
                    assuranceValue: 40000, assuranceFee: 2000, assuranceTotalFeeValue: 2300,
                    assuranceTotalValue: 47600)
                .Build()
            };

            return creditMasterList;
        }

        /// <summary>
        /// Get Credit Master Canceled List
        /// </summary>
        /// <returns></returns>
        public static List<CreditMaster> GetCreditMasterCanceledList()
        {
            return GetCreditMasterCanceledList(Guid.NewGuid());
        }

        /// <summary>
        /// Get Credit Master Canceled List with same customerId
        /// </summary>
        /// <returns></returns>
        public static List<CreditMaster> GetCreditMasterCanceledList(Guid customerId)
        {
            return new List<CreditMaster>
            {
                BuilderDefault(SourceHelperTest.GetCredinetSource(), customerId,
                    creditValue: 500000, creditNumber: 1, interestRate: 0.1M, effectiveAnnualRate: 0.283200M,
                    creditDate: DateTime.Now, StoreHelperTest.GetStore(), fees: 15, Frequencies.Monthly,
                    feeValue: 30000, StatusHelperTest.GetCalceledStatus(), AuthMethodHelperTest.GetTokenAuthMethod(),
                    downPayment: 50000, totalDownPayment: 52300, TransactionTypeHelperTest.GetCreateCreditType(),
                    PaymentTypeHelperTest.GetOrdinaryPaymentType(), dueDate: DateTime.Today, assurancePercentage: 0.1M,
                    assuranceValue: 40000, assuranceFee: 2000, assuranceTotalFeeValue: 2300,
                    assuranceTotalValue: 47600)
                .Build(),
                BuilderDefault(SourceHelperTest.GetCredinetSource(), customerId,
                    creditValue: 200000, creditNumber: 2, interestRate: 0.0209971486861902M, effectiveAnnualRate: 0.283200M,
                    creditDate: DateTime.Now, StoreHelperTest.GetStore(), fees: 10, Frequencies.Monthly,
                    feeValue: 20591.0M, StatusHelperTest.GetCalceledStatus(), AuthMethodHelperTest.GetTokenAuthMethod(),
                    downPayment: 20000, totalDownPayment: 22300, TransactionTypeHelperTest.GetCreateCreditType(),
                    PaymentTypeHelperTest.GetOrdinaryPaymentType(), dueDate: DateTime.Today, assurancePercentage: 0.1M,
                    assuranceValue: 23800.0M, assuranceFee: 345.0M,  assuranceTotalFeeValue: 2300,
                    assuranceTotalValue: 28322)
                .Build(),
                BuilderDefault(SourceHelperTest.GetCredinetSource(), customerId,
                    creditValue: 500000, creditNumber: 3, interestRate: 0.1M, effectiveAnnualRate: 0.283200M,
                    creditDate: DateTime.Now, StoreHelperTest.GetStore(), fees: 15, Frequencies.Monthly,
                    feeValue: 30000, StatusHelperTest.GetCalceledStatus(), AuthMethodHelperTest.GetTokenAuthMethod(),
                    downPayment: 50000, totalDownPayment: 52300, TransactionTypeHelperTest.GetCreateCreditType(),
                    PaymentTypeHelperTest.GetOrdinaryPaymentType(), dueDate: DateTime.Today, assurancePercentage: 0.1M,
                    assuranceValue: 40000, assuranceFee: 2000,  assuranceTotalFeeValue: 2300,
                    assuranceTotalValue: 47600)
                .Build()
            };
        }

        /// <summary>
        /// Add Transaction to the credit history
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="transactionType"></param>
        /// <param name="store"></param>
        /// <param name="creditStatus"></param>
        /// <param name="lastPaymentDate"></param>
        /// <param name="calculationDate"></param>
        public static void AddTransaction(CreditMaster creditMaster, TransactionTypes transactionType, Store store,
            Statuses creditStatus, DateTime lastPaymentDate, DateTime? calculationDate = null)
        {
            Credit credit = creditMaster.Current;
            int frequency = credit.GetFrequency;
            DateTime currentDueDate = credit.CreditPayment.GetDueDate;

            creditMaster.HandleEvent(
                CreditHelperTest.GetAddPaymentMasterEvent(
                    creditMaster: creditMaster, transactionType: TransactionTypeHelperTest.GetTransactionType(transactionType),
                    store: store, dueDate: currentDueDate.AddDays(frequency), calculationDate: calculationDate ?? currentDueDate,
                    lastFee: credit.CreditPayment.GetLastFee + 1, creditStatusId: (int)creditStatus, lastPaymentDate: lastPaymentDate));
        }

        /// <summary>
        /// Add Transaction with dynamic due date to the credit history
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="transactionType"></param>
        /// <param name="store"></param>
        /// <param name="creditStatus"></param>
        /// <param name="lastPaymentDate"></param>
        /// <param name="calculationDate"></param>
        /// <param name="dueDate"></param>
        public static void AddTransactionWithDueDate(CreditMaster creditMaster, TransactionTypes transactionType, Store store,
            Statuses creditStatus, DateTime lastPaymentDate, DateTime dueDate, DateTime? calculationDate = null)
        {
            Credit credit = creditMaster.Current;

            creditMaster.HandleEvent(
                CreditHelperTest.GetAddPaymentMasterEvent(
                    creditMaster: creditMaster, transactionType: TransactionTypeHelperTest.GetTransactionType(transactionType),
                    store: store, dueDate: dueDate, calculationDate: calculationDate ?? dueDate,
                    lastFee: credit.CreditPayment.GetLastFee + 1, creditStatusId: (int)creditStatus, lastPaymentDate: lastPaymentDate));
        }

        /// <summary>
        /// Update Charges Payment Plan
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="chargesValue"></param>
        /// <param name="hasArrearsCharge"></param>
        /// <param name="arrearsCharges"></param>
        /// <param name="updatedPaymentPlanValue"></param>
        /// <param name="transactionType"></param>
        public static void UpdateChargesPaymentPlan(CreditMaster creditMaster, decimal chargesValue, bool hasArrearsCharge,
            decimal arrearsCharges, decimal updatedPaymentPlanValue, TransactionTypes transactionType)
        {
            creditMaster.HandleEvent(new UpdateChargesPaymentPlanMasterEvent(creditMaster, chargesValue, hasArrearsCharge, arrearsCharges,
                updatedPaymentPlanValue, TransactionTypeHelperTest.GetTransactionType(transactionType)));
        }

        /// <summary>
        /// Credit master extra field
        /// </summary>
        /// <returns></returns>
        public static CreditMaster GetCreditMasterExtraFields()
        {
            CreditMaster creditMaster = BuilderDefault(SourceHelperTest.GetCredinetSource(), customerId: new Guid("527af1e7-bcbf-4559-bc38-054f785976f5"),
                    creditValue: 500000, creditNumber: 1, interestRate: 0.1M, effectiveAnnualRate: 0.283200M,
                    creditDate: DateTime.Now, StoreHelperTest.GetStore(), fees: 15, Frequencies.Monthly,
                    feeValue: 30000, StatusHelperTest.GetCancelRequestStatus(), AuthMethodHelperTest.GetTokenAuthMethod(),
                    downPayment: 50000, totalDownPayment: 52300, TransactionTypeHelperTest.GetCreateCreditType(),
                    PaymentTypeHelperTest.GetOrdinaryPaymentType(), dueDate: DateTime.Today, assurancePercentage: 0.1M,
                    assuranceValue: 40000, assuranceFee: 2000, assuranceTotalFeeValue: 2300,
                    assuranceTotalValue: 47600)
                .Build();

            creditMaster.SetSellerInfo(seller: "PruebaSeller", products: "PruebaProducts", invoice: "PruebaInvoice");

            return creditMaster;
        }

        /// <summary>
        /// Get Credit Master
        /// </summary>
        /// <returns></returns>
        public static CreditMaster GetCreditMasterDateTimeNow()
        {
            return GetCreditMaster(creditDate: DateTime.Now, dueDate: DateTime.Now);
        }

        /// <summary>
        /// Get Credit Master Paid
        /// </summary>
        /// <returns></returns>
        public static CreditMaster GetCreditMasterPaid()
        {
            return GetCreditMasterPaidStatus(creditDate: DateTime.Now, dueDate: DateTime.Now.AddDays(30));
        }

        /// <summary>
        /// Get Credit Master
        /// </summary>
        /// <returns></returns>
        public static CreditMaster GetCreditMaster()
        {
            return GetCreditMaster(creditDate: DateTime.Now, dueDate: DateTime.Now.AddDays(30));
        }

        /// <summary>
        /// Get credit master
        /// </summary>
        /// <param name="creditDate"></param>
        /// <param name="dueDate"></param>
        /// <returns></returns>
        public static CreditMaster GetCreditMaster(DateTime creditDate, DateTime dueDate)
        {
            CreditMaster creditMaster = BuilderDefault(SourceHelperTest.GetCredinetSource(), customerId: new Guid("55094b4d-5d5e-cd2b-435f-08d72284f7ae"),
                    creditValue: 200000, creditNumber: 2, interestRate: 0.0343660831319166M, effectiveAnnualRate: 0.283243M,
                    creditDate, StoreHelperTest.GetStore(), fees: 8, Frequencies.Monthly,
                    feeValue: 26117.0M, StatusHelperTest.GetActiveStatus(), AuthMethodHelperTest.GetTokenAuthMethod(),
                    downPayment: 20000, totalDownPayment: 22644.0M, TransactionTypeHelperTest.GetCreateCreditType(),
                    PaymentTypeHelperTest.GetOrdinaryPaymentType(), dueDate, assurancePercentage: 0.1M,
                    assuranceValue: 20000.0M, assuranceFee: 2222.0M, assuranceTotalFeeValue: 2644.0M,
                    assuranceTotalValue: 23800.0M)
                .Build();

            creditMaster.SetSellerInfo(seller: "Vendedor de prueba", products: "Camisetas", invoice: "56886565");

            return creditMaster;
        }

        /// <summary>
        /// Get credit master
        /// </summary>
        /// <param name="creditDate"></param>
        /// <param name="dueDate"></param>
        /// <returns></returns>
        public static CreditMaster GetCreditMasterNoDownPayment(DateTime creditDate, DateTime dueDate)
        {
            CreditMaster creditMaster = BuilderDefault(SourceHelperTest.GetCredinetSource(), customerId: new Guid("55094b4d-5d5e-cd2b-435f-08d72284f7ae"),
                    creditValue: 200000, creditNumber: 2, interestRate: 0.0204985209860811M, effectiveAnnualRate: 0.2757M,
                    creditDate, StoreHelperTest.GetStore(), fees: 8, Frequencies.Monthly,
                    feeValue: 27361.0M, StatusHelperTest.GetActiveStatus(), AuthMethodHelperTest.GetTokenAuthMethod(),
                    downPayment: 0, totalDownPayment: 0, TransactionTypeHelperTest.GetCreateCreditType(),
                    PaymentTypeHelperTest.GetOrdinaryPaymentType(), dueDate, assurancePercentage: 0.1M,
                    assuranceValue: 20000.0M, assuranceFee: 2500.0M, assuranceTotalFeeValue: 2975.0M,
                    assuranceTotalValue: 23800.0M)
                .Build();

            creditMaster.SetSellerInfo(seller: "Vendedor de prueba", products: "Camisetas", invoice: "56886565");

            return creditMaster;
        }

        public static CreditMaster GetCreditMasterWithStoreAllowPromissoryNoteSignature()
        {
            CreditMaster creditMaster = BuilderDefault(SourceHelperTest.GetCredinetSource(), customerId: new Guid("55094b4d-5d5e-cd2b-435f-08d72284f7ae"),
                creditValue: 200000, creditNumber: 2, interestRate: 0.0204985209860811M, effectiveAnnualRate: 0.283243M,
                creditDate: DateTime.Now.AddDays(30), StoreHelperTest.GetStoreWithAllowPromissoryNoteSignature(), fees: 8, Frequencies.Monthly,
                feeValue: 26117.0M, StatusHelperTest.GetActiveStatus(), AuthMethodHelperTest.GetTokenAuthMethod(),
                downPayment: 20000, totalDownPayment: 22644.0M, TransactionTypeHelperTest.GetCreateCreditType(),
                PaymentTypeHelperTest.GetOrdinaryPaymentType(), dueDate: DateTime.Today, assurancePercentage: 0.1M,
                assuranceValue: 20000.0M, assuranceFee: 2222.0M, assuranceTotalFeeValue: 2644.0M,
                assuranceTotalValue: 23800.0M)
            .Build();

            creditMaster.SetSellerInfo(seller: "Vendedor de prueba", products: "Camisetas", invoice: "56886565");

            return creditMaster;
        }

        /// <summary>
        /// Get Credit Master PaidStatus
        /// </summary>
        /// <param name="creditDate"></param>
        /// <param name="dueDate"></param>
        /// <returns></returns>
        public static CreditMaster GetCreditMasterPaidStatus(DateTime creditDate, DateTime dueDate)
        {
            CreditMaster creditMaster = BuilderDefault(SourceHelperTest.GetCredinetSource(), customerId: new Guid("55094b4d-5d5e-cd2b-435f-08d72284f7ae"),
                creditValue: 200000, creditNumber: 2, interestRate: 0.0204985209860811M, effectiveAnnualRate: 0.283243M,
                creditDate, StoreHelperTest.GetStoreWithAllowPromissoryNoteSignature(), fees: 8, Frequencies.Monthly,
                feeValue: 26117.0M, StatusHelperTest.GetPaidStatus(), AuthMethodHelperTest.GetTokenAuthMethod(),
                downPayment: 20000, totalDownPayment: 22644.0M, TransactionTypeHelperTest.GetCreateCreditType(),
                PaymentTypeHelperTest.GetOrdinaryPaymentType(), dueDate, assurancePercentage: 0.1M,
                assuranceValue: 20000.0M, assuranceFee: 2222.0M, assuranceTotalFeeValue: 2644.0M,
                assuranceTotalValue: 23800.0M)
            .Build();

            creditMaster.SetSellerInfo(seller: "Vendedor de prueba", products: "Camisetas", invoice: "56886565");

            return creditMaster;
        }

        /// <summary>
        /// Update credit master extra field
        /// </summary>
        /// <returns></returns>
        public static UpdateCreditExtraFieldsRequest GetUpdateCreditExtraFields()
        {
            return new UpdateCreditExtraFieldsRequest(Guid.NewGuid(), "UpdateSeller", "UpdateProducts", "UpdateInvoice");
        }

        /// <summary>
        /// Add Random number of payment transactions in Credit History
        /// </summary>
        /// <param name="creditMaster"></param>
        public static void AddRandomPayments(CreditMaster creditMaster)
        {
            Random rnd = new Random();
            int numberOfPayments = rnd.Next(6);

            Enumerable.Range(1, numberOfPayments)
                .ToList()
                .ForEach(x => AddTransaction(creditMaster, TransactionTypes.Payment, creditMaster.Store, Statuses.Active, DateTime.Now.AddDays(-x)));
        }

        /// <summary>
        /// Add count of payment transactions in Credit History
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="count"></param>
        public static void AddCountPayments(CreditMaster creditMaster, int count)
        {
            Enumerable.Range(1, count)
                .ToList()
                .ForEach(x => AddTransaction(creditMaster, TransactionTypes.Payment, creditMaster.Store, Statuses.Active, DateTime.Now.AddDays(-x)));
        }

        /// <summary>
        /// Get Credit Master List
        /// </summary>
        /// <returns></returns>
        public static List<CreditMaster> GetCreditMasterListTemplate()
        {
            return new List<CreditMaster>
            {
                BuilderDefault(SourceHelperTest.GetCredinetSource(), customerId: new Guid("527af1e7-bcbf-4559-bc38-054f785976f5"),
                    creditValue: 500000, creditNumber: 1, interestRate: 0.1M, effectiveAnnualRate: 0.283200M,
                    creditDate: DateTime.Now, StoreHelperTest.GetStoreLimitClient(), fees: 15, Frequencies.Monthly,
                    feeValue: 30000, StatusHelperTest.GetActiveStatus(), AuthMethodHelperTest.GetTokenAuthMethod(),
                    downPayment: 50000, totalDownPayment: 52300, TransactionTypeHelperTest.GetCreateCreditType(),
                    PaymentTypeHelperTest.GetOrdinaryPaymentType(), dueDate: DateTime.Today, assurancePercentage: 0.1M,
                    assuranceValue: 40000, assuranceFee: 2000, assuranceTotalFeeValue: 2300,
                    assuranceTotalValue: 47600)
                .Build(),
                BuilderDefault(SourceHelperTest.GetCredinetSource(), customerId: new Guid("55094b4d-5d5e-cd2b-435f-08d72284f7ae"),
                    creditValue: 200000, creditNumber: 2, interestRate: 0.0209971486861902M, effectiveAnnualRate: 0.283200M,
                    creditDate: DateTime.Now, StoreHelperTest.GetStore(), fees: 10, Frequencies.Monthly,
                    feeValue: 20591.0M, StatusHelperTest.GetActiveStatus(), AuthMethodHelperTest.GetTokenAuthMethod(),
                    downPayment: 20000, totalDownPayment: 22300, TransactionTypeHelperTest.GetCreateCreditType(),
                    PaymentTypeHelperTest.GetOrdinaryPaymentType(), dueDate: DateTime.Now.AddDays(30), assurancePercentage: 0.1M,
                    assuranceValue: 23800.0M, assuranceFee: 345.0M,  assuranceTotalFeeValue: 2300,
                    assuranceTotalValue: 28322)
                .Build(),
                BuilderDefault(SourceHelperTest.GetCredinetSource(), customerId: new Guid("527af1e7-bcbf-4559-bc38-054f785976f5"),
                    creditValue: 0, creditNumber: 1, interestRate: 0.1M, effectiveAnnualRate: 0.283200M,
                    creditDate: DateTime.Now, StoreHelperTest.GetStoreLimitClient(), fees: 15, Frequencies.Monthly,
                    feeValue: 30000, StatusHelperTest.GetActiveStatus(), AuthMethodHelperTest.GetTokenAuthMethod(),
                    downPayment: 50000, totalDownPayment: 52300, TransactionTypeHelperTest.GetCreateCreditType(),
                    PaymentTypeHelperTest.GetOrdinaryPaymentType(), dueDate: DateTime.Today, assurancePercentage: 0.1M,
                    assuranceValue: 40000, assuranceFee: 2000, assuranceTotalFeeValue: 2300,
                    assuranceTotalValue: 47600)
                .Build()
            };
        }

        /// <summary>
        /// Get Credit Master Paid List Template
        /// </summary>
        /// <returns></returns>
        public static List<CreditMaster> GetCreditMasterPaidListTemplate()
        {
            return new List<CreditMaster>
            {
                BuilderDefault(SourceHelperTest.GetCredinetSource(), customerId: new Guid("55094b4d-5d5e-cd2b-435f-08d72284f7ae"),
                    creditValue: 200000, creditNumber: 2, interestRate: 0.0204985209860811M, effectiveAnnualRate: 0.283243M,
                    creditDate: DateTime.Now, StoreHelperTest.GetStore(), fees: 8, Frequencies.Monthly,
                    feeValue: 26117.0M, StatusHelperTest.GetPaidStatus(), AuthMethodHelperTest.GetTokenAuthMethod(),
                    downPayment: 20000, totalDownPayment: 22644.0M, TransactionTypeHelperTest.GetCreateCreditType(),
                    PaymentTypeHelperTest.GetOrdinaryPaymentType(), dueDate: DateTime.Now.AddDays(30), assurancePercentage: 0.1M,
                    assuranceValue: 20000.0M, assuranceFee: 2222.0M, assuranceTotalFeeValue: 2644.0M,
                    assuranceTotalValue: 23800.0M)
                .Build(),
                BuilderDefault(SourceHelperTest.GetCredinetSource(), customerId: new Guid("55094b4d-5d5e-cd2b-435f-08d72284f7ae"),
                    creditValue: 200000, creditNumber: 2, interestRate: 0.0209971486861902M, effectiveAnnualRate: 0.283200M,
                    creditDate: DateTime.Now, StoreHelperTest.GetStore(), fees: 10, Frequencies.Monthly,
                    feeValue: 20591.0M, StatusHelperTest.GetPaidStatus(), AuthMethodHelperTest.GetTokenAuthMethod(),
                    downPayment: 20000, totalDownPayment: 22300, TransactionTypeHelperTest.GetCreateCreditType(),
                    PaymentTypeHelperTest.GetOrdinaryPaymentType(), dueDate: DateTime.Today, assurancePercentage: 0.1M,
                    assuranceValue: 23800.0M, assuranceFee: 345.0M,  assuranceTotalFeeValue: 2300,
                    assuranceTotalValue: 28322)
                .Build()
            };
        }

        /// <summary>
        /// Builder default
        /// </summary>
        /// <param name="source"></param>
        /// <param name="customerId"></param>
        /// <param name="creditValue"></param>
        /// <param name="creditNumber"></param>
        /// <param name="interestRate"></param>
        /// <param name="effectiveAnnualRate"></param>
        /// <param name="creditDate"></param>
        /// <param name="store"></param>
        /// <param name="fees"></param>
        /// <param name="frequency"></param>
        /// <param name="feeValue"></param>
        /// <param name="status"></param>
        /// <param name="authMethod"></param>
        /// <param name="downPayment"></param>
        /// <param name="totalDownPayment"></param>
        /// <param name="transactionType"></param>
        /// <param name="paymentType"></param>
        /// <param name="dueDate"></param>
        /// <param name="assurancePercentage"></param>
        /// <param name="assuranceValue"></param>
        /// <param name="assuranceFee"></param>
        /// <param name="assuranceTotalFeeValue"></param>
        /// <param name="assuranceTotalValue"></param>
        /// <returns></returns>
        public static ICreditCompleteBuilder BuilderDefault(Source source, Guid customerId, decimal creditValue, long creditNumber, decimal interestRate,
            decimal effectiveAnnualRate, DateTime creditDate, Store store, int fees, Frequencies frequency, decimal feeValue,
            Status status, AuthMethod authMethod, decimal downPayment, decimal totalDownPayment, TransactionType transactionType,
            PaymentType paymentType, DateTime dueDate, decimal assurancePercentage, decimal assuranceValue, decimal assuranceFee,
            decimal assuranceTotalFeeValue, decimal assuranceTotalValue)
        {
            return CreditBuilder.CreateBuilder()
                .Init(source, CustomerHelperTest.GetCustomer(customerId), creditValue, creditNumber,
                    new UserInfo(userName: "TestUser", userId: "TestUserId"), CredinetAppSettingsHelperTest.GetCredinetAppSettings())
                .Rates(interestRate, effectiveAnnualRate)
                .CreditDate(creditDate)
                .AdditionalInfo(store, token: "345687", location: "1,1", "1")
                .FeesInfo(fees, (int)frequency, feeValue)
                .AdditionalDetailInfo(status, authMethod, downPayment, totalDownPayment)
                .InitialPayment(transactionType, paymentType, dueDate, calculationDate: creditDate)
                .AssuranceValues(assurancePercentage, assuranceValue, assuranceFee, assuranceTotalFeeValue, assuranceTotalValue);
        }

        /// <summary>
        /// Get Credit Master List
        /// </summary>
        /// <returns></returns>
        public static List<CreditMaster> GetCreditMasterWithRequestCancelList()
        {
            return new List<CreditMaster>
            {
                BuilderDefault(SourceHelperTest.GetCredinetSource(), customerId: new Guid("55094b4d-5d5e-cd3b-435f-08d72284f7ae"),
                    creditValue: 500000, creditNumber: 1, interestRate: 0.1M, effectiveAnnualRate: 0.283200M,
                    creditDate: new DateTime(2021, 1, 1), StoreHelperTest.GetStore(), fees: 15, Frequencies.Monthly,
                    feeValue: 30000, StatusHelperTest.GetActiveStatus(), AuthMethodHelperTest.GetTokenAuthMethod(),
                    downPayment: 50000, totalDownPayment: 52300, TransactionTypeHelperTest.GetCreateCreditType(),
                    PaymentTypeHelperTest.GetOrdinaryPaymentType(), dueDate: DateTime.Today, assurancePercentage: 0.1M,
                    assuranceValue: 40000, assuranceFee: 2000, assuranceTotalFeeValue: 2300,
                    assuranceTotalValue: 47600)
                .Build(),
                BuilderDefault(SourceHelperTest.GetCredinetSource(), customerId: new Guid("55094b4d-5d5e-cd3b-435f-08d72284f7ae"),
                    creditValue: 200000, creditNumber: 2, interestRate: 0.0209971486861902M, effectiveAnnualRate: 0.283200M,
                    creditDate: new DateTime(2021, 1, 1), StoreHelperTest.GetStore(), fees: 10, Frequencies.Monthly,
                    feeValue: 20591.0M, StatusHelperTest.GetCancelRequestStatus(), AuthMethodHelperTest.GetTokenAuthMethod(),
                    downPayment: 20000, totalDownPayment: 22300, TransactionTypeHelperTest.GetCreateCreditType(),
                    PaymentTypeHelperTest.GetOrdinaryPaymentType(), dueDate: DateTime.Today, assurancePercentage: 0.1M,
                    assuranceValue: 23800.0M, assuranceFee: 345.0M,  assuranceTotalFeeValue: 2300,
                    assuranceTotalValue: 28322)
                .Build()
                .SetId(default)
                .SetCreditId(default)
            };
        }
    }
}