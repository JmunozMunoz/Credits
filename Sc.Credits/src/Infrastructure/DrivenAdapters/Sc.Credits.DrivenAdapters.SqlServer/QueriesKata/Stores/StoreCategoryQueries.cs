using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Stores.Queries;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Stores
{
    /// <summary>
    /// Stores categories queries
    /// </summary>
    internal class StoreCategoryQueries
        : ReadQueries<StoreCategoryFields>
    {
        /// <summary>
        /// New stores categories queries
        /// </summary>
        public StoreCategoryQueries()
            : base(Tables.Catalog.StoresCategories)
        {
        }
    }
}