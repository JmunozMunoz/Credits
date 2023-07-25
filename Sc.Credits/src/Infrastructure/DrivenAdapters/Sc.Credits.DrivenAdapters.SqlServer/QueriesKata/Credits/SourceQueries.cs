using Sc.Credits.Domain.Model.Credits.Queries;
using Sc.Credits.Domain.Model.Queries;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Credits
{
    /// <summary>
    /// Source queries
    /// </summary>
    internal class SourceQueries
        : ReadQueries<SourceFields>
    {
        /// <summary>
        /// New source queries
        /// </summary>
        public SourceQueries()
            : base(Tables.Catalog.Sources)
        {
        }
    }
}