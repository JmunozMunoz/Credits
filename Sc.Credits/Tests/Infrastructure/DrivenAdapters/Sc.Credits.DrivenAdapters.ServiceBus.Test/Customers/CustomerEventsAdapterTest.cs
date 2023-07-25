using credinet.comun.models.Credits;
using Moq;
using org.reactivecommons.api;
using org.reactivecommons.api.domain;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Customers.Gateway;
using Sc.Credits.DrivenAdapters.ServiceBus.Customers;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.Commons.Messaging;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.DrivenAdapters.ServiceBus.Test.Customers
{
    public class CustomerEventsAdapterTest
    {
        private readonly Mock<IDirectAsyncGateway<CreditLimitResponse>> _directAsyncGatewayCreditLimitResponseMock = new Mock<IDirectAsyncGateway<CreditLimitResponse>>();
        private readonly Mock<ISettings<CredinetAppSettings>> _appSettingsMock = new Mock<ISettings<CredinetAppSettings>>();
        private readonly Mock<IMessagingLogger> _messagingLoggerMock = new Mock<IMessagingLogger>();
        private readonly Mock<ILoggerService<CustomerEventsAdapter>> _loggerServiceMock = new Mock<ILoggerService<CustomerEventsAdapter>>();

        public ICustomerEventsRepository CustomerEventsRepository =>
            new CustomerEventsAdapter(_directAsyncGatewayCreditLimitResponseMock.Object,
                _appSettingsMock.Object, _loggerServiceMock.Object, _messagingLoggerMock.Object);

        public CustomerEventsAdapterTest()
        {
            _appSettingsMock.Setup(mock => mock.Get())
                .Returns(new CredinetAppSettings());
        }

        [Fact]
        public async Task ShouldNotifyCreditLimitUpdate()
        {
            Customer customer = CustomerHelperTest.GetCustomer();

            await CustomerEventsRepository.NotifyCreditLimitUpdateAsync(customer);

            _directAsyncGatewayCreditLimitResponseMock.Verify(mock => mock.SendEvent(It.IsAny<string>(), It.IsAny<DomainEvent<CreditLimitResponse>>()), Times.Once);
        }
    }
}