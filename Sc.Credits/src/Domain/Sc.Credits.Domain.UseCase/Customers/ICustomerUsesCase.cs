using Sc.Credits.Domain.Model.Customers;
using System;

namespace Sc.Credits.Domain.UseCase.Customers
{
    /// <summary>
    /// Customer uses case contract
    /// </summary>
    public interface ICustomerUsesCase
    {
        /// <summary>
        /// Create customer
        /// </summary>
        /// <param name="createCustomerRequest"></param>
        /// <param name="eventDate"></param>
        /// <returns></returns>
        Customer Create(CustomerRequest createCustomerRequest, DateTime eventDate);

        /// <summary>
        /// Update customer
        /// </summary>
        /// <param name="createCustomerRequest"></param>
        /// <param name="customer"></param>
        /// <returns></returns>
        void Update(CustomerRequest createCustomerRequest, Customer customer);

        /// <summary>
        /// Reject Credit Limit
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        void RejectCreditLimit(Customer customer);
    }
}