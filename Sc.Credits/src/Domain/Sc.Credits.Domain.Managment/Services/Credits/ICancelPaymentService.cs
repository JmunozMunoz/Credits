using Sc.Credits.Domain.Model.Credits;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace Sc.Credits.Domain.Managment.Services.Credits
{
    /// <summary>
    /// Credit payment service contract
    /// </summary>
    public interface ICancelPaymentService
    {
        /// <summary>
        /// Get active and pending cancellation
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        Task<List<ActivePendingCancellationPaymentResponse>> GetActiveAndPendingCancellationAsync(string typeDocument, string idDocument,
            string storeId);

        /// <summary>
        /// Request
        /// </summary>
        /// <param name="cancelPaymentRequest"></param>
        /// <returns></returns>
        Task RequestAsync(CancelPaymentRequest cancelPaymentRequest);

        /// <summary>
        /// Get pendings
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="valuePage"></param>
        /// <returns></returns>
        Task<CancelPaymentDetailResponsePaged> GetPendingsAsync(string vendorId, int pageNumber, int valuePage, bool count);

        /// <summary>
        /// Cancel
        /// </summary>
        /// <param name="cancelPayments"></param>
        /// <returns></returns>
        Task CancelAsync(CancelPayments cancelPayments);

        /// <summary>
        /// Cancel transaction func
        /// </summary>
        /// <param name="cancelPaymentsTransactionRequest"></param>
        /// <param name="cancelTransactionType"></param>
        /// <param name="activeStatus"></param>
        /// <returns></returns>
        Func<Transaction, Task<List<RequestCancelPayment>>> CancelTransactionFunc(CancelPaymentsTransactionRequest cancelPaymentsTransactionRequest,
            TransactionType cancelTransactionType, Status activeStatus);

        /// <summary>
        /// Reject
        /// </summary>
        /// <param name="cancelPaymentRequest"></param>
        /// <returns></returns>
        Task RejectAsync(CancelPayments cancelPaymentRequest);

        /// <summary>
        /// Rejects unprocessed request
        /// </summary>
        /// <returns></returns>
        Task RejectUnprocessedRequestAsync();
    }
}