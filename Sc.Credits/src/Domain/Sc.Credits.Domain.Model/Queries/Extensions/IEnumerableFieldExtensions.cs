using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.Domain.Model.Queries.Extensions
{
    /// <summary>
    /// IEnumerable field extensions
    /// </summary>
    public static class IEnumerableFieldExtensions
    {
        /// <summary>
        /// Rename
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        public static IEnumerable<Field> Rename(this IEnumerable<Field> fields, string alias) =>
            fields
                .Select(f => new Field(f.Name, alias));
    }
}