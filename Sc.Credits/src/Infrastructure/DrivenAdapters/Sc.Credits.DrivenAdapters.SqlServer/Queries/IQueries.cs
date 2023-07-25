using Sc.Credits.Domain.Model.Queries;

namespace Sc.Credits.DrivenAdapters.SqlServer.Queries
{
    /// <summary>
    /// Queries
    /// </summary>
    public interface IQueries<TFields>
        where TFields : Fields
    {
        /// <summary>
        /// Table
        /// </summary>
        Table<TFields> Table { get; }
    }
}