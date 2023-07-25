using Sc.Credits.DrivenAdapters.SqlServer.Dapper.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Sc.Credits.DrivenAdapters.SqlServer.Queries
{
    /// <summary>
    /// The sql delegated handlers.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISqlDelegatedHandlers<T>
        where T : class
    {
        /// <summary>
        /// Gets a single result of T.
        /// </summary>
        Task<T> GetSingleAsync(IDbConnection connection, SqlQuery sqlQuery,
            IDbTransaction transaction = null, int? commandTimeout = null);

        /// <summary>
        /// Gets a list results of T.
        /// </summary>
        Task<List<T>> GetListAsync(IDbConnection connection, SqlQuery sqlQuery,
            IDbTransaction transaction = null, int? commandTimeout = null);

        /// <summary>
        /// Inserts an element of T.
        /// </summary>
        Task<int> InsertAsync(IDbConnection connection, T entityToInsert, string tableName, IDbTransaction transaction = null,
            int? commandTimeout = null, ISqlAdapter sqlAdapter = null, params object[] entitiesForMerge);

        /// <summary>
        /// Verifies any occurence of specific query.
        /// </summary>
        Task<bool> AnyAsync(IDbConnection connection, SqlQuery sqlQuery,
            IDbTransaction transaction = null, int? commandTimeout = null);

        /// <summary>
        /// Executes a command.
        /// </summary>
        Task<int> ExcecuteAsync(IDbConnection connection, SqlQuery sqlQuery,
            IDbTransaction transaction = null, int? commandTimeout = null);

        /// <summary>
        /// Executes a specific query.
        /// </summary>
        Task<IEnumerable<dynamic>> QueryAsync(IDbConnection connection, SqlQuery sqlQuery,
            IDbTransaction transaction = null, int? commandTimeout = null);

        /// <summary>
        /// Delete an element of T.
        /// </summary>
        Task<bool> DeleteAsync(IDbConnection connection, string tableName, string column, string id, IDbTransaction transaction = null, ISqlAdapter sqlAdapter = null);
    }
}