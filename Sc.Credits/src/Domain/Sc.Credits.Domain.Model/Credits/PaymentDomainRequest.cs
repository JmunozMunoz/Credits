using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Helpers.ObjectsUtils;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The payment domain request entity
    /// </summary>
    public class PaymentDomainRequest
    {
        /// <summary>
        /// Gets the payment credit request
        /// </summary>
        public PaymentCreditRequestComplete PaymentCreditRequest { get; private set; }

        /// <summary>
        /// Gets the credit master
        /// </summary>
        public CreditMaster CreditMaster { get; private set; }

        /// <summary>
        /// Gets the store
        /// </summary>
        public Store Store { get; private set; }

        /// <summary>
        /// Gets the parameters
        /// </summary>
        public AppParameters Parameters { get; private set; }

        /// <summary>
        /// Gets the payment transaction type
        /// </summary>
        public TransactionType PaymentTransactionType { get; private set; }

        /// <summary>
        /// Gets the active status
        /// </summary>
        public Status ActiveStatus { get; private set; }

        /// <summary>
        /// Gets the paid status
        /// </summary>
        public Status PaidStatus { get; private set; }

        /// <summary>
        /// Gets or sets the balance release for refinancing
        /// </summary>
        public decimal BalanceReleaseForRefinancing { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="PaymentDomainRequest"/>
        /// </summary>
        /// <param name="paymentCreditRequest"></param>
        /// <param name="creditMaster"></param>
        /// <param name="store"></param>
        /// <param name="parameters"></param>
        /// <param name="credinetAppSettings"></param>
        /// <param name="source"></param>
        public PaymentDomainRequest(PaymentCreditRequestComplete paymentCreditRequest, CreditMaster creditMaster,
            Store store, AppParameters parameters, CredinetAppSettings credinetAppSettings, int source=0, bool setCreditLimit = true)
        {
            PaymentCreditRequest = paymentCreditRequest;
            CreditMaster = creditMaster;
            Store = store;
            Parameters = parameters;
            BalanceReleaseForRefinancing = paymentCreditRequest.BalanceReleaseForRefinancing;

            CreditMaster.SetCustomerCreditLimitUpdate(credinetAppSettings, source, setCreditLimit : setCreditLimit);
        }

        /// <summary>
        /// Set masters
        /// </summary>
        /// <param name="paymentTransactionType"></param>
        /// <param name="activeStatus"></param>
        /// <param name="paidStatus"></param>
        /// <returns></returns>
        public PaymentDomainRequest SetMasters(TransactionType paymentTransactionType, Status activeStatus,
            Status paidStatus)
        {
            PaymentTransactionType = paymentTransactionType;
            ActiveStatus = activeStatus;
            PaidStatus = paidStatus;
            return this;
        }
    }
}