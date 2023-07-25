using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Stores.Queries;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Stores
{
    /// <summary>
    /// Assurance company queries
    /// </summary>
    internal class AssuranceCompanyQueries
        : ReadQueries<AssuranceCompanyFields>
    {
        /// <summary>
        /// New assurance company queries
        /// </summary>
        public AssuranceCompanyQueries()
            : base(Tables.Catalog.AssuranceCompanies)
        {
        }
    }
}