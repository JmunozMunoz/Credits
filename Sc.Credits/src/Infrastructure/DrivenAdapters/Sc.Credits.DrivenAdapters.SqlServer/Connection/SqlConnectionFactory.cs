using System.Data.Common;
using System.Data.SqlClient;

namespace Sc.Credits.DrivenAdapters.SqlServer.Connection
{
    /// <summary>
    /// <see cref="IConnectionFactory"/>
    /// </summary>
    public class SqlConnectionFactory : IConnectionFactory
    {
        private readonly string _connectionString;

        /// <summary>
        /// New MsSql connection factory
        /// </summary>
        /// <param name="connectionString"></param>
        public SqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// <see cref="IConnectionFactory.Create"/>
        /// </summary>
        /// <returns></returns>
        public DbConnection Create()
        {
            return new SqlConnection(_connectionString);
        }
    }
}