using Moq;
using org.reactivecommons.api;
using org.reactivecommons.api.domain;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Customers;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.EntryPoints.ServicesBus.Customer;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Helper.Test.ServiceBus;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.EntryPoints.ServiceBus.Tests.Customer
{
    public class CustomerUpdateMailMobileEventSuscriptionTest
    {
        private readonly Mock<IDirectAsyncGateway<CustomerRequest>> _directAsyncCustomerRequestMock = new Mock<IDirectAsyncGateway<CustomerRequest>>();
        private readonly Mock<ICustomerService> _customerServiceMock = new Mock<ICustomerService>();
        private readonly Mock<ICommons> _commonsMock = new Mock<ICommons>();
        private readonly Mock<IAppParametersService> _appParametersServiceMock = new Mock<IAppParametersService>();

        private readonly Mock<ILoggerService<CustomerUpdateMailMobileEventSubscription>> _loggerServiceMock =
            new Mock<ILoggerService<CustomerUpdateMailMobileEventSubscription>>();

        private ICustomerUpdateMailMobileEventSubscription CustomerUpdateMailMobileEventSuscription =>
            new CustomerUpdateMailMobileEventSubscription(_customerServiceMock.Object,
                _directAsyncCustomerRequestMock.Object,
                _commonsMock.Object,
                _loggerServiceMock.Object);

        public CustomerUpdateMailMobileEventSuscriptionTest()
        {
            _appParametersServiceMock.Setup(mock => mock.GetSettings())
                .Returns(new CredinetAppSettings()
                {
                    StoreMonthLimitDefault = 9,
                    StoreAssurancePercentageDefault = 0.1M,
                    DomainName = "Credits"
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

            await CustomerUpdateMailMobileEventSuscription.SubscribeAsync();

            Assert.True(called);
        }

        [Fact]
        public async Task ShouldInvokeChangeMail()
        {
            CustomerRequest customerRequest = ModelHelperTest.InstanceModel<CustomerRequest>();

            DomainEvent<CustomerRequest> domainEvent = new DomainEvent<CustomerRequest>("Legacy.ChangeMail", "TestId", customerRequest);

            ServiceBusHelperTest.InvokeSubscriptionEventWithDate(_directAsyncCustomerRequestMock, domainEvent);

            await CustomerUpdateMailMobileEventSuscription.SubscribeAsync();

            _customerServiceMock.Verify(mock => mock.UpdateMailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task ShouldInvokeChangeMobile()
        {
            CustomerRequest customerRequest = ModelHelperTest.InstanceModel<CustomerRequest>();

            DomainEvent<CustomerRequest> domainEvent = new DomainEvent<CustomerRequest>("Legacy.ChangeMobile", "TestId", customerRequest);

            ServiceBusHelperTest.InvokeSubscriptionEventWithDate(_directAsyncCustomerRequestMock, domainEvent);

            await CustomerUpdateMailMobileEventSuscription.SubscribeAsync();

            _customerServiceMock.Verify(mock => mock.UpdateMobileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}