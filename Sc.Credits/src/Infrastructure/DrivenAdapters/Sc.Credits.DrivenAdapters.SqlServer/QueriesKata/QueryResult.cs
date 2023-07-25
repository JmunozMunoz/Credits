using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using SqlKata;
using SqlKata.Compilers;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata
{
    /// <summary>
    /// Query result
    /// </summary>
    internal class QueryResult
    {
        private readonly Query _query;
        private readonly SqlResult _compilation;

        /// <summary>
        /// New query result
        /// </summary>
        /// <param name="compiler"></param>
        public QueryResult(Query query, Compiler compiler)
        {
            _query = query;
            _compilation = compiler.Compile(query);
        }

        /// <summary>
        /// As read query
        /// </summary>
        public ReadQuery AsReadQuery() =>
            new ReadQuery(_compilation, _query);

        /// <summary>
        /// As command query
        /// </summary>
        public CommandQuery AsCommandQuery() =>
            new CommandQuery(_compilation, _query);
    }
}