using credinet.exception.middleware.enums;
using credinet.exception.middleware.models;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Linq;

namespace Sc.Credits.Domain.Model.Refinancings
{
    /// <summary>
    /// The refinancing params entity
    /// </summary>
    public class RefinancingParams
    {
        /// <summary>
        /// Creates a new instance of <see cref="RefinancingParams"/>
        /// </summary>
        protected RefinancingParams()
        {
        }

        /// <summary>
        /// Gets the default frequency
        /// </summary>
        public static Frequencies DefaultFrequency => Frequencies.Monthly;

        /// <summary>
        /// Gets the calculation date
        /// </summary>
        public static DateTime CalculationDate => DateTime.Today;

        /// <summary>
        /// Gets the store id
        /// </summary>
        /// <param name="credinetAppSettings"></param>
        /// <returns></returns>
        public static string StoreId(CredinetAppSettings credinetAppSettings)
            => credinetAppSettings.RefinancingStoreId;

        /// <summary>
        /// Validates the source
        /// </summary>
        /// <param name="credinetAppSettings"></param>
        /// <param name="source"></param>
        public static void ValidateSource(CredinetAppSettings credinetAppSettings, int source)
        {
            if (!IsAllowedSource(credinetAppSettings, source))
            {
                throw new BusinessException(nameof(BusinessResponse.InvalidSource), (int)BusinessResponse.InvalidSource);
            }
        }

        /// <summary>
        /// Gets if is allowed source
        /// </summary>
        /// <param name="credinetAppSettings"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsAllowedSource(CredinetAppSettings credinetAppSettings, int source, bool setCreditLimit = true) =>
            credinetAppSettings.RefinancingSourcesAllowed.Split(',').Contains(source.ToString()) && setCreditLimit;
    }
}