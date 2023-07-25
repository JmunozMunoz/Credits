using Sc.Credits.Helpers.ObjectsUtils;

namespace Sc.Credits.Domain.Managment.Services.Common
{
    /// <summary>
    /// Commons contract
    /// </summary>
    public interface ICommons
    {
        /// <summary>
        /// Parameters
        /// </summary>
        IAppParametersService AppParameters { get; }

        /// <summary>
        /// Notification service
        /// </summary>
        INotificationService Notification { get; }

        /// <summary>
        /// Templates service
        /// </summary>
        ITemplatesService Templates { get; }

        /// <summary>
        /// Storage service
        /// </summary>
        IStorageService Storage { get; }

        /// <summary>
        /// Credinet app settings
        /// </summary>
        CredinetAppSettings CredinetAppSettings { get; }
    }
}