using Sc.Credits.Domain.Model.Queries;
using System.Collections.Generic;

namespace Sc.Credits.DrivenAdapters.SqlServer.Queries
{
    /// <summary>
    /// Command queries
    /// </summary>
    public interface ICommandQueries<TFields>
        : IReadQueries<TFields>
        where TFields : EntityFields
    {
        /// <summary>
        /// Update
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="fields"></param>
        /// <param name="entitiesForMerge"></param>
        /// <returns></returns>
        SqlQuery Update<TEntity>(TEntity entity, IEnumerable<Field> fields, params object[] entitiesForMerge);
    }
}