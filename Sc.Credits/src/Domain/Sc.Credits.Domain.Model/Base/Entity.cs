using Sc.Credits.Helpers.Commons.Dapper.Annotations;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Base
{
    /// <summary>
    /// Base class for domain entities.
    /// </summary>
    /// <typeparam name="T">Repository's Entity Id type</typeparam>
    public class Entity<T> : ICloneable
    {
        [Key]
        public T Id { get; protected set; }

        /// <summary>
        /// Set the id to entity
        /// </summary>
        /// <param name="id"></param>
        internal void SetId(T id)
        {
            Id = id;
        }

        /// <summary>
        /// Clone the current entity
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    /// <summary>
    /// Entity extensions
    /// </summary>
    public static class EntityExtensions
    {
        /// <summary>
        /// Get values
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="fields"></param>
        public static IReadOnlyDictionary<string, object> GetValues(this object entity, params string[] fields)
            => ReflectionUtils.GetValuesAsDictionary(entity, fields);

        /// <summary>
        /// Get value
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="fieldName"></param>
        public static object GetValue(this object entity, string fieldName)
            => ReflectionUtils.GetValue(entity, fieldName);
    }
}