using Sc.Credits.Domain.Model.Credits;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Credits
{
    /// <summary>
    /// Cancel credit service contract
    /// </summary>
    public interface ICancelCreditService
    {
        /// <summary>
        /// Get active and pending cancellations
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <param name="idDocument"></param>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        Task<List<ActivePendingCancellationCreditResponse>> GetActiveAndPendingCancellationAsync(string typeDocument, string idDocument, string vendorId);

        /// <summary>
        /// Request
        /// </summary>
        /// <param name="cancelCreditRequest"></param>
        /// <returns></returns>
        Task RequestAsync(CancelCreditRequest cancelCreditRequest);

        /// <summary>
        /// Get pendings
        /// </summary>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        Task<CancelCreditDetailResponsePaged> GetPendingsAsync(string vendorId,int pageNumber,int valuePage, bool count);

        /// <summary>
        /// Cancel
        /// </summary>
        /// <param name="creditId"></param>
        /// <returns></returns>
        Task CancelAsync(CancelCredit cancelCredit);

        /// <summary>
        /// Reject
        /// </summary>
        /// <param name="cancelCredit"></param>
        /// <returns></returns>
        Task RejectAsync(CancelCredit cancelCredit);

        /// <summary>
        /// Rejects unprocessed request
        /// </summary>
        /// <returns></returns>
        Task RejectUnprocessedRequestAsync();
    }
}