using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Helpers.ObjectsUtils;

namespace Sc.Credits.Domain.Model.Credits.Builders
{
    /// <summary>
    /// Credit init builder contract
    /// </summary>
    public interface ICreditInitBuilder
    {
        /// <summary>
        /// Init
        /// </summary>
        /// <param name="source"></param>
        /// <param name="customer"></param>
        /// <param name="creditValue"></param>
        /// <param name="creditNumber"></param>
        /// <param name="userInfo"></param>
        /// <param name="credinetAppSettings"></param>
        /// <returns></returns>
        ICreditPostInitBuilder Init(Source source, Customer customer, decimal creditValue, long creditNumber,
            UserInfo userInfo, CredinetAppSettings credinetAppSettings, bool setCreditLimit = true);
    }
}