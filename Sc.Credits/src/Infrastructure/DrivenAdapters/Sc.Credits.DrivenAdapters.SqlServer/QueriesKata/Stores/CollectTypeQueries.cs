using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Stores.Queries;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Stores
{
    /// <summary>
    /// Collect type queries
    /// </summary>
    internal class CollectTypeQueries
        : ReadQueries<CollectTypeFields>
    {
        /// <summary>
        /// New store queries
        /// </summary>
        public CollectTypeQueries()
            : base(Tables.Catalog.CollectTypes)
        {
        }
    }
}