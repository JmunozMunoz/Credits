using Moq;
using org.reactivecommons.api;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Stores;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.EntryPoints.ServicesBus.Store;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Helper.Test.ServiceBus;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.EntryPoints.ServiceBus.Tests.Store
{
    public class StoreCommandSubscriptionTest
    {
        private readonly Mock<IDirectAsyncGateway<StoreRequest>> _directAsyncMock = new Mock<IDirectAsyncGateway<StoreRequest>>();
        private readonly Mock<IStoreService> _storeServiceMock = new Mock<IStoreService>();
        private readonly Mock<ICommons> _commonsMock = new Mock<ICommons>();
        private readonly Mock<IAppParametersService> _appParametersServiceMock = new Mock<IAppParametersService>();

        private readonly Mock<ILoggerService<StoreCommandSubscription>> _loggerServiceMock =
            new Mock<ILoggerService<StoreCommandSubscription>>();

        private IStoreCommandSubscription StoreCommandSubscription =>
            new StoreCommandSubscription(_storeServiceMock.Object,
                _directAsyncMock.Object,
                _commonsMock.Object,
                _loggerServiceMock.Object);

        public StoreCommandSubscriptionTest()
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
        public async Task ShouldGetStoreFromSubscription()
        {
            bool called = false;
            void call() => called = true;
            ServiceBusHelperTest.SetupSubscribeCommandCallback(_directAsyncMock, call);

            await StoreCommandSubscription.SubscribeAsync();

            Assert.True(called);
        }

        [Fact]
        public async Task ShoulInvokeStoreMessage()
        {
            StoreRequest storeRequest = ModelHelperTest.InstanceModel<StoreRequest>();

            ServiceBusHelperTest.InvokeSubscriptionCommand(_directAsyncMock, storeRequest);

            await StoreCommandSubscription.SubscribeAsync();

            _storeServiceMock.Verify(mock => mock.CreateOrUpdateAsync(It.IsAny<StoreRequest>()), Times.Once);
        }
    }
}