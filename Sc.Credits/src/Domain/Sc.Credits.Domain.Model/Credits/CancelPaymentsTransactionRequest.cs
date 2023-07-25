using Sc.Credits.Domain.Model.Common;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The cancel payments transaction request entity
    /// </summary>
    public class CancelPaymentsTransactionRequest
    {
        /// <summary>
        /// Gets the creditMaster
        /// </summary>
        public CreditMaster CreditMaster { get; private set; }

        /// <summary>
        /// Gets the request cancel payments
        /// </summary>
        public List<RequestCancelPayment> RequestCancelPayments { get; private set; }

        /// <summary>
        /// Gets all request cancels
        /// </summary>
        public List<RequestCancelPayment> AllRequestCancels { get; private set; }

        /// <summary>
        /// Gets the user's info
        /// </summary>
        public UserInfo UserInfo { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="CancelPaymentsTransactionRequest"/>
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="requestCancelPayments"></param>
        /// <param name="allRequestCancels"></param>
        /// <param name="userInfo"></param>
        public CancelPaymentsTransactionRequest(CreditMaster creditMaster,
            List<RequestCancelPayment> requestCancelPayments,
            List<RequestCancelPayment> allRequestCancels,
            UserInfo userInfo)
        {
            CreditMaster = creditMaster;
            RequestCancelPayments = requestCancelPayments;
            AllRequestCancels = allRequestCancels;
            UserInfo = userInfo;
        }

        /// <summary>
        /// Checks if there are subsequent payments
        /// </summary>
        /// <param name="requestCancelPayment"></param>
        /// <returns></returns>
        public bool ThereAreSubsequentPayments(RequestCancelPayment requestCancelPayment) =>
            CreditMaster.GetPaymentTransactionsNotCanceled(AllRequestCancels)
                .Any(payment => payment.GetTransactionDateComplete > requestCancelPayment.GetDateComplete);

        /// <summary>
        /// Gets the last payment without cancel
        /// </summary>
        /// <param name="requestCanceleds"></param>
        /// <returns></returns>
        public Credit GetLastPaymentWithoutCancel(List<RequestCancelPayment> requestCanceleds) =>
            CreditMaster.GetLastPaymentWithoutCancel(requestCanceleds, AllRequestCancels);
    }
}