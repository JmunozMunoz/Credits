using Sc.Credits.Domain.Model.Credits.Queries;
using Sc.Credits.Domain.Model.Queries;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Credits
{
    /// <summary>
    /// Auth method queries
    /// </summary>
    internal class AuthMethodQueries
        : ReadQueries<AuthMethodFields>
    {
        /// <summary>
        /// New auth methods queries
        /// </summary>
        public AuthMethodQueries()
            : base(Tables.Catalog.AuthMethods)
        {
        }
    }
}