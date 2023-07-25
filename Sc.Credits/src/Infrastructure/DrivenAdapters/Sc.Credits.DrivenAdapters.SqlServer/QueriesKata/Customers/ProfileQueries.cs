using Sc.Credits.Domain.Model.Customers.Queries;
using Sc.Credits.Domain.Model.Queries;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Customers
{
    /// <summary>
    /// Profile queries
    /// </summary>
    internal class ProfileQueries
        : ReadQueries<ProfileFields>
    {
        /// <summary>
        /// New profile queries
        /// </summary>
        public ProfileQueries()
            : base(Tables.Catalog.Profiles)
        {
        }
    }
}