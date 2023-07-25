namespace Sc.Credits.Domain.Model.Credits.Gateway
{
    using Sc.Credits.Domain.Model.Base;
    using Sc.Credits.Domain.Model.Customers;
    using Sc.Credits.Domain.Model.Enums;
    using Sc.Credits.Domain.Model.Queries;
    using Sc.Credits.Domain.Model.Stores;
    using Sc.Credits.Helpers.ObjectsUtils;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Transactions;

    /// <summary>
    /// Credit master repository contract
    /// </summary>
    public interface ICreditMasterRepository
        : ITransactionRepository<CreditMaster>
    {
        /// <summary>
        /// Add transaction
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="additionalMasterUpdateFields"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task AddTransactionAsync(CreditMaster creditMaster, IEnumerable<Field> additionalMasterUpdateFields = null,
            Transaction transaction = null);

        /// <summary>
        /// Get with current
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fields"></param>
        /// <param name="transactionFields"></param>
        /// <param name="customerFields"></param>
        /// <param name="storeFields"></param>
        /// <param name="transactionStoreFields"></param>
        /// <returns></returns>
        Task<CreditMaster> GetWithCurrentAsync(Guid id, IEnumerable<Field> fields = null,
            IEnumerable<Field> transactionFields = null, IEnumerable<Field> customerFields = null,
            IEnumerable<Field> storeFields = null, IEnumerable<Field> transactionStoreFields = null);

        /// <summary>
        /// Get with current
        /// </summary>
        /// <param name="id"></param>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <param name="transactionStoreFields"></param>
        /// <returns></returns>
        Task<CreditMaster> GetWithCurrentAsync(Guid id, Customer customer, Store store,
            IEnumerable<Field> transactionStoreFields = null);

        /// <summary>
        /// Get with current
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="customer"></param>
        /// <param name="storeFields"></param>
        /// <param name="transactionStoreFields"></param>
        /// <param name="statuses"></param>
        /// <returns></returns>
        Task<List<CreditMaster>> GetWithCurrentAsync(List<Guid> ids, Customer customer, IEnumerable<Field> storeFields = null,
            IEnumerable<Field> transactionStoreFields = null, IEnumerable<Statuses> statuses = null, bool setCreditLimit = true);

        /// <summary>
        /// Get with transactions
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fields"></param>
        /// <param name="transactionFields"></param>
        /// <param name="customerFields"></param>
        /// <param name="storeFields"></param>
        /// <param name="transactionStoreFields"></param>
        /// <returns></returns>
        Task<CreditMaster> GetWithTransactionsAsync(Guid id, IEnumerable<Field> fields = null,
            IEnumerable<Field> transactionFields = null, IEnumerable<Field> customerFields = null,
            IEnumerable<Field> storeFields = null, IEnumerable<Field> transactionStoreFields = null);

        /// <summary>
        /// Get with transactions
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="statuses"></param>
        /// <param name="storeFields"></param>
        /// <param name="transactionStoreFields"></param>
        /// <param name="statusId"></param>
        /// <returns></returns>
        Task<List<CreditMaster>> GetWithTransactionsAsync(Customer customer, IEnumerable<Statuses> statuses = null,
            IEnumerable<Field> storeFields = null, IEnumerable<Field> transactionStoreFields = null);

        /// <summary>
        /// Get with transactions by specific ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="fields"></param>
        /// <param name="transactionFields"></param>
        /// <param name="customerFields"></param>
        /// <param name="storeFields"></param>
        /// <returns></returns>
        Task<List<CreditMaster>> GetWithTransactionsAsync(List<Guid> ids, IEnumerable<Field> fields = null,
            IEnumerable<Field> transactionFields = null, IEnumerable<Field> customerFields = null, IEnumerable<Field> storeFields = null, int statusId = 0);

        /// <summary>
        /// Get with transactions by credit id
        /// </summary>
        /// <param name="creditId"></param>
        /// <param name="fields"></param>
        /// <param name="transactionFields"></param>
        /// <param name="customerFields"></param>
        /// <param name="storeFields"></param>
        /// <param name="transactionStoreFields"></param>
        /// <returns></returns>
        Task<CreditMaster> GetWithTransactionsByCreditIdAsync(Guid creditId, IEnumerable<Field> fields = null,
            IEnumerable<Field> transactionFields = null, IEnumerable<Field> customerFields = null,
            IEnumerable<Field> storeFields = null, IEnumerable<Field> transactionStoreFields = null);

        /// <summary>
        /// Get active credits by collect type
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        Task<List<CreditMaster>> GetActiveCreditsByCollectTypeAsync(Customer customer, Store store);


        /// <summary>
        /// 
        ///Get active credits by collect type for the commitment module
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        Task<List<CreditMaster>> GetActiveCreditsByCollectTypeAsync(Customer customer);

        /// <summary>
        /// Get active credits
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        Task<List<CreditMaster>> GetActiveCreditsAsync(Customer customer);

        /// <summary>
        /// Get active and cancel requested credits
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        Task<List<CreditMaster>> GetActiveAndCancelRequestCreditsAsync(Customer customer);

        /// <summary>
        /// Get paid credits for certificate
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<List<CreditMaster>> GetPaidCreditsForCertificateAsync(List<Guid> ids);

        /// <summary>
        /// Get customer credit history
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="customer"></param>
        /// <param name="maximumMonthsCreditHistory"></param>
        /// <returns></returns>
        Task<List<CreditMaster>> GetCustomerCreditHistoryAsync(Customer customer, string storeId, int maximumMonthsCreditHistory);

        /// <summary>
        /// Get active and pendig cancellation credits
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="vendorId"></param>
        /// <param name="transactionFields"></param>
        /// <param name="storeFields"></param>
        /// <returns></returns>
        Task<List<CreditMaster>> GetActiveAndPendingCancellationCreditsAsync(Customer customer, string vendorId,
            IEnumerable<Field> transactionFields, IEnumerable<Field> storeFields);

        /// <summary>
        /// Get active and pending cancellation payments
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        Task<List<CreditMaster>> GetActiveAndPendingCancellationPaymentsAsync(Customer customer);

        /// <summary>
        /// Get customer payment history
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <param name="masterFields"></param>
        /// <param name="transactionStoreFields"></param>
        /// <param name="maximumMonthsPaymentHistory"></param>
        /// <returns></returns>
        Task<List<Credit>> GetCustomerPaymentHistoryAsync(Customer customer, Store store, IEnumerable<Field> masterFields,
            IEnumerable<Field> transactionStoreFields, int maximumMonthsPaymentHistory);

        /// <summary>
        /// Get payments
        /// </summary>
        /// <param name="paymentsId"></param>
        /// <param name="fields"></param>
        /// <param name="masterFields"></param>
        /// <param name="customerFields"></param>
        /// <param name="storeFields"></param>
        /// <param name="masterStoreFields"></param>
        /// <returns></returns>
        Task<List<Credit>> GetPaymentsAsync(List<Guid> paymentsId, IEnumerable<Field> fields, IEnumerable<Field> masterFields,
            IEnumerable<Field> customerFields, IEnumerable<Field> storeFields, IEnumerable<Field> masterStoreFields);

        /// <summary>
        /// Gets specific credit transactions by ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="customerFields"></param>
        /// <param name="storeFields"></param>
        /// <returns></returns>
        Task<List<Credit>> GetTransactionsAsync(List<Guid> ids, IEnumerable<Field> customerFields, IEnumerable<Field> storeFields);

        /// <summary>
        /// Is duplicated credit
        /// </summary>
        /// <param name="token"></param>
        /// <param name="date"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        Task<bool> IsDuplicatedAsync(string token, DateTime date, Guid customerId);

        /// <summary>
        /// Customer photo signature allowed
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="paidCreditDays"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<bool> ValidateACreditPaidAccordingToTime(Guid customerId, int paidCreditDays, Statuses status);

        /// <summary>
        /// Validate customer history
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        Task<bool> ValidateCustomerHistory(Guid customerId);

        /// <summary>
        /// Gets active credits with token
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        Task<List<CreditMaster>> GetActiveCreditsWithTokenAsync(Customer customer, string storeId);

        /// <summary>
        /// Gets active credits by day, month and year of the date with promissoryFile is null
        /// </summary>
        /// <param name="date"></param>
        /// <param name="customerFields"></param>
        /// <param name="top">limit of credits per transaction</param>
        /// <returns></returns>
        Task<List<CreditMaster>> GetActiveWithDayDateAndPendingPromissoryNoteAsync(DateTime date, IEnumerable<Field> customerFields, int top);
    }
}