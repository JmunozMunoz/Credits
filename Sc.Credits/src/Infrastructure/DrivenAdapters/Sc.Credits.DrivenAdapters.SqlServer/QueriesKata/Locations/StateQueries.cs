using Sc.Credits.Domain.Model.Locations.Queries;
using Sc.Credits.Domain.Model.Queries;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Locations
{
    /// <summary>
    /// State queries
    /// </summary>
    internal class StateQueries
        : CommandQueries<StateFields>
    {
        /// <summary>
        /// New state queries
        /// </summary>
        public StateQueries()
            : base(Tables.Catalog.States)
        {
        }
    }
}