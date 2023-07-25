using SqlKata;

namespace Sc.Credits.DrivenAdapters.SqlServer.Queries
{
    /// <summary>
    /// Command query
    /// </summary>
    internal class CommandQuery
        : SqlQuery
    {
        /// <summary>
        /// New command query
        /// </summary>
        /// <param name="compilation"></param>
        /// <param name="query"></param>
        public CommandQuery(SqlResult compilation, Query query)
            : base(compilation, query)
        {
        }
    }
}