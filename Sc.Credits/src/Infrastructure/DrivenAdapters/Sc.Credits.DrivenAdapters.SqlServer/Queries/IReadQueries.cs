using Sc.Credits.Domain.Model.Queries;
using System.Collections.Generic;

namespace Sc.Credits.DrivenAdapters.SqlServer.Queries
{
    /// <summary>
    /// Read queries
    /// </summary>
    public interface IReadQueries<TFields>
        : IQueries<TFields>
        where TFields : EntityFields
    {
        /// <summary>
        /// By id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        SqlQuery ById<T>(T id, IEnumerable<Field> fields = null);

    }
}