using System.Threading.Tasks;

namespace Sc.Credits.Domain.Model.Common.Gateway
{
    /// <summary>
    /// Notification Repository contract
    /// </summary>
    public interface INotificationRepository
    {
        /// <summary>
        /// Notify mail
        /// </summary>
        /// <param name="mailNotificationRequest"></param>
        /// <param name="notificationName"></param>
        /// <param name="handleError"></param>
        /// <returns></returns>
        Task NotifyMailAsync(MailNotificationRequest mailNotificationRequest, string notificationName, bool handleError);

        /// <summary>
        /// Notify Sms
        /// </summary>
        /// <param name="smsNotificationRequest"></param>
        /// <param name="notificationName"></param>
        /// <param name="handleError"></param>
        /// <returns></returns>
        Task NotifySmsAsync(SmsNotificationRequest smsNotificationRequest, string notificationName, bool handleError);
    }
}