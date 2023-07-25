using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Stores.Queries;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Stores
{
    /// <summary>
    /// Store queries
    /// </summary>
    internal class StoreQueries
        : CommandQueries<StoreFields>
    {
        /// <summary>
        /// New store queries
        /// </summary>
        public StoreQueries()
            : base(Tables.Catalog.Stores)
        {
        }
    }
}