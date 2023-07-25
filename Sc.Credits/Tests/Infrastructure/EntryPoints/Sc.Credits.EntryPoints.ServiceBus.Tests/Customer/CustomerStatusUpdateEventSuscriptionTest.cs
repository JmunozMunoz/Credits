using credinet.comun.models.Study;
using Moq;
using org.reactivecommons.api;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Customers;
using Sc.Credits.EntryPoints.ServicesBus.Customer;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Helper.Test.ServiceBus;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.EntryPoints.ServiceBus.Tests.Customer
{
    public class CustomerStatusUpdateEventSuscriptionTest
    {
        private readonly Mock<IDirectAsyncGateway<StudyResponse>> _directAsyncStudyResponseMock = new Mock<IDirectAsyncGateway<StudyResponse>>();
        private readonly Mock<ICustomerService> _customerServiceMock = new Mock<ICustomerService>();
        private readonly Mock<ICommons> _commonsMock = new Mock<ICommons>();
        private readonly Mock<IAppParametersService> _appParametersServiceMock = new Mock<IAppParametersService>();

        private readonly Mock<ILoggerService<CustomerStatusUpdateEventSubscription>> _loggerServiceMock =
            new Mock<ILoggerService<CustomerStatusUpdateEventSubscription>>();

        private ICustomerStatusUpdateEventSubscription CustomerStatusUpdateEventSuscription =>
            new CustomerStatusUpdateEventSubscription(_customerServiceMock.Object,
                _directAsyncStudyResponseMock.Object,
                _commonsMock.Object,
                _loggerServiceMock.Object);

        public CustomerStatusUpdateEventSuscriptionTest()
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
            ServiceBusHelperTest.SetupSubscribeEventCallback(_directAsyncStudyResponseMock, call);

            await CustomerStatusUpdateEventSuscription.SubscribeAsync();

            Assert.True(called);
        }

        [Fact]
        public async Task ShouldInvokeCustomerResponse()
        {
            StudyResponse studyResponse = ModelHelperTest.InstanceModel<StudyResponse>();

            ServiceBusHelperTest.InvokeSubscriptionEvent(_directAsyncStudyResponseMock, studyResponse);

            await CustomerStatusUpdateEventSuscription.SubscribeAsync();

            _customerServiceMock.Verify(mock => mock.UpdateStatusAsync(It.IsAny<StudyResponse>()), Times.Once);
        }
    }
}