using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The active and pending cancellation payments request entity
    /// </summary>
    public class ActiveAndPendingCancellationPaymentsRequest
    {
        /// <summary>
        /// Gets the customer active and pending cancelation credits masters
        /// </summary>
        public List<CreditMaster> CustomerActiveAndPendingCancelationCreditMasters { get; private set; }

        /// <summary>
        /// Gets all request cancel payments undismissed
        /// </summary>
        public List<RequestCancelPayment> AllRequestCancelPaymentsUndismissed { get; private set; }

        /// <summary>
        /// Gets the store's id
        /// </summary>
        public string StoreId { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="ActiveAndPendingCancellationPaymentsRequest"/>
        /// </summary>
        /// <param name="customerActiveAndPendingCancelationCreditMasters"></param>
        /// <param name="allRequestCancelPaymentsUndismissed"></param>
        /// <param name="storeId"></param>
        public ActiveAndPendingCancellationPaymentsRequest(List<CreditMaster> customerActiveAndPendingCancelationCreditMasters, List<RequestCancelPayment> allRequestCancelPaymentsUndismissed, string storeId)
        {
            CustomerActiveAndPendingCancelationCreditMasters = customerActiveAndPendingCancelationCreditMasters;
            AllRequestCancelPaymentsUndismissed = allRequestCancelPaymentsUndismissed;
            StoreId = storeId;
        }

        /// <summary>
        /// Gets payment transactions
        /// </summary>
        /// <param name="maximumDaysRequestCancellationPayments"></param>
        /// <returns></returns>
        public List<Credit> GetPaymentTransactions(int maximumDaysRequestCancellationPayments) =>
            CustomerActiveAndPendingCancelationCreditMasters
                .SelectMany(creditMaster => creditMaster.GetPaymentTransactionsNotCanceled(AllRequestCancelPaymentsUndismissed, StoreId,
                    maximumDaysRequestCancellationPayments)).ToList();

        /// <summary>
        /// Verify if payment is cancellable
        /// </summary>
        /// <param name="paymentTransaction"></param>
        /// <returns></returns>
        public bool IsCancellablePayment(Credit paymentTransaction) =>
            GetCancellablePayments().Any(payment => payment.Id == paymentTransaction.Id);

        /// <summary>
        /// Gets the cancellable payments
        /// </summary>
        /// <returns></returns>
        private List<Credit> GetCancellablePayments() =>
            CustomerActiveAndPendingCancelationCreditMasters
                .Select(credit => credit.GetCancelablePayment(AllRequestCancelPaymentsUndismissed))
                    .Where(payment => payment != null)
                        .ToList();

        /// <summary>
        /// Verify if payment has cancel request
        /// </summary>
        /// <param name="paymentTransaction"></param>
        /// <returns></returns>
        public bool PaymentHasCancelRequest(Credit paymentTransaction) =>
            paymentTransaction.HasCancelRequest(AllRequestCancelPaymentsUndismissed);
    }
}