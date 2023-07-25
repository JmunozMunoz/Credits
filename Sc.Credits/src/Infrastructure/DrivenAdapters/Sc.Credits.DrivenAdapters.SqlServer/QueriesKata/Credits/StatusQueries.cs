using Sc.Credits.Domain.Model.Credits.Queries;
using Sc.Credits.Domain.Model.Queries;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Credits
{
    /// <summary>
    /// Status queries
    /// </summary>
    internal class StatusQueries
        : ReadQueries<StatusFields>
    {
        /// <summary>
        /// New status queries
        /// </summary>
        public StatusQueries()
            : base(Tables.Catalog.Status)
        {
        }
    }
}