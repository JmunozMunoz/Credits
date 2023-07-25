using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Sc.Credits.DrivenAdapters.SqlServer.Repository.Base
{
    /// <summary>
    /// The sql read repository base class.
    /// </summary>
    public abstract class SqlReadRepository
        : SqlRepository
    {
        /// <summary>
        /// Creates new instance of <see cref="SqlReadRepository"/>
        /// </summary>
        /// <param name="connectionFactory"></param>
        protected SqlReadRepository(IConnectionFactory connectionFactory)
            : base(connectionFactory)
        {
        }

        /// <summary>
        /// Reads a function using the current connection.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="readFunc"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        protected async Task<TResult> ReadUsingConnectionAsync<TResult>(Func<DbConnection, Task<TResult>> readFunc)
        {
            using (DbConnection connection = ConnectionFactory.Create())
            {
                await connection.OpenAsync();

                return await readFunc(connection);
            }
        }

        /// <summary>
        /// Reads a function using a specific connection.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="readFunc"></param>
        /// <returns></returns>
        protected async Task<TResult> ReadAsync<TResult>(Func<DbConnection, Task<TResult>> readFunc, DbConnection connection)
            where TResult : class => await readFunc(connection);
    }
}