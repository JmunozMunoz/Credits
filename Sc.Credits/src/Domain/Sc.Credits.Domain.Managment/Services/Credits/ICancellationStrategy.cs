using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Credits
{
    /// <summary>
    /// Cancellation strategy contract
    /// </summary>
    public interface ICancellationStrategy
    {
        /// <summary>
        /// Approve cancellation async
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="requestCancelCredit"></param>
        /// <param name="creditMaster"></param>
        /// <returns></returns>
        Task ApproveCancellationAsync(UserInfo userInfo, RequestCancelCredit requestCancelCredit, CreditMaster creditMaster);

        /// <summary>
        /// Validate cancellation request async
        /// </summary>
        /// <param name="cancelCreditRequest"></param>
        /// <param name="creditMaster"></param>
        /// <returns></returns>
        Task<decimal> ValidateCancellationRequest(CancelCreditRequest cancelCreditRequest, CreditMaster creditMaster);
    }
}