using Moq;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Common.Gateway;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.Domain.Managment.Tests.Common
{
    public class NotificationServiceTest
    {
        private readonly Mock<INotificationRepository> _notificationRepositoryMock = new Mock<INotificationRepository>();

        private INotificationService NotificationService => new NotificationService(_notificationRepositoryMock.Object);

        [Fact]
        public async Task ShouldSendMail()
        {
            _notificationRepositoryMock.Setup(mock => mock.NotifyMailAsync(It.IsAny<MailNotificationRequest>(), It.IsAny<string>(),
                    It.IsAny<bool>()))
                .Verifiable();

            await NotificationService.SendMailAsync(new MailNotificationRequest(), "TestNotificationMail");

            _notificationRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task ShouldSendSms()
        {
            _notificationRepositoryMock.Setup(mock => mock.NotifySmsAsync(It.IsAny<SmsNotificationRequest>(), It.IsAny<string>(),
                    It.IsAny<bool>()))
                .Verifiable();

            await NotificationService.SendSmsAsync(new SmsNotificationRequest(), "TestNotificationSms");

            _notificationRepositoryMock.VerifyAll();
        }
    }
}