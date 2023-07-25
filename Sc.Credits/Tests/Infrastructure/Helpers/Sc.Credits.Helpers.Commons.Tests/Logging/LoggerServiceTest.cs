using Microsoft.Extensions.Logging;
using Moq;
using org.reactivecommons.api;
using org.reactivecommons.api.domain;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.Commons.Messaging;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.Helpers.Commons.Tests.Logging
{
    public class LoggerServiceTest
    {
        private readonly Mock<IDirectAsyncGateway<dynamic>> _eventsGatewayMock = new Mock<IDirectAsyncGateway<dynamic>>();
        private readonly Mock<ILogger<object>> _loggerMock = new Mock<ILogger<object>>();
        private readonly Mock<ISettings<CredinetAppSettings>> _appSettingsMock = new Mock<ISettings<CredinetAppSettings>>();
        private readonly Mock<IMessagingLogger> _messagingLoggerMock = new Mock<IMessagingLogger>();

        private ILoggerService<object> LoggerService =>
            new LoggerService<object>(_eventsGatewayMock.Object,
                _loggerMock.Object,
                _messagingLoggerMock.Object,
                _appSettingsMock.Object);

        public LoggerServiceTest()
        {
            _appSettingsMock
                .Setup(mock => mock.Get())
                .Returns(CredinetAppSettingsHelperTest.GetCredinetAppSettings());
        }

        [Fact]
        public async Task ShouldNotify()
        {
            _eventsGatewayMock.Setup(mock => mock.SendEvent(It.IsAny<string>(), It.IsAny<DomainEvent<dynamic>>()))
                .Verifiable();

            await LoggerService.NotifyAsync("TestEvent", Guid.NewGuid().ToString(), new { test = true });

            _eventsGatewayMock.VerifyAll();
        }

        [Fact]
        public async Task ShouldNotifyMethodBase()
        {
            _eventsGatewayMock.Setup(mock => mock.SendEvent(It.IsAny<string>(), It.IsAny<DomainEvent<dynamic>>()))
                .Verifiable();

            await LoggerService.NotifyAsync(Guid.NewGuid().ToString(), new { test = true }, MethodBase.GetCurrentMethod());

            _eventsGatewayMock.VerifyAll();
        }
    }
}