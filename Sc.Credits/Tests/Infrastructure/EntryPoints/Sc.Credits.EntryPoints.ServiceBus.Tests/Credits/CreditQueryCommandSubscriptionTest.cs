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
    public class CreditQueryCommandSubscriptionTest
    {
        private readonly Mock<IDirectAsyncGateway<CalculatedQuery>> _directAsyncCreditsResponseMock = new Mock<IDirectAsyncGateway<CalculatedQuery>>();

        private readonly Mock<ICreditPaymentService> _creditPaymentServiceMock = new Mock<ICreditPaymentService>();
        private readonly Mock<ICommons> _commonsMock = new Mock<ICommons>();
        private readonly Mock<IAppParametersService> _appParametersServiceMock = new Mock<IAppParametersService>();

        private readonly Mock<ILoggerService<CreditQueryCommandSubscription>> _loggerServiceMock =
            new Mock<ILoggerService<CreditQueryCommandSubscription>>();

        private ICreditQueryCommandSubscription CreditCommandSubscription =>
            new CreditQueryCommandSubscription(_creditPaymentServiceMock.Object,
                _directAsyncCreditsResponseMock.Object,
                _commonsMock.Object,
                _loggerServiceMock.Object);

        public CreditQueryCommandSubscriptionTest()
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
            bool updateCreditResponseCalled = false;
            void updateCreditResponseCallback() => updateCreditResponseCalled = true;
            ServiceBusHelperTest.SetupResponseReplyCallback<CalculatedQuery, CalculatedQuery>(_directAsyncCreditsResponseMock, Callback(updateCreditResponseCallback));

            await CreditCommandSubscription.SubscribeAsync();

            Assert.True(updateCreditResponseCalled);
        }

        private Action Callback(Action handle)
        {
            return () => handle();
        }

        [Fact]
        public async Task ShouldInvokeCreditResponse()
        {
            CalculatedQuery calculatedQuery = ModelHelperTest.InstanceModel<CalculatedQuery>();
            calculatedQuery.CreditId = Guid.NewGuid().ToString();

            CalculatedQuery calculatedQueryReturns = ModelHelperTest.InstanceModel<CalculatedQuery>();

            CalculatedQuery calculatedQueryResponse = null;
            void callback(CalculatedQuery result) =>
                calculatedQueryResponse = result;

            _creditPaymentServiceMock.Setup(mock => mock.GetDataCalculateCreditAsync(It.IsAny<Guid>(), It.IsAny<DateTime>()))
                .ReturnsAsync(calculatedQueryReturns);

            ServiceBusHelperTest.InvokeResponseReply<CalculatedQuery, CalculatedQuery>(_directAsyncCreditsResponseMock, calculatedQuery, callback);

            await CreditCommandSubscription.SubscribeAsync();

            Assert.NotNull(calculatedQueryResponse);
        }
    }
}