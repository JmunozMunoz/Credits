using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Sc.Credits.Domain.Model.Customers;
using System;

namespace Sc.Credits.Domain.UseCase.Customers
{
    /// <summary>
    /// Customer uses case is an implementation of <see cref="ICustomerUsesCase"/>
    /// </summary>
    public class CustomerUsesCase : ICustomerUsesCase
    {
        /// <summary>
        /// <see cref="ICustomerUsesCase.Create(CustomerRequest, DateTime)"/>
        /// </summary>
        /// <param name="createCustomerRequest"></param>
        /// <param name="eventDate"></param>
        /// <returns></returns>
        public Customer Create(CustomerRequest createCustomerRequest, DateTime eventDate)
        {
            if (createCustomerRequest.CustomerId == Guid.Empty)
            {
                throw new BusinessException(nameof(BusinessResponse.RequestValuesInvalid), (int)BusinessResponse.RequestValuesInvalid);
            }

            decimal amount = createCustomerRequest.Amount ?? 0;
            bool validatedMail = createCustomerRequest.VerifiedEmail ?? false;
            bool sendTokenMail = createCustomerRequest.SendTokenMail ?? false;
            bool sendTokenSms = createCustomerRequest.SendTokenSms ?? true;
            int profileId = createCustomerRequest.ProfileId ?? 1;
            CustomerName customerName = CustomerName.New(
                    createCustomerRequest.FirstName, createCustomerRequest.SecondName, createCustomerRequest.FirstLastName, createCustomerRequest.SecondLastName);

            return
                CustomerBuilder
                    .CreateBuilder()
                    .BasicInfo(createCustomerRequest.CustomerId, createCustomerRequest.IdDocument, createCustomerRequest.TypeDocument, customerName,
                        createCustomerRequest.MovileNumber, createCustomerRequest.Email)
                    .ValidationInfo(validatedMail, profileId, sendTokenMail, sendTokenSms, createCustomerRequest.Status)
                    .CreditLimitInfo(amount, createCustomerRequest.AmountAvailable, eventDate)
                    .Build();
        }

        /// <summary>
        /// <see cref="ICustomerUsesCase.Update(CustomerRequest, Customer)"/>
        /// </summary>
        /// <param name="createCustomerRequest"></param>
        /// <param name="customer"></param>
        /// <returns></returns>
        public void Update(CustomerRequest createCustomerRequest, Customer customer)
        {
            customer.Update(createCustomerRequest);
        }

        /// <summary>
        /// <see cref="ICustomerUsesCase.RejectCreditLimit(Customer)"/>
        /// </summary>
        /// <param name="customer"></param>
        public void RejectCreditLimit(Customer customer)
        {
            if (customer.GetCreditLimit == 0)
            {
                throw new BusinessException(BusinessResponse.CreditLimitAlreadyRejected.ToString(), (int)BusinessResponse.CreditLimitAlreadyRejected);
            }

            customer.ReduceCreditLimit(customer.GetCreditLimit);
        }
    }
}