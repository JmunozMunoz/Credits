using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The active and pending cancellation credits request
    /// </summary>
    public class ActiveAndPendingCancellationCreditsRequest
    {
        /// <summary>
        /// Gets the customer active and pending cancelation credits master
        /// </summary>
        public List<CreditMaster> CustomerActiveAndPendingCancelationCreditMasters { get; private set; }

        /// <summary>
        /// Gets the request cancel payments canceled
        /// </summary>
        public List<RequestCancelPayment> RequestCancelPaymentsCanceled { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="ActiveAndPendingCancellationCreditsRequest"/>
        /// </summary>
        /// <param name="customerActiveAndPendingCancelationCreditMasters"></param>
        /// <param name="requestCancelPaymentsCanceled"></param>
        public ActiveAndPendingCancellationCreditsRequest(List<CreditMaster> customerActiveAndPendingCancelationCreditMasters,
            List<RequestCancelPayment> requestCancelPaymentsCanceled)
        {
            CustomerActiveAndPendingCancelationCreditMasters = customerActiveAndPendingCancelationCreditMasters;
            RequestCancelPaymentsCanceled = requestCancelPaymentsCanceled;
        }

        /// <summary>
        /// Verify if credit has payments
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <returns></returns>
        public bool CreditHasPayments(CreditMaster creditMaster) =>
            creditMaster.HasPayments(RequestCancelPaymentsCanceled);
    }
}