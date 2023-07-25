using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Helpers.ObjectsUtils;

namespace Sc.Credits.Domain.Model.Credits.Builders
{
    /// <summary>
    /// Credit complete builder contract
    /// </summary>
    public interface ICreditCompleteBuilder : ICreditMasterBuilder, ICreditDetailBuilder, ICreditPaymentBuilder
    {
        /// <summary>
        /// Restart
        /// </summary>
        /// <param name="source"></param>
        /// <param name="customer"></param>
        /// <param name="creditValue"></param>
        /// <param name="creditNumber"></param>
        /// <param name="userInfo"></param>
        /// <param name="credinetAppSettings"></param>
        /// <returns></returns>
        ICreditPostInitBuilder Restart(Source source, Customer customer, decimal creditValue, long creditNumber, UserInfo userInfo,
            CredinetAppSettings credinetAppSettings);
    }
}