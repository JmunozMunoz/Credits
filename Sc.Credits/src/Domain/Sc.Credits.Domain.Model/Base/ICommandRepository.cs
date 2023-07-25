using Sc.Credits.Domain.Model.Queries;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Transactions;

namespace Sc.Credits.Domain.Model.Base
{
    /// <summary>
    /// Base contract for command repository
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface ICommandRepository<TEntity>
        : IReadRepository<TEntity>
        where TEntity : IAggregateRoot
    {
        /// <summary>
        /// Add aggregate root entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task AddAsync(TEntity entity, Transaction transaction = null);

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(TEntity entity, Transaction transaction = null);

        /// <summary>
        /// Update aggregate root entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="fields"></param>
        /// <param name="transaction"></param>
        /// <param name="entitiesForMerge"></param>
        /// <returns></returns>
        Task UpdateAsync(TEntity entity, IEnumerable<Field> fields, Transaction transaction = null, params object[] entitiesForMerge);
    }
}