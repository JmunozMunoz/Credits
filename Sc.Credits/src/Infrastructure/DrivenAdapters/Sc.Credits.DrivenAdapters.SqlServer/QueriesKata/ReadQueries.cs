using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Queries.Extensions;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Extensions;
using SqlKata;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata
{
    /// <summary>
    /// <see cref="IReadQueries{TFields}"/>
    /// </summary>
    internal abstract class ReadQueries<TFields>
        : Queries<TFields>, IReadQueries<TFields>
        where TFields : EntityFields
    {
        /// <summary>
        /// <see cref="IQueries{TFields}.IdName"/>
        /// </summary>
        public string IdName => Table.Fields.Id.Name;

        /// <summary>
        /// New read queries
        /// </summary>
        /// <param name="table"></param>
        protected ReadQueries(Table<TFields> table)
            : base(table)
        {
        }

        /// <summary>
        /// <see cref="IReadQueries{TFields}.ById{T}(T, IEnumerable{Field})"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public SqlQuery ById<T>(T id, IEnumerable<Field> fields = null)
        {
            Query query =
                Query
                    .Where(IdName, id)
                    .Select((fields ?? Fields.GetAllFields()).Select(f => f.Name).ToArray());

            return ToReadQuery(query);
        }

        /// <summary>
        /// To read query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected SqlQuery ToReadQuery(Query query) =>
            query.AsQueryResult(Compiler).AsReadQuery();
    }
}