using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.Domain.Model.Queries.Extensions
{
    /// <summary>
    /// Entity fields extensions
    /// </summary>
    public static class FieldsExtensions
    {
        /// <summary>
        /// Gets all fields.
        /// </summary>
        /// <typeparam name="TFields"></typeparam>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static IEnumerable<Field> GetAllFields<TFields>(this TFields fields)
            where TFields : Fields =>
            fields
                .GetType()
                .GetProperties()
                .Where(p => p.PropertyType == typeof(Field))
                .Select(p => (Field)p.GetValue(fields))
                .Where(f => f != null)
                .Reverse();
    }
}