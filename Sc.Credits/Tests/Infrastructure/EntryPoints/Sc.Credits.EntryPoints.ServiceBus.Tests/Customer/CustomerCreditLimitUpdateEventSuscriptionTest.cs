using credinet.comun.models.Credits;
using Moq;
using org.reactivecommons.api;
using org.reactivecommons.api.domain;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Customers;
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
    public class CustomerCreditLimitUpdateEventSuscriptionTest
    {
        private readonly Mock<IDirectAsyncGateway<CreditLimitResponse>> _directAsyncCreditLimitResponseMock = new Mock<IDirectAsyncGateway<CreditLimitResponse>>();
        private readonly Mock<ICustomerService> _customerServiceMock = new Mock<ICustomerService>();
        private readonly Mock<ICommons> _commonsMock = new Mock<ICommons>();
        private readonly Mock<IAppParametersService> _appParametersServiceMock = new Mock<IAppParametersService>();

        private readonly Mock<ILoggerService<CustomerCreditLimitUpdateEventSubscription>> _loggerServiceMock =
            new Mock<ILoggerService<CustomerCreditLimitUpdateEventSubscription>>();

        private ICustomerCreditLimitUpdateEventSubscription CustomerEventSuscription =>
            new CustomerCreditLimitUpdateEventSubscription(_customerServiceMock.Object,
                _directAsyncCreditLimitResponseMock.Object,
                _commonsMock.Object,
                _loggerServiceMock.Object);

        public CustomerCreditLimitUpdateEventSuscriptionTest()
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
            bool creditLimitCalled = false;
            void creditLimitCall() => creditLimitCalled = true;
            ServiceBusHelperTest.SetupSubscribeEventCallbackWithDate(_directAsyncCreditLimitResponseMock, creditLimitCall);

            await CustomerEventSuscription.SubscribeAsync();

            Assert.True(creditLimitCalled);
        }

        [Fact]
        public async Task ShouldInvokeCreditLimitResponse()
        {
            CreditLimitResponse creditLimitResponse = ModelHelperTest.InstanceModel<CreditLimitResponse>();

            ServiceBusHelperTest.InvokeSubscriptionEventWithDate(_directAsyncCreditLimitResponseMock, new DomainEvent<CreditLimitResponse>("legacy.creditlimitupdate", creditLimitResponse.IdDocument, creditLimitResponse));

            await CustomerEventSuscription.SubscribeAsync();

            _customerServiceMock.Verify(mock => mock.TryCreditLimitUpdateAsync(It.IsAny<CreditLimitResponse>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task ShouldInvokeCreditLimitResponseNotProcess()
        {
            CreditLimitResponse creditLimitResponse = ModelHelperTest.InstanceModel<CreditLimitResponse>();

            ServiceBusHelperTest.InvokeSubscriptionEventWithDate(_directAsyncCreditLimitResponseMock, new DomainEvent<CreditLimitResponse>("credits.creditlimitupdate", creditLimitResponse.IdDocument, creditLimitResponse));

            await CustomerEventSuscription.SubscribeAsync();

            _customerServiceMock.Verify(mock => mock.TryCreditLimitUpdateAsync(It.IsAny<CreditLimitResponse>(), It.IsAny<DateTime>()), Times.Never);
        }
    }
}