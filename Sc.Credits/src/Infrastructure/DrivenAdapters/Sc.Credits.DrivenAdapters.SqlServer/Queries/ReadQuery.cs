using SqlKata;

namespace Sc.Credits.DrivenAdapters.SqlServer.Queries
{
    /// <summary>
    /// Read query
    /// </summary>
    internal class ReadQuery
        : SqlQuery
    {
        /// <summary>
        /// New read query
        /// </summary>
        /// <param name="compilation"></param>
        /// <param name="query"></param>
        public ReadQuery(SqlResult compilation, Query query)
            : base(compilation, query)
        {
        }
    }
}