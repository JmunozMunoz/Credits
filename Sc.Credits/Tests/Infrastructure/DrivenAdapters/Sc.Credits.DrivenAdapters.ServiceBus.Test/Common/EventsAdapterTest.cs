using Moq;
using org.reactivecommons.api;
using org.reactivecommons.api.domain;
using Sc.Credits.DrivenAdapters.ServiceBus.Common;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.Commons.Logging.Gateway;
using Sc.Credits.Helpers.Commons.Messaging;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.DrivenAdapters.ServiceBus.Test.Common
{
    public class EventsAdapterTest
    {
        private readonly Mock<IDirectAsyncGateway<dynamic>> _directAsyncGatewayMock = new Mock<IDirectAsyncGateway<dynamic>>();
        private readonly Mock<ISettings<CredinetAppSettings>> _appSettingsMock = new Mock<ISettings<CredinetAppSettings>>();
        private readonly Mock<ILoggerService<EventsAdapter>> _loggerServiceMock = new Mock<ILoggerService<EventsAdapter>>();
        private readonly Mock<IMessagingLogger> _messagingLoggerMock = new Mock<IMessagingLogger>();

        private IEventsRepository EventsRepository =>
            new EventsAdapter(_directAsyncGatewayMock.Object,
                _messagingLoggerMock.Object,
                _loggerServiceMock.Object,
                _appSettingsMock.Object);

        public EventsAdapterTest()
        {
            _appSettingsMock.Setup(mock => mock.Get())
                .Returns(new CredinetAppSettings());
        }

        [Fact]
        public async Task SouldSendEventNotification()
        {
            await EventsRepository.SendAsync("Credits.GetCreditDetails", "55s445f58saa85e4",
                new
                {
                    creditValue = 200000,
                    months = 10,
                    storeId = "dfr74a58f4erer5a8",
                    typeDocument = "CE",
                    idDocument = "1037610201",
                    frequency = 30
                });

            _directAsyncGatewayMock.Verify(mock => mock.SendEvent(It.IsAny<string>(), It.IsAny<DomainEvent<dynamic>>()),
                Times.Once());
        }
    }
}