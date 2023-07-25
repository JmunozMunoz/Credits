using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Common.Gateway;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Common
{
    /// <summary>
    /// Notification service is an implementation of <see cref="INotificationService"/>
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        /// <summary>
        /// Creates a new instance of <see cref="NotificationService"/>
        /// </summary>
        /// <param name="notificationRepository"></param>
        /// <param name="blobStoreRepository"></param>
        /// <param name="templatesService"></param>
        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        /// <summary>
        /// <see cref="INotificationService.SendMailAsync(MailNotificationRequest, string, bool)"/>
        /// </summary>
        /// <param name="mailNotificationRequest"></param>
        /// <param name="name"></param>
        /// <param name="handleError"></param>
        /// <returns></returns>
        public async Task SendMailAsync(MailNotificationRequest mailNotificationRequest, string name, bool handleError = false) =>
            await _notificationRepository.NotifyMailAsync(mailNotificationRequest, name, handleError);

        /// <summary>
        /// <see cref="INotificationService.SendSmsAsync(SmsNotificationRequest, string, bool)"/>
        /// </summary>
        /// <param name="smsNotificationRequest"></param>
        /// <param name="name"></param>
        /// <param name="handleError"></param>
        /// <returns></returns>
        public async Task SendSmsAsync(SmsNotificationRequest smsNotificationRequest, string name, bool handleError = false) =>
            await _notificationRepository.NotifySmsAsync(smsNotificationRequest, name, handleError);
    }
}