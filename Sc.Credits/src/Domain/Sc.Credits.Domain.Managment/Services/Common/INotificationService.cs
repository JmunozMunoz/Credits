using Sc.Credits.Domain.Model.Common;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Common
{
    /// <summary>
    /// Notification service contract
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Send mail
        /// </summary>
        /// <param name="mailNotificationRequest"></param>
        /// <param name="name"></param>
        /// <param name="handleError"></param>
        /// <returns></returns>
        Task SendMailAsync(MailNotificationRequest mailNotificationRequest, string name, bool handleError = false);

        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="smsNotificationRequest"></param>
        /// <param name="name"></param>
        /// <param name="handleError"></param>
        /// <returns></returns>
        Task SendSmsAsync(SmsNotificationRequest smsNotificationRequest, string name, bool handleError = false);
    }
}