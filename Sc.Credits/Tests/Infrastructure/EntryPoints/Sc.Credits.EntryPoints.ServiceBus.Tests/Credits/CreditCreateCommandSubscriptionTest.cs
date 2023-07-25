using Moq;
using org.reactivecommons.api;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.EntryPoints.ServicesBus.Credits;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Helper.Test.ServiceBus;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.EntryPoints.ServiceBus.Tests.Credits
{
    public class CreditCreateCommandSubscriptionTest
    {
        private readonly Mock<IDirectAsyncGateway<CreateCreditResponse>> _directAsyncCreateCreditMock = new Mock<IDirectAsyncGateway<CreateCreditResponse>>();

        private readonly Mock<ICreditService> _creditServiceMock = new Mock<ICreditService>();
        private readonly Mock<ICommons> _commonsMock = new Mock<ICommons>();
        private readonly Mock<IAppParametersService> _appParametersServiceMock = new Mock<IAppParametersService>();

        private readonly Mock<ILoggerService<CreditCreateCommandSubscription>> _loggerServiceMock =
            new Mock<ILoggerService<CreditCreateCommandSubscription>>();

        private ICreditCreateCommandSubscription CreditCommandSubscription =>
            new CreditCreateCommandSubscription(_creditServiceMock.Object,
                _directAsyncCreateCreditMock.Object,
                _commonsMock.Object,
                _loggerServiceMock.Object);

        public CreditCreateCommandSubscriptionTest()
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
            bool updateCreateCreditCalled = false;
            void updateCreateCreditCallback() => updateCreateCreditCalled = true;
            ServiceBusHelperTest.SetupSubscribeCommandCallback(_directAsyncCreateCreditMock, Callback(updateCreateCreditCallback));

            await CreditCommandSubscription.SubscribeAsync();

            Assert.True(updateCreateCreditCalled);
        }

        private Action Callback(Action handle)
        {
            return () => handle();
        }

        [Fact]
        public async Task ShouldInvokeCreateCredit()
        {
            CreateCreditResponse createCreditResponse = CreditHelperTest.GetCreateCreditResponse();

            ServiceBusHelperTest.InvokeSubscriptionCommand(_directAsyncCreateCreditMock, createCreditResponse);

            await CreditCommandSubscription.SubscribeAsync();

            _creditServiceMock.Verify(mock => mock.CreditCreationNotifyAsync(createCreditResponse), Times.Once);
        }
    }
}