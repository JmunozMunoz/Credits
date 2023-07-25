using Moq;
using org.reactivecommons.api;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.EntryPoints.ServicesBus.Credits;
using Sc.Credits.Helper.Test.ServiceBus;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.EntryPoints.ServiceBus.Tests.Credits
{
    public class PayCreditsCommandSubscriptionTest
    {
        private readonly Mock<IDirectAsyncGateway<PayCreditsRequest>> _directAsyncPayCreditsMock = new Mock<IDirectAsyncGateway<PayCreditsRequest>>();
        private readonly Mock<ICreditPaymentService> _creditPaymentServiceMock = new Mock<ICreditPaymentService>();
        private readonly Mock<ICommons> _commonsMock = new Mock<ICommons>();
        private readonly Mock<IAppParametersService> _appParametersServiceMock = new Mock<IAppParametersService>();

        private readonly Mock<ILoggerService<PayCreditsCommandSubscription>> _loggerServiceMock =
            new Mock<ILoggerService<PayCreditsCommandSubscription>>();

        private IPayCreditsCommandSubscription _payCreditsCommandSubscription =>
            new PayCreditsCommandSubscription(_creditPaymentServiceMock.Object, _directAsyncPayCreditsMock.Object,
                                              _commonsMock.Object, _loggerServiceMock.Object);

        public PayCreditsCommandSubscriptionTest()
        {
            _appParametersServiceMock.Setup(mock => mock.GetSettings())
                .Returns(new CredinetAppSettings
                {
                    CultureInfo = "es-CO"
                });

            _commonsMock.SetupGet(mock => mock.AppParameters)
                .Returns(_appParametersServiceMock.Object);

            _commonsMock.SetupGet(mock => mock.CredinetAppSettings)
                .Returns(_appParametersServiceMock.Object.GetSettings());
        }

        [Fact]
        public async Task ShouldSubscribe()
        {
            bool updateChargesPaymentPlanCalled = false;
            void updateChargesPaymentPlanCallback() => updateChargesPaymentPlanCalled = true;
            ServiceBusHelperTest.SetupSubscribeCommandCallback(_directAsyncPayCreditsMock, Callback(updateChargesPaymentPlanCallback));

            await _payCreditsCommandSubscription.SubscribeAsync();

            Assert.True(updateChargesPaymentPlanCalled);
        }

        private Action Callback(Action handle)
        {
            return () => handle();
        }
    }
}