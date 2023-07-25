using Moq;
using org.reactivecommons.api;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.EntryPoints.ServicesBus.Credits;
using Sc.Credits.EntryPoints.ServicesBus.Model;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Helper.Test.ServiceBus;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.EntryPoints.ServiceBus.Tests.Credits
{
    public class CreditCommandSubscriptionTest
    {
        private readonly Mock<IDirectAsyncGateway<ChargesUpdatedPaymentPlanValueRequest>> _directAsyncChargesUpdatedPaymentPlanMock = new Mock<IDirectAsyncGateway<ChargesUpdatedPaymentPlanValueRequest>>();
        private readonly Mock<ICreditService> _creditServiceMock = new Mock<ICreditService>();
        private readonly Mock<ICommons> _commonsMock = new Mock<ICommons>();
        private readonly Mock<IAppParametersService> _appParametersServiceMock = new Mock<IAppParametersService>();

        private readonly Mock<ILoggerService<CreditCommandSubscription>> _loggerServiceMock =
            new Mock<ILoggerService<CreditCommandSubscription>>();

        private ICreditCommandSubscription CreditCommandSubscription =>
            new CreditCommandSubscription(_creditServiceMock.Object,
                _directAsyncChargesUpdatedPaymentPlanMock.Object,
                _commonsMock.Object,
                _loggerServiceMock.Object);

        public CreditCommandSubscriptionTest()
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
            ServiceBusHelperTest.SetupSubscribeCommandCallback(_directAsyncChargesUpdatedPaymentPlanMock, Callback(updateChargesPaymentPlanCallback));

            await CreditCommandSubscription.SubscribeAsync();

            Assert.True(updateChargesPaymentPlanCalled);
        }

        private Action Callback(Action handle)
        {
            return () => handle();
        }

        [Fact]
        public async Task ShouldInvokeUpdateChargesPaymentPlan()
        {
            ChargesUpdatedPaymentPlanValueRequest chargesUpdatedPaymentPlanValueRequest = ModelHelperTest
                .InstanceModel<ChargesUpdatedPaymentPlanValueRequest>();
            chargesUpdatedPaymentPlanValueRequest.CreditId = Guid.NewGuid().ToString();

            ServiceBusHelperTest.InvokeSubscriptionCommand(_directAsyncChargesUpdatedPaymentPlanMock, chargesUpdatedPaymentPlanValueRequest);

            await CreditCommandSubscription.SubscribeAsync();

            _creditServiceMock.Verify(mock => mock.UpdateChargesPaymentPlanValueAsync(new Guid(chargesUpdatedPaymentPlanValueRequest.CreditId),
                    (decimal)chargesUpdatedPaymentPlanValueRequest.ChargeValue, chargesUpdatedPaymentPlanValueRequest.HasArrearsCharge,
                    (decimal)chargesUpdatedPaymentPlanValueRequest.ArrearsCharge, (decimal)chargesUpdatedPaymentPlanValueRequest.UpdatedPaymentPlanValue),
                Times.Once);
        }
    }
}