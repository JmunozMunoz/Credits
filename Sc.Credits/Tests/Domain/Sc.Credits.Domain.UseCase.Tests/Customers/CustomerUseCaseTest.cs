using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.UseCase.Customers;
using Sc.Credits.Helper.Test.Model;
using System;
using Xunit;

namespace Sc.Credits.Domain.UseCase.Tests.Customers
{
    public class CustomerUseCaseTest
    {
        public ICustomerUsesCase CustomerUsesCase
        {
            get
            {
                return new CustomerUsesCase();
            }
        }

        /// <summary>
        ///Customer create test
        /// </summary>
        [Fact]
        public void ShouldCustomerCreateTest()
        {
            CustomerRequest customerRequest = CustomerRequestHelperTest.GetCustomerRequest();
            customerRequest.FirstName = "John";

            Customer customer = CustomerUsesCase.Create(customerRequest, DateTime.Now);

            Assert.IsType<Customer>(customer);
            Assert.Equal(customerRequest.IdDocument, customer.IdDocument);
            Assert.Equal(customerRequest.FirstName, customer.Name.GetFirstName);
            Assert.Equal(customerRequest.ProfileId, customer.GetProfileId);
            Assert.Equal(customerRequest.Amount, customer.GetCreditLimit);
        }

        /// <summary>
        /// Customer update test
        /// </summary>
        [Fact]
        public void ShouldCustomerUpdateTest()

        {
            Customer customer = CustomerHelperTest.GetCustomerCreditLimitTrue();
            CustomerRequest customerRequest = CustomerRequestHelperTest.GetCustomerRequest();
            customerRequest.FirstName = "John";

            CustomerUsesCase.Update(customerRequest, customer);

            Assert.IsType<Customer>(customer);
            Assert.Equal(customerRequest.FirstName, customer.Name.GetFirstName);
            Assert.Equal(customerRequest.ProfileId, customer.GetProfileId);
        }

        [Fact]
        public void ShouldThrowBusinessExeptionByCustomerId()
        {
            CustomerRequest customerRequest = CustomerRequestHelperTest.GetCustomerRequestForEmptyCustomerId();

            Assert.Throws<BusinessException>(() => CustomerUsesCase.Create(customerRequest, DateTime.Now));
        }

        [Fact]
        public void RejectCreditLimit_ShouldSetCreditLimitToZero()
        {
            //Arrange
            Customer customer = CustomerHelperTest.GetCustomer();

            var newAvailableCreditLimit = customer.GetAvailableCreditLimit - customer.GetCreditLimit;

            //Act
            CustomerUsesCase.RejectCreditLimit(customer);

            //Assert
            Assert.Equal(0, customer.GetCreditLimit);
            Assert.Equal(newAvailableCreditLimit, customer.GetAvailableCreditLimit);
        }

        [Fact]
        public void RejectCreditLimit_CreditLimitInZero_ThrowBusinessExeption()
        {
            //Arrange
            Customer customer = CustomerHelperTest.GetCustomerWithoutCreditLimit();

            //Act
            BusinessException exception = Assert.Throws<BusinessException>(() => CustomerUsesCase.RejectCreditLimit(customer));

            //Assert
            Assert.Equal((int)BusinessResponse.CreditLimitAlreadyRejected, exception.code);
        }
    }
}