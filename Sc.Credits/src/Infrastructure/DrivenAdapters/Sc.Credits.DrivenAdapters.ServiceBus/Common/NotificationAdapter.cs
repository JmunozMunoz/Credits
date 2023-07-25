using org.reactivecommons.api;
using org.reactivecommons.api.domain;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Common.Gateway;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.Commons.Messaging;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System.Reflection;
using System.Threading.Tasks;

namespace Sc.Credits.DrivenAdapters.ServiceBus.Common
{
    /// <summary>
    /// The notification adapter is an implementation of <see cref="INotificationRepository"/>
    /// </summary>
    public class NotificationAdapter
        : AsyncGatewayAdapterBase<NotificationAdapter>, INotificationRepository
    {
        private readonly IDirectAsyncGateway<MailNotificationRequest> _directAsyncGatewayMail;
        private readonly IDirectAsyncGateway<SmsNotificationRequest> _directAsyncGatewaySms;
        private readonly CredinetAppSettings _credinetAppSettings;
        private readonly string _mailMessagesQueue;
        private readonly string _smsQueue;

        /// <summary>
        /// Creates new instance of <see cref="NotificationAdapter"/>
        /// </summary>
        /// <param name="directAsyncGatewayMail"></param>
        /// <param name="messagingLogger"></param>
        /// <param name="loggerService"></param>
        /// <param name="appSettings"></param>
        public NotificationAdapter(IDirectAsyncGateway<MailNotificationRequest> directAsyncGatewayMail,
            IDirectAsyncGateway<SmsNotificationRequest> directAsyncGatewaySms,
            IMessagingLogger messagingLogger,
            ILoggerService<NotificationAdapter> loggerService,
            ISettings<CredinetAppSettings> appSettings)
            : base(loggerService, messagingLogger)
        {
            _directAsyncGatewayMail = directAsyncGatewayMail;
            _directAsyncGatewaySms = directAsyncGatewaySms;
            _credinetAppSettings = appSettings.Get();
            _mailMessagesQueue = _credinetAppSettings.MailMessagesQueue;
            _smsQueue = _credinetAppSettings.SmsQueue;
        }

        /// <summary>
        /// <see cref="INotificationRepository.NotifyMailAsync(MailNotificationRequest, string, bool)"/>
        /// </summary>
        /// <param name="mailNotificationRequest"></param>
        /// <param name="notificationName"></param>
        /// <param name="handleError"></param>
        /// <returns></returns>
        public async Task NotifyMailAsync(MailNotificationRequest mailNotificationRequest, string notificationName, bool handleError)
        {
            mailNotificationRequest.From = string.IsNullOrEmpty(mailNotificationRequest.From) ?
                _credinetAppSettings.MailFrom : mailNotificationRequest.From;

            mailNotificationRequest.FromName = string.IsNullOrEmpty(mailNotificationRequest.FromName) ?
                _credinetAppSettings.MailFromName : mailNotificationRequest.FromName;

            if (handleError)
            {
                await HandleSendCommandAsync(_directAsyncGatewayMail, id: notificationName, mailNotificationRequest, _mailMessagesQueue,
                    commandName: notificationName,
                    MethodBase.GetCurrentMethod(),
                    _credinetAppSettings.MaxRetryAttemptsMailNotificationRequest);
            }
            else
            {
                Command<MailNotificationRequest> command = new Command<MailNotificationRequest>(notificationName, notificationName, mailNotificationRequest);

                await _directAsyncGatewayMail.SendCommand(_mailMessagesQueue, command);
            }
        }

        /// <summary>
        /// <see cref="INotificationRepository.NotifySmsAsync(SmsNotificationRequest, string, bool)"/>
        /// </summary>
        /// <param name="smsNotificationRequest"></param>
        /// <param name="notificationName"></param>
        /// <param name="handleError"></param>
        /// <returns></returns>
        public async Task NotifySmsAsync(SmsNotificationRequest smsNotificationRequest, string notificationName, bool handleError)
        {
            if (handleError)
            {
                await HandleSendCommandAsync(_directAsyncGatewaySms, id: notificationName, smsNotificationRequest, _smsQueue,
                    commandName: notificationName,
                    MethodBase.GetCurrentMethod(),
                    _credinetAppSettings.MaxRetryAttemptsSmsNotificationRequest);
            }
            else
            {
                Command<SmsNotificationRequest> command = new Command<SmsNotificationRequest>(notificationName, notificationName, smsNotificationRequest);

                await _directAsyncGatewaySms.SendCommand(_smsQueue, command);
            }
        }
    }
}