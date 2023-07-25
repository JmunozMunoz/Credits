using credinet.comun.models.Credits;
using credinet.comun.models.Study;
using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Moq;
using Sc.Credits.Domain.Managment.Services.Customers;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Customers.Gateway;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.UseCase.Customers;
using Sc.Credits.Helper.Test.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace Sc.Credits.Domain.Managment.Tests.Customers
{
    public class CustomerServiceTest
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock = new Mock<ICustomerRepository>();
        private readonly Mock<ICustomerUsesCase> _customerUsesCaseMock = new Mock<ICustomerUsesCase>();
        private readonly Mock<ICustomerEventsRepository> _customerEventsRepositoryMock = new Mock<ICustomerEventsRepository>();

        private ICustomerService CustomerService =>
            new CustomerService(_customerRepositoryMock.Object,
                _customerUsesCaseMock.Object,
                _customerEventsRepositoryMock.Object);

        public CustomerServiceTest()
        {
        }

        /// <summary>
        /// Should create customer
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldCreateCustomer()
        {
            Customer customer = CustomerHelperTest.GetCustomerCreditLimit();
            CustomerRequest customerRequest = CustomerRequestHelperTest.GetCustomerRequest();

            bool modified = false;

            _customerRepositoryMock.Setup(cr => cr.GetByDocumentInfoAsync(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()));

            _customerUsesCaseMock.Setup(cu => cu.Create(It.IsAny<CustomerRequest>(), It.IsAny<DateTime>())).Returns(customer);

            _customerRepositoryMock.Setup(cm => cm.AddAsync(customer, It.IsAny<Transaction>())).Callback(() => modified = true);

            await CustomerService.CreateOrUpdateAsync(customerRequest, DateTime.Now);

            _customerUsesCaseMock.Verify(item => item.Create(It.IsAny<CustomerRequest>(), It.IsAny<DateTime>()), Times.Once());

            Assert.True(modified);
        }

        /// <summary>
        /// Should update customer
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldUpdateCustomer()
        {
            bool modified = false;

            Customer customer = CustomerHelperTest.GetCustomerCreditLimit();
            CustomerRequest customerRequest = CustomerRequestHelperTest.GetCustomerRequest();

            _customerRepositoryMock.Setup(cr => cr.GetByDocumentInfoAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .Returns(Task.FromResult(customer));

            _customerRepositoryMock.Setup(cm => cm.UpdateAsync(It.IsAny<Customer>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<Transaction>(), It.IsAny<CustomerName>())).Callback(() => modified = true);

            await CustomerService.CreateOrUpdateAsync(customerRequest, DateTime.Now);

            _customerUsesCaseMock.Verify(item => item.Update(It.IsAny<CustomerRequest>(), It.IsAny<Customer>()), Times.Once());

            Assert.True(modified);
        }

        /// <summary>
        /// Should get customer
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldGetCustomer()
        {
            Customer customer = CustomerHelperTest.GetCustomer();

            _customerRepositoryMock.Setup(cr => cr.GetByDocumentInfoAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customer);

            Customer actual = await CustomerService.GetAsync(customer.IdDocument, customer.DocumentType,
                Enumerable.Empty<Field>());

            Assert.IsType<Customer>(customer);
            Assert.Equal(customer.GetFullName, actual.GetFullName);
            Assert.Equal(customer.GetStatus, actual.GetStatus);
        }

        /// <summary>
        /// Should get customer active
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldGetCustomerActive()
        {
            Customer customer = CustomerHelperTest.GetCustomer();

            _customerRepositoryMock.Setup(cr => cr.GetByDocumentInfoAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customer);

            Customer actual = await CustomerService.GetActiveAsync(customer.IdDocument, customer.DocumentType,
                Enumerable.Empty<Field>());

            Assert.IsType<Customer>(customer);
            Assert.Equal(customer.GetFullName, actual.GetFullName);
            Assert.Equal(customer.GetStatus, actual.GetStatus);
        }

        /// <summary>
        /// Should get customer throws not found exception
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldGetCustomerThrowsNotFoundExecption()
        {
            Customer customer = CustomerHelperTest.GetCustomer();
            Customer customerResult = null;

            _customerRepositoryMock.Setup(cr => cr.GetByDocumentInfoAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customerResult);

            BusinessException exception = await Assert.ThrowsAsync<BusinessException>(() =>
                CustomerService.GetAsync(customer.IdDocument, customer.DocumentType,
                    Enumerable.Empty<Field>()));

            Assert.Equal((int)BusinessResponse.CustomerNotFound, exception.code);
        }

        /// <summary>
        /// Should get customer active throws not found exception
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldGetCustomerActiveThrowsNotFoundException()
        {
            Customer customer = CustomerHelperTest.GetCustomer();
            Customer customerResult = null;

            _customerRepositoryMock.Setup(cr => cr.GetByDocumentInfoAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customerResult);

            BusinessException exception = await Assert.ThrowsAsync<BusinessException>(() =>
                CustomerService.GetActiveAsync(customer.IdDocument, customer.DocumentType,
                    Enumerable.Empty<Field>()));

            Assert.Equal((int)BusinessResponse.CustomerNotFound, exception.code);
        }

        /// <summary>
        /// Should get customer active throws incomplete exception
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldGetCustomerActiveThrowsIncompleteException()
        {
            Customer customer = CustomerHelperTest.GetCustomerByStatus(CustomerStatuses.IncompleteRequest);

            _customerRepositoryMock.Setup(cr => cr.GetByDocumentInfoAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customer);

            BusinessException exception = await Assert.ThrowsAsync<BusinessException>(() =>
                CustomerService.GetActiveAsync(customer.IdDocument, customer.DocumentType,
                    Enumerable.Empty<Field>()));

            Assert.Equal((int)BusinessResponse.CustomerWithIncompleteInfo, exception.code);
        }

        /// <summary>
        /// Should get customer active throws not active exception
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldGetCustomerActiveThrowsNotActiveException()
        {
            Customer customer = CustomerHelperTest.GetCustomerByStatus(CustomerStatuses.Denied);

            _customerRepositoryMock.Setup(cr => cr.GetByDocumentInfoAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customer);

            BusinessException exception = await Assert.ThrowsAsync<BusinessException>(() =>
                CustomerService.GetActiveAsync(customer.IdDocument, customer.DocumentType,
                    Enumerable.Empty<Field>()));

            Assert.Equal((int)BusinessResponse.CustomerNotActive, exception.code);
        }

        /// <summary>
        /// Should try send credit limit update send
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldTrySendCreditLimitUpdateSend()
        {
            Customer customer = CustomerHelperTest.GetCustomerByStatus(CustomerStatuses.Approved);

            customer.CreditLimitIncrease(100);

            await CustomerService.TrySendCreditLimitUpdateAsync(customer);

            _customerEventsRepositoryMock.Verify(mock => mock.NotifyCreditLimitUpdateAsync(It.IsAny<Customer>()), Times.Once);
        }

        /// <summary>
        /// Should try send credit limit update not send
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldTrySendCreditLimitUpdateNotSend()
        {
            Customer customer = CustomerHelperTest.GetCustomerByStatus(CustomerStatuses.Approved);

            await CustomerService.TrySendCreditLimitUpdateAsync(customer);

            _customerEventsRepositoryMock.Verify(mock => mock.NotifyCreditLimitUpdateAsync(It.IsAny<Customer>()), Times.Never);
        }

        /// <summary>
        /// Should try credit limit update
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldTryCreditLimitUpdate()
        {
            Customer customer = CustomerHelperTest.GetCustomerByStatus(CustomerStatuses.Approved);

            CreditLimitResponse creditLimitResponse = new CreditLimitResponse
            {
                AvailableCreditLimit = 1000,
                CreditLimit = 200,
                IdDocument = customer.IdDocument,
                TypeDocument = customer.DocumentType
            };

            bool modified = false;

            _customerRepositoryMock.Setup(cr => cr.GetByDocumentInfoAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customer);

            _customerRepositoryMock.Setup(cm => cm.UpdateAsync(It.IsAny<Customer>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<Transaction>())).Callback(() => modified = true);

            await CustomerService.TryCreditLimitUpdateAsync(creditLimitResponse, DateTime.Now.AddMinutes(1));

            Assert.True(modified);
        }

        /// <summary>
        /// Should try credit limit not update
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldTryCreditLimitNotUpdate()
        {
            Customer customer = CustomerHelperTest.GetCustomerByStatus(CustomerStatuses.Approved);

            CreditLimitResponse creditLimitResponse = new CreditLimitResponse
            {
                AvailableCreditLimit = 1000,
                CreditLimit = 200,
                IdDocument = customer.IdDocument,
                TypeDocument = customer.DocumentType
            };

            bool modified = false;

            _customerRepositoryMock.Setup(cr => cr.GetByDocumentInfoAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customer);

            _customerRepositoryMock.Setup(cm => cm.UpdateAsync(It.IsAny<Customer>(),
                It.IsAny<IEnumerable<Field>>(), It.IsAny<Transaction>())).Callback(() => modified = true);

            await CustomerService.TryCreditLimitUpdateAsync(creditLimitResponse, DateTime.Now.AddDays(-1));

            Assert.False(modified);
        }

        /// <summary>
        /// Should update status
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldUpdateStatus()
        {
            Customer customer = CustomerHelperTest.GetCustomerByStatus(CustomerStatuses.Approved);

            bool modified = false;

            _customerRepositoryMock.Setup(cr => cr.GetByDocumentInfoAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customer);

            _customerRepositoryMock.Setup(cm => cm.UpdateAsync(It.IsAny<Customer>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<Transaction>())).Callback(() => modified = true);

            await CustomerService.UpdateStatusAsync(new StudyResponse
            {
                Status = (int)CustomerStatuses.IncompleteRequest
            });

            Assert.True(modified);
            Assert.Equal((int)CustomerStatuses.IncompleteRequest, customer.GetStatus);
        }

        /// <summary>
        /// Should update mail and validate
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldUpdateMailAndValidate()
        {
            Customer customer = CustomerHelperTest.GetCustomerUnvalidatedMail();

            bool modified = false;

            _customerRepositoryMock.Setup(cr => cr.GetByDocumentInfoAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customer);

            _customerRepositoryMock.Setup(cm => cm.UpdateAsync(It.IsAny<Customer>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<Transaction>())).Callback(() => modified = true);

            await CustomerService.UpdateMailAsync("TestId", "CC", "test@test.com", true);

            Assert.True(modified);
            Assert.True(customer.GetValidatedMail);
            Assert.Equal("test@test.com", customer.GetEmail);
        }

        /// <summary>
        /// Should update mail and not invalidate
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldUpdateMailAndNotInvalidate()
        {
            Customer customer = CustomerHelperTest.GetCustomer();

            bool modified = false;

            _customerRepositoryMock.Setup(cr => cr.GetByDocumentInfoAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customer);

            _customerRepositoryMock.Setup(cm => cm.UpdateAsync(It.IsAny<Customer>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<Transaction>())).Callback(() => modified = true);

            await CustomerService.UpdateMailAsync("TestId", "CC", "test@test.com", false);

            Assert.True(modified);
            Assert.True(customer.GetValidatedMail);
            Assert.Equal("test@test.com", customer.GetEmail);
        }

        /// <summary>
        /// Should update mail and confirm invalidate
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldUpdateMailAndConfirmInvalidate()
        {
            Customer customer = CustomerHelperTest.GetCustomerUnvalidatedMail();

            bool modified = false;

            _customerRepositoryMock.Setup(cr => cr.GetByDocumentInfoAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customer);

            _customerRepositoryMock.Setup(cm => cm.UpdateAsync(It.IsAny<Customer>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<Transaction>())).Callback(() => modified = true);

            await CustomerService.UpdateMailAsync("TestId", "CC", "test@test.com", false);

            Assert.True(modified);
            Assert.False(customer.GetValidatedMail);
            Assert.Equal("test@test.com", customer.GetEmail);
        }

        /// <summary>
        /// Should create customer and not update status
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldCreateCustomerAndNotUpdateStatus()
        {
            Customer customer = CustomerHelperTest.GetCustomerCreditLimit();
            CustomerRequest customerRequest = CustomerRequestHelperTest.GetCustomerRequest();

            bool modified = false;

            _customerUsesCaseMock.Setup(cu => cu.Create(It.IsAny<CustomerRequest>(), It.IsAny<DateTime>())).Returns(customer);

            _customerRepositoryMock.Setup(cm => cm.AddAsync(It.IsAny<Customer>(),
                It.IsAny<Transaction>())).Callback(() => modified = true);

            await CustomerService.CreateOrUpdateStatusAsync(customerRequest, DateTime.Now);

            _customerUsesCaseMock.Verify(item => item.Create(It.IsAny<CustomerRequest>(), It.IsAny<DateTime>()), Times.Once());

            Assert.True(modified);
        }

        /// <summary>
        /// Should not create customer and update status
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldNotCreateCustomerAndUpdateStatus()
        {
            Customer customer = CustomerHelperTest.GetCustomerByStatus(CustomerStatuses.Study);
            CustomerRequest customerRequest = CustomerRequestHelperTest.GetCustomerRequest();
            customerRequest.Status = (int)CustomerStatuses.Approved;

            bool modified = false;

            _customerRepositoryMock.Setup(cr => cr.GetByDocumentInfoAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customer);

            _customerRepositoryMock.Setup(cm => cm.UpdateAsync(It.IsAny<Customer>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<Transaction>(), It.IsAny<CustomerName>())).Callback(() => modified = true);

            await CustomerService.CreateOrUpdateStatusAsync(customerRequest, DateTime.Now);

            Assert.True(modified);
            Assert.Equal((int)CustomerStatuses.Approved, customer.GetStatus);
        }

        /// <summary>
        /// Reject credit limit without data
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task RejectCreditLimit_WithoutData()
        {
            //Arrange
            string idDocument = "";
            string documentType = null;
            string userName = "";
            string userId = "";

            //Act
            BusinessException exception = await Assert.ThrowsAsync<BusinessException>(() => CustomerService.RejectCreditLimit(idDocument, documentType, userName, userId));

            //Assert
            Assert.Equal((int)BusinessResponse.RequestValuesInvalid, exception.code);
        }

        /// <summary>
        /// Credit limit lower
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void LowerCreditLimit()
        {
            //Arrange
            Customer customer = CustomerHelperTest.GetCustomer();

            //Act
            customer.ReduceCreditLimit(customer.GetCreditLimit);
            var newAvailableCreditLimit = customer.GetAvailableCreditLimit - customer.GetCreditLimit;

            //Assert
            Assert.Equal(0, customer.GetCreditLimit);
            Assert.Equal(newAvailableCreditLimit, customer.GetAvailableCreditLimit);
        }

        /// <summary>
        /// Should update mobile
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldUpdateMobile()
        {
            Customer customer = CustomerHelperTest.GetCustomer();

            bool modified = false;

            _customerRepositoryMock.Setup(cr => cr.GetByDocumentInfoAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(), It.IsAny<IEnumerable<Field>>()))
                .ReturnsAsync(customer);

            _customerRepositoryMock.Setup(cm => cm.UpdateAsync(It.IsAny<Customer>(), It.IsAny<IEnumerable<Field>>(),
                It.IsAny<Transaction>())).Callback(() => modified = true);

            await CustomerService.UpdateMobileAsync("TestId", "CC", "999999999");

            Assert.True(modified);
            Assert.Equal("999999999", customer.GetMobile);
        }

        [Fact]
        public async Task ShouldGetCustomerIdDocumentEmptyThrowsRequestValuesInvalidException() =>
            await Assert.ThrowsAsync<BusinessException>(() => CustomerService.GetAsync(string.Empty, "CC",
                Enumerable.Empty<Field>()));

        [Fact]
        public async Task ShouldGetCustomerTypeDocumentEmptyThrowsRequestValuesInvalidException() =>
            await Assert.ThrowsAsync<BusinessException>(() => CustomerService.GetAsync("1058898747", string.Empty,
                Enumerable.Empty<Field>()));

        [Fact]
        public async Task ShouldGetCustomerActiveIdDocumentEmptyThrowsRequestValuesInvalidException() =>
           await Assert.ThrowsAsync<BusinessException>(() => CustomerService.GetActiveAsync(string.Empty, "CC",
               Enumerable.Empty<Field>()));

        [Fact]
        public async Task ShouldGetCustomerActiveAsTypeDocumentEmptyThrowsRequestValuesInvalidException() =>
            await Assert.ThrowsAsync<BusinessException>(() => CustomerService.GetActiveAsync("1058898747", string.Empty,
                Enumerable.Empty<Field>()));
    }
}