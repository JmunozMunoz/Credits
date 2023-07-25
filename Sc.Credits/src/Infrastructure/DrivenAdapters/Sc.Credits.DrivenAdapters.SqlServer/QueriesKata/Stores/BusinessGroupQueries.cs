using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Stores.Queries;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Stores
{
    /// <summary>
    /// Business group queries
    /// </summary>
    internal class BusinessGroupQueries
        : CommandQueries<BusinessGroupFields>
    {
        /// <summary>
        /// New business group queries
        /// </summary>
        public BusinessGroupQueries()
            : base(Tables.Catalog.BusinessGroup)
        {
        }
    }
}