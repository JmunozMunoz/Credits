using Sc.Credits.Domain.Model.Base;
using System;
using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Common
{
    /// <summary>
    /// Implementation of <see cref="IEqualityComparer{T}"/> for domain entities where have <see
    /// cref="Guid"/> type as Id.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class EntityGuidEqualityComparer<TEntity> : IEqualityComparer<TEntity>
        where TEntity : Entity<Guid>
    {
        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(TEntity x, TEntity y)
        {
            return x.Id == y.Id;
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        public int GetHashCode(TEntity obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}