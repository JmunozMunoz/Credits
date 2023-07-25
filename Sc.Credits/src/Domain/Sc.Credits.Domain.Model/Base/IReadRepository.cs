using Sc.Credits.Domain.Model.Queries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Model.Base
{
    /// <summary>
    /// Base contract for read repository
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IReadRepository<TEntity>
        where TEntity : IAggregateRoot
    {
        /// <summary>
        /// Get entity by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        Task<TEntity> GetByIdAsync(object id, IEnumerable<Field> fields = null);

    }
}