using Moq;
using org.reactivecommons.api;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Customers;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.EntryPoints.ServicesBus.Customer;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Helper.Test.ServiceBus;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.EntryPoints.ServiceBus.Tests.Customer
{
    public class CustomerUpdateMigrationEventSuscriptionTest
    {
        private readonly Mock<IDirectAsyncGateway<CustomerRequest>> _directAsyncCustomerRequestMock = new Mock<IDirectAsyncGateway<CustomerRequest>>();
        private readonly Mock<ICustomerService> _customerServiceMock = new Mock<ICustomerService>();
        private readonly Mock<ICommons> _commonsMock = new Mock<ICommons>();
        private readonly Mock<IAppParametersService> _appParametersServiceMock = new Mock<IAppParametersService>();

        private readonly Mock<ILoggerService<CustomerUpdateMigrationEventSubscription>> _loggerServiceMock =
            new Mock<ILoggerService<CustomerUpdateMigrationEventSubscription>>();

        private ICustomerUpdateMigrationEventSubscription CustomerUpdateMigrationEventSuscription =>
            new CustomerUpdateMigrationEventSubscription(_customerServiceMock.Object,
                _directAsyncCustomerRequestMock.Object,
                _commonsMock.Object,
                _loggerServiceMock.Object);

        public CustomerUpdateMigrationEventSuscriptionTest()
        {
            _appParametersServiceMock.Setup(mock => mock.GetSettings())
                .Returns(new CredinetAppSettings()
                {
                    StoreMonthLimitDefault = 9,
                    StoreAssurancePercentageDefault = 0.1M
                });

            _commonsMock.SetupGet(mock => mock.AppParameters)
                .Returns(_appParametersServiceMock.Object);

            _commonsMock.SetupGet(mock => mock.CredinetAppSettings)
                .Returns(_appParametersServiceMock.Object.GetSettings());
        }

        [Fact]
        public async Task ShouldGetCustomerFromSubscription()
        {
            bool called = false;
            void call() => called = true;
            ServiceBusHelperTest.SetupSubscribeEventCallbackWithDate(_directAsyncCustomerRequestMock, call);

            await CustomerUpdateMigrationEventSuscription.SubscribeAsync();

            Assert.True(called);
        }

        [Fact]
        public async Task ShouldInvokeCustomerResponse()
        {
            CustomerRequest customerRequest = ModelHelperTest.InstanceModel<CustomerRequest>();

            ServiceBusHelperTest.InvokeSubscriptionEventWithDate(_directAsyncCustomerRequestMock, customerRequest);

            await CustomerUpdateMigrationEventSuscription.SubscribeAsync();

            _customerServiceMock.Verify(mock => mock.CreateOrUpdateAsync(It.IsAny<CustomerRequest>(), It.IsAny<DateTime>()), Times.Once);
        }
    }
}