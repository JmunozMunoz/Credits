using Moq;
using org.reactivecommons.api;
using org.reactivecommons.api.domain;
using Sc.Credits.Domain.Managment.Services.Stores;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.DrivenAdapters.ServiceBus.Credits;
using Sc.Credits.DrivenAdapters.ServiceBus.Model;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.Commons.Messaging;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.DrivenAdapters.ServiceBus.Test.Credits
{
    public class CreditNotificationAdapterTest
    {
        private readonly Mock<IDirectAsyncGateway<CreateCreditResponse>> _directAsyncGatewayCreateMock = new Mock<IDirectAsyncGateway<CreateCreditResponse>>();
        private readonly Mock<IDirectAsyncGateway<CreditEvent>> _directAsyncGatewayEventMock = new Mock<IDirectAsyncGateway<CreditEvent>>();
        private readonly Mock<IMessagingLogger> _messagingLoggerMock = new Mock<IMessagingLogger>();
        private readonly Mock<ISettings<CredinetAppSettings>> _appSettingsMock = new Mock<ISettings<CredinetAppSettings>>();
        private Mock<IStoreService> _storeServiceMock = new Mock<IStoreService>();
        private readonly Mock<ILoggerService<CreditNotificationAdapter>> _loggerServiceMock =
            new Mock<ILoggerService<CreditNotificationAdapter>>();

        private ICreditNotificationRepository CreditNotificationRepository =>
            new CreditNotificationAdapter(_directAsyncGatewayCreateMock.Object,
                _directAsyncGatewayEventMock.Object,
                _messagingLoggerMock.Object,
                _appSettingsMock.Object,
                _loggerServiceMock.Object);

        public CreditNotificationAdapterTest()
        {
            _appSettingsMock.Setup(mock => mock.Get())
                .Returns(new CredinetAppSettings());
        }

        [Fact]
        public async Task ShouldNotifyCreate()
        {
            await CreditNotificationRepository.NotifyCreationAsync(CreditHelperTest.GetCreateCreditResponse());

            _directAsyncGatewayCreateMock.Verify(mock => mock.SendCommand(It.IsAny<string>(), It.IsAny<Command<CreateCreditResponse>>()), Times.Once());
        }

        [Fact]
        public async Task SouldSendCreditEvent()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();
            Store store = StoreHelperTest.GetStore();

            await CreditNotificationRepository.SendEventAsync(creditMaster, creditMaster.Customer, store);

            _directAsyncGatewayEventMock.Verify(mock => mock.SendEvent(It.IsAny<string>(), It.IsAny<DomainEvent<CreditEvent>>()), Times.Once());
        }

        [Fact]
        public async Task SouldLogFailedSendCreditEvent()
        {
            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();
            Store store = StoreHelperTest.GetStore();

            _directAsyncGatewayEventMock.Setup(mock => mock.SendEvent(It.IsAny<string>(), It.IsAny<DomainEvent<CreditEvent>>()))
                .ThrowsAsync(new Exception());

            await CreditNotificationRepository.SendEventAsync(creditMaster, creditMaster.Customer, store);

            _loggerServiceMock.Verify(mock =>
                mock.LogError(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<MethodBase>(), It.IsAny<string>()),
                Times.Once);

            _messagingLoggerMock.Verify(mock => mock.LogTopicErrorAsync(It.IsAny<DomainEvent<CreditEvent>>(), It.IsAny<string>()), Times.Once);
        }
    }
}