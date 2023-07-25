using SqlKata;
using SqlKata.Compilers;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Extensions
{
    /// <summary>
    /// Query extensions
    /// </summary>
    internal static class QueryExtensions
    {
        /// <summary>
        /// As query result
        /// </summary>
        /// <param name="query"></param>
        /// <param name="compiler"></param>
        /// <returns></returns>
        public static QueryResult AsQueryResult(this Query query, Compiler compiler) =>
            new QueryResult(query, compiler);
    }
}