using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Extensions;
using SqlKata;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata
{
    /// <summary>
    /// Command queries
    /// </summary>
    internal abstract class CommandQueries<TFields>
        : ReadQueries<TFields>, ICommandQueries<TFields>
        where TFields : EntityFields
    {
        /// <summary>
        /// New command queries
        /// </summary>
        /// <param name="table"></param>
        protected CommandQueries(Table<TFields> table)
            : base(table)
        {
        }

        /// <summary>
        /// <see cref="ICommandQueries{TFields}.Update{TEntity}(TEntity, IEnumerable{Field}, object[])"/>
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="fields"></param>
        /// <param name="entitiesForMerge"></param>
        /// <returns></returns>
        public SqlQuery Update<TEntity>(TEntity entity, IEnumerable<Field> fields, params object[] entitiesForMerge)
        {
            string[] updateFieldNames = fields.Select(f => f.Name).ToArray();

            Query query =
                Query
                    .Where(IdName, entity.GetValue(IdName))
                    .AsUpdate(entity.GetValues(updateFieldNames)
                        .Union(entitiesForMerge.SelectMany(entitieForMerge => entitieForMerge.GetValues(updateFieldNames)))
                        .ToDictionary(updatePairKey => updatePairKey.Key, updatePairValue => updatePairValue.Value));

            return ToCommandQuery(query);
        }

        /// <summary>
        /// To command query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected CommandQuery ToCommandQuery(Query query) =>
            query.AsQueryResult(Compiler).AsCommandQuery();
    }
}