using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using SqlKata;
using SqlKata.Compilers;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata
{
    /// <summary>
    /// <see cref="IQueries{TFields}"/>
    /// </summary>
    internal abstract class Queries<TFields>
        : IQueries<TFields>
        where TFields : Fields
    {
        /// <summary>
        /// Query
        /// </summary>
        protected Query Query => new Query(Table.Name);

        /// <summary>
        /// Compiler
        /// </summary>
        protected virtual Compiler Compiler => Compilers.SqlServer;

        /// <summary>
        /// <see cref="IQueries{TFields}.Table"/>
        /// </summary>
        public Table<TFields> Table { get; }

        /// <summary>
        /// Fields
        /// </summary>
        public TFields Fields => Table.Fields;

        /// <summary>
        /// New queries
        /// </summary>
        /// <param name="table"></param>
        protected Queries(Table<TFields> table)
        {
            Table = table;
        }

        /// <summary>
        /// Get result
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected QueryResult GetResult(Query query) =>
            new QueryResult(query, Compiler);
    }
}