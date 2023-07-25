using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.Model.Stores;
using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The create credit domain request entity
    /// </summary>
    public class CreateCreditDomainRequest
    {
        /// <summary>
        /// Gets the create credit request
        /// </summary>
        public CreateCreditRequest CreateCreditRequest { get; private set; }

        /// <summary>
        /// Gets the customer
        /// </summary>
        public Customer Customer { get; private set; }

        /// <summary>
        /// Gets the store
        /// </summary>
        public Store Store { get; private set; }

        /// <summary>
        /// Gets the create credit transcation type
        /// </summary>
        public TransactionType CreateCreditTransactionType { get; private set; }

        /// <summary>
        /// Gets the ordinary payment type
        /// </summary>
        public PaymentType OrdinaryPaymentType { get; private set; }

        /// <summary>
        /// Gets the status
        /// </summary>
        public Status Status { get; private set; }

        /// <summary>
        /// Gets the source
        /// </summary>
        public Source Source { get; private set; }

        /// <summary>
        /// Gets the authorization method
        /// </summary>
        public AuthMethod AuthMethod { get; private set; }

        /// <summary>
        /// Gets the credit's date
        /// </summary>
        public DateTime CreditDate { get; private set; }

        /// <summary>
        /// Gets the parameters
        /// </summary>
        public AppParameters Parameters { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="CreateCreditDomainRequest"/>
        /// </summary>
        /// <param name="createCreditRequest"></param>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <param name="parameters"></param>

        public CreateCreditDomainRequest(CreateCreditRequest createCreditRequest, Customer customer, Store store,
            AppParameters parameters)
        {
            CreateCreditRequest = createCreditRequest;
            Customer = customer;
            Store = store;
            Parameters = parameters;
            CreditDate = DateTime.Now;
        }

        /// <summary>
        /// Sets the create credit transaction type
        /// </summary>
        /// <param name="createCreditTransactionType"></param>
        /// <returns></returns>
        public CreateCreditDomainRequest SetCreateCreditTransactionType(TransactionType createCreditTransactionType)
        {
            CreateCreditTransactionType = createCreditTransactionType;
            return this;
        }

        /// <summary>
        /// Sets the ordinary payment type
        /// </summary>
        /// <param name="ordinaryPaymentType"></param>
        /// <returns></returns>
        public CreateCreditDomainRequest SetOrdinaryPaymentType(PaymentType ordinaryPaymentType)
        {
            OrdinaryPaymentType = ordinaryPaymentType;
            return this;
        }

        /// <summary>
        /// Sets the seed masters
        /// </summary>
        /// <param name="status"></param>
        /// <param name="source"></param>
        /// <param name="authMethod"></param>
        /// <returns></returns>
        public CreateCreditDomainRequest SetSeedMasters(Status status, Source source, AuthMethod authMethod)
        {
            Status = status;
            Source = source;
            AuthMethod = authMethod;
            return this;
        }

        /// <summary>
        /// Sets the credit value
        /// </summary>
        /// <param name="creditValue"></param>
        public void SetCreditValue(decimal creditValue)
        {
            CreateCreditRequest.CreditValue = creditValue;
        }
    }
}