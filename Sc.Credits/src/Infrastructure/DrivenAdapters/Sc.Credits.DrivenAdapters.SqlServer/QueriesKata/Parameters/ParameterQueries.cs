using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Parameters
{
    /// <summary>
    /// Parameter queries
    /// </summary>
    internal class ParameterQueries
        : ReadQueries<EntityFields>
    {
        /// <summary>
        /// New parameter queries
        /// </summary>
        public ParameterQueries()
            : base(Tables.Catalog.Parameters)
        {
        }

        /// <summary>
        /// All
        /// </summary>
        /// <returns></returns>
        public SqlQuery All() =>
            ToReadQuery(Query);
    }
}