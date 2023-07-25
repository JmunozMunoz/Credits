using Sc.Credits.Domain.Model.Credits.Queries;
using Sc.Credits.Domain.Model.Queries;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Credits
{
    /// <summary>
    /// Request status queries
    /// </summary>
    internal class RequestStatusQueries
        : ReadQueries<RequestStatusFields>
    {
        /// <summary>
        /// New request status queries
        /// </summary>
        public RequestStatusQueries()
            : base(Tables.Catalog.RequestStatus)
        {
        }
    }
}