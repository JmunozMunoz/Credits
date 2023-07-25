using SqlKata;
using System.Collections.Generic;

namespace Sc.Credits.DrivenAdapters.SqlServer.Queries
{
    /// <summary>
    /// Sql query
    /// </summary>
    public abstract class SqlQuery
    {
        /// <summary>
        /// Params
        /// </summary>
        public readonly IReadOnlyDictionary<string, object> Params;

        /// <summary>
        /// Sql
        /// </summary>
        public readonly string Sql;

        /// <summary>
        /// Sql
        /// </summary>
        public readonly Query Query;

        /// <summary>
        /// New sql query
        /// </summary>
        /// <param name="compilation"></param>
        /// <param name="query"></param>
        protected SqlQuery(SqlResult compilation, Query query)
        {
            Sql = compilation.Sql;
            Params = compilation.NamedBindings;
            Query = query;
        }
    }
}