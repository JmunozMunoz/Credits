using Sc.Credits.Helpers.ObjectsUtils;

namespace Sc.Credits.Domain.Managment.Services.Common
{
    /// <summary>
    /// Commons is an implementation of <see cref="ICommons"/>
    /// </summary>
    public class Commons : ICommons
    {
        /// <summary>
        /// <see cref="ICommons.AppParameters"/>
        /// </summary>
        public IAppParametersService AppParameters { get; }

        /// <summary>
        /// <see cref="ICommons.Notification"/>
        /// </summary>
        public INotificationService Notification { get; }

        /// <summary>
        /// <see cref="ICommons.Templates"/>
        /// </summary>
        public ITemplatesService Templates { get; }

        /// <summary>
        /// <see cref="ICommons.Storage"/>
        /// </summary>
        public IStorageService Storage { get; }

        /// <summary>
        /// <see cref="ICommons.CredinetAppSettings"/>
        /// </summary>
        public CredinetAppSettings CredinetAppSettings =>
            AppParameters.GetSettings();

        /// <summary>
        /// Creates a new instance of <see cref="Commons"/>
        /// </summary>
        /// <param name="appParametersService"></param>
        /// <param name="notificationService"></param>
        /// <param name="templatesService"></param>
        /// <param name="storageService"></param>
        public Commons(IAppParametersService appParametersService,
            INotificationService notificationService,
            ITemplatesService templatesService,
            IStorageService storageService)
        {
            AppParameters = appParametersService;
            Notification = notificationService;
            Templates = templatesService;
            Storage = storageService;
        }
    }
}