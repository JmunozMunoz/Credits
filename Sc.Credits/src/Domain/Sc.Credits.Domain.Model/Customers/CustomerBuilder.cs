using System;

namespace Sc.Credits.Domain.Model.Customers
{
    /// <summary>
    /// Customer builder
    /// </summary>
    public class CustomerBuilder
    {
        private readonly Customer _customer;

        /// <summary>
        /// Creates a new instance of <see cref="CustomerBuilder"/>
        /// </summary>
        /// <returns></returns>
        public static CustomerBuilder CreateBuilder()
        {
            return new CustomerBuilder();
        }

        /// <summary>
        /// New builder
        /// </summary>
        private CustomerBuilder()
        {
            _customer = Customer.New();
        }

        /// <summary>
        /// Set customer basic info
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idDocument"></param>
        /// <param name="documentType"></param>
        /// <param name="name"></param>
        /// <param name="mobile"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public CustomerBuilder BasicInfo(Guid id, string idDocument, string documentType, CustomerName name,
            string mobile, string email)
        {
            _customer.SetBasicInfo(id, idDocument, documentType, name, mobile, email);

            return this;
        }

        /// <summary>
        /// Set customer validation info
        /// </summary>
        /// <param name="validatedMail"></param>
        /// <param name="profileId"></param>
        /// <param name="sendTokenMail"></param>
        /// <param name="sendTokenSms"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public CustomerBuilder ValidationInfo(bool validatedMail, int profileId, bool sendTokenMail, bool sendTokenSms, int status)
        {
            _customer.SetValidationInfo(validatedMail, profileId, sendTokenMail, sendTokenSms, status);

            return this;
        }

        /// <summary>
        /// Set customer credit limit info
        /// </summary>
        /// <param name="creditLimit"></param>
        /// <param name="availableCreditLimit"></param>
        /// <param name="eventDate"></param>
        /// <returns></returns>
        public CustomerBuilder CreditLimitInfo(decimal creditLimit, decimal availableCreditLimit, DateTime eventDate)
        {
            _customer.SetCreditLimitInfo(creditLimit, availableCreditLimit, eventDate);

            return this;
        }

        /// <summary>
        /// Build
        /// </summary>
        /// <returns></returns>
        public Customer Build() => _customer;
    }
}