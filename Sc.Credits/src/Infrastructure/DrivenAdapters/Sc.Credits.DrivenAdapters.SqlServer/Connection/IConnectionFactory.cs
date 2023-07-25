using System.Data.Common;

namespace Sc.Credits.DrivenAdapters.SqlServer.Connection
{
    /// <summary>
    /// Connection factory
    /// </summary>
    public interface IConnectionFactory
    {
        /// <summary>
        /// Create
        /// </summary>
        /// <returns></returns>
        DbConnection Create();
    }
}