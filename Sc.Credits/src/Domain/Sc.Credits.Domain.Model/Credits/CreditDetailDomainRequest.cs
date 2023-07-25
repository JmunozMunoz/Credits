using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.Model.Stores;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// Credit detail domain request
    /// </summary>
    public class CreditDetailDomainRequest : GeneralCreditDetailDomainRequest
    {
        /// <summary>
        /// Customer
        /// </summary>
        public Customer Customer { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="CreditDetailDomainRequest"/>
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <param name="creditValue"></param>
        /// <param name="frequency"></param>
        /// <param name="appParameters"></param>
        public CreditDetailDomainRequest(Customer customer, Store store, decimal creditValue, int frequency,
            AppParameters appParameters) : base(store, creditValue, frequency, appParameters)
        {
            Customer = customer;
        }
    }
}