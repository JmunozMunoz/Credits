using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Helpers.ObjectsUtils;
using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Refinancings
{
    /// <summary>
    /// The refinancing domain request entity
    /// </summary>
    public class RefinancingDomainRequest
    {
        /// <summary>
        /// Gets the refinancing credit request
        /// </summary>
        public RefinancingCreditRequest RefinancingCreditRequest { get; private set; }

        /// <summary>
        /// Gets the create credit domain request
        /// </summary>
        public CreateCreditDomainRequest CreateCreditDomainRequest { get; private set; }

        /// <summary>
        /// Gets the customer
        /// </summary>
        public Customer Customer { get; private set; }

        /// <summary>
        /// Gets the store
        /// </summary>
        public Store Store { get; private set; }

        /// <summary>
        /// Gets the parameters
        /// </summary>
        public AppParameters Parameters { get; private set; }

        /// <summary>
        /// Gets the refinancing credit masters
        /// </summary>
        public List<CreditMaster> RefinancingCreditMasters { get; private set; }

        /// <summary>
        /// Gets the application
        /// </summary>
        public RefinancingApplication Application { get; private set; }

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
        /// Gets the paid status
        /// </summary>
        public int Source { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="RefinancingDomainRequest"/>
        /// </summary>
        /// <param name="refinancingCreditRequest"></param>
        /// <param name="createCreditDomainRequest"></param>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <param name="parameters"></param>
        public RefinancingDomainRequest(RefinancingCreditRequest refinancingCreditRequest, CreateCreditDomainRequest createCreditDomainRequest,
            Customer customer, Store store, AppParameters parameters)
        {
            customer.SetCreditLimitValidation(false);

            RefinancingCreditRequest = refinancingCreditRequest;
            CreateCreditDomainRequest = createCreditDomainRequest;
            Customer = customer;
            Store = store;
            Parameters = parameters;
            Source = refinancingCreditRequest.Source;
        }

        /// <summary>
        /// Set refinancing params
        /// </summary>
        /// <param name="refinancingCreditMasters"></param>
        /// <param name="application"></param>
        /// <param name="credinetAppSettings"></param>
        /// <returns></returns>
        public RefinancingDomainRequest SetRefinancingParams(List<CreditMaster> refinancingCreditMasters, RefinancingApplication application,
            CredinetAppSettings credinetAppSettings, bool setCreditLimit = true)
        {
            refinancingCreditMasters.ForEach(creditMaster => creditMaster.SetCustomer(Customer, credinetAppSettings, setCreditLimit));
            RefinancingCreditMasters = refinancingCreditMasters;
            Application = application;
            return this;
        }

        /// <summary>
        /// Set payment params
        /// </summary>
        /// <param name="paymentTransactionType"></param>
        /// <param name="activeStatus"></param>
        /// <param name="paidStatus"></param>
        /// <returns></returns>
        public RefinancingDomainRequest SetPaymentParams(TransactionType paymentTransactionType,
            Status activeStatus, Status paidStatus)
        {
            PaymentTransactionType = paymentTransactionType;
            ActiveStatus = activeStatus;
            PaidStatus = paidStatus;
            return this;
        }
    }
}