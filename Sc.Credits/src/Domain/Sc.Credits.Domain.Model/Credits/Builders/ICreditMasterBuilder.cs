using Sc.Credits.Domain.Model.Stores;
using System;

namespace Sc.Credits.Domain.Model.Credits.Builders
{
    /// <summary>
    /// Credit master builder contract, extends <see cref="IBuilder{CreditMaster}"/>
    /// </summary>
    public interface ICreditMasterBuilder : IBuilder<CreditMaster>
    {
        /// <summary>
        /// Seller info
        /// </summary>
        /// <param name="seller"></param>
        /// <param name="products"></param>
        /// <param name="invoice"></param>
        /// <returns></returns>
        ICreditMasterBuilder SellerInfo(string seller, string products, string invoice);

        /// <summary>
        /// Credit date
        /// </summary>
        /// <param name="creditDate"></param>
        /// <returns></returns>
        ICreditMasterBuilder CreditDate(DateTime creditDate);

        /// <summary>
        /// Additional info
        /// </summary>
        /// <param name="store"></param>
        /// <param name="token"></param>
        /// <param name="location"></param>
        /// <param name="riskLevel"></param
        /// <returns></returns>
        ICreditDetailBuilder AdditionalInfo(Store store, string token, string location, string riskLevel);
    }
}