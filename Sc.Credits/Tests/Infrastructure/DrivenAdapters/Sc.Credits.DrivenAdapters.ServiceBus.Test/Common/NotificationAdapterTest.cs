using Moq;
using org.reactivecommons.api;
using org.reactivecommons.api.domain;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Common.Gateway;
using Sc.Credits.DrivenAdapters.ServiceBus.Common;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.Commons.Messaging;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.DrivenAdapters.ServiceBus.Test.Common
{
    public class NotificationAdapterTest
    {
        private readonly Mock<IDirectAsyncGateway<MailNotificationRequest>> _directAsyncGatewayMailMock = new Mock<IDirectAsyncGateway<MailNotificationRequest>>();
        private readonly Mock<IDirectAsyncGateway<SmsNotificationRequest>> _directAsyncGatewaySmsMock = new Mock<IDirectAsyncGateway<SmsNotificationRequest>>();
        private readonly Mock<ISettings<CredinetAppSettings>> _appSettingsMock = new Mock<ISettings<CredinetAppSettings>>();
        private readonly Mock<IMessagingLogger> _messagingLoggerMock = new Mock<IMessagingLogger>();
        private readonly Mock<ILoggerService<NotificationAdapter>> _loggerServiceMock = new Mock<ILoggerService<NotificationAdapter>>();

        private INotificationRepository NotificationRepositoryRepository =>
            new NotificationAdapter(_directAsyncGatewayMailMock.Object,
                _directAsyncGatewaySmsMock.Object,
                _messagingLoggerMock.Object,
                _loggerServiceMock.Object,
                _appSettingsMock.Object);

        public NotificationAdapterTest()
        {
            _appSettingsMock.Setup(mock => mock.Get())
                .Returns(new CredinetAppSettings());
        }

        [Fact]
        public async Task ShouldNotifyMail()
        {
            await NotificationRepositoryRepository.NotifyMailAsync(new MailNotificationRequest(), It.IsAny<string>(),
                It.IsAny<bool>());

            _directAsyncGatewayMailMock.Verify(mock => mock.SendCommand(It.IsAny<string>(), It.IsAny<Command<MailNotificationRequest>>()), Times.Once());
        }

        [Fact]
        public async Task ShouldNotifySms()
        {
            await NotificationRepositoryRepository.NotifySmsAsync(new SmsNotificationRequest(), It.IsAny<string>(),
                It.IsAny<bool>());

            _directAsyncGatewaySmsMock.Verify(mock => mock.SendCommand(It.IsAny<string>(), It.IsAny<Command<SmsNotificationRequest>>()), Times.Once());
        }
    }
}