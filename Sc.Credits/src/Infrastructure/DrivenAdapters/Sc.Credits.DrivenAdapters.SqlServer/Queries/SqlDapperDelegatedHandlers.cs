using Dapper;
using Sc.Credits.DrivenAdapters.SqlServer.Dapper.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Sc.Credits.DrivenAdapters.SqlServer.Queries
{
    /// <summary>
    /// The Dapper implementation of <see cref="ISqlDelegatedHandlers{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SqlDapperDelegatedHandlers<T>
        : ISqlDelegatedHandlers<T>
        where T : class
    {
        /// <summary>
        /// <see cref="ISqlDelegatedHandlers{T}.GetSingleAsync(IDbConnection, SqlQuery,
        /// IDbTransaction, int?)"/>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sqlQuery"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="sqlAdapter"></param>
        /// <returns></returns>
        public async Task<T> GetSingleAsync(IDbConnection connection, SqlQuery sqlQuery,
            IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return await connection.GetSingleAsync<T>(sqlQuery.Sql, sqlQuery.Params, transaction, commandTimeout);
        }

        /// <summary>
        /// <see cref="ISqlDelegatedHandlers{T}.GetListAsync(IDbConnection, SqlQuery,
        /// IDbTransaction, int?)"/>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sqlQuery"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="sqlAdapter"></param>
        /// <returns></returns>
        public async Task<List<T>> GetListAsync(IDbConnection connection, SqlQuery sqlQuery,
            IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return await connection.GetListAsync<T>(sqlQuery.Sql, sqlQuery.Params, transaction, commandTimeout);
        }

        /// <summary>
        /// <see cref="ISqlDelegatedHandlers{T}.InsertAsync(IDbConnection, T, string,
        /// IDbTransaction, int?, ISqlAdapter, object[])"/>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="entityToInsert"></param>
        /// <param name="tableName"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="sqlAdapter"></param>
        /// <param name="entitiesForMerge"></param>
        /// <returns></returns>
        public async Task<int> InsertAsync(IDbConnection connection, T entityToInsert, string tableName = null, IDbTransaction transaction = null,
            int? commandTimeout = null, ISqlAdapter sqlAdapter = null, params object[] entitiesForMerge)
        {
            return await connection.InsertAsync(entityToInsert, tableName, transaction, commandTimeout, sqlAdapter, entitiesForMerge);
        }

        /// <summary>
        /// <see cref="ISqlDelegatedHandlers{T}.AnyAsync(IDbConnection, SqlQuery, IDbTransaction, int?)"/>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sqlQuery"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public async Task<bool> AnyAsync(IDbConnection connection, SqlQuery sqlQuery,
            IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return await connection.AnyAsync(sqlQuery.Sql, sqlQuery.Params, transaction, commandTimeout);
        }

        /// <summary>
        /// <see cref="ISqlDelegatedHandlers{T}.DeleteAsync(IDbConnection connection, IDbTransaction transaction, string tableName, string column, string id)"/>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="tableName"></param>
        /// <param name="column"></param>
        /// <param name="id"></param>
        /// <returns>is Delete</returns>
        public async Task<bool> DeleteAsync(IDbConnection connection, string tableName, string column, string id, IDbTransaction transaction = null, ISqlAdapter sqlAdapter = null)
        {
            return await connection.DeleteAsync(tableName, column, id, transaction, sqlAdapter);
        }

        /// <summary>
        /// <see cref="ISqlDelegatedHandlers{T}.ExcecuteAsync(IDbConnection, SqlQuery,
        /// IDbTransaction, int?)"/>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sqlQuery"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="sqlAdapter"></param>
        /// <returns></returns>
        public async Task<int> ExcecuteAsync(IDbConnection connection, SqlQuery sqlQuery,
            IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return await connection.ExecuteAsync(sqlQuery.Sql, sqlQuery.Params, transaction, commandTimeout);
        }

        /// <summary>
        /// <see cref="ISqlDelegatedHandlers{T}.QueryAsync(IDbConnection, SqlQuery, IDbTransaction, int?)"/>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sqlQuery"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public async Task<IEnumerable<dynamic>> QueryAsync(IDbConnection connection, SqlQuery sqlQuery,
            IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return await connection.QueryAsync(sqlQuery.Sql, sqlQuery.Params, transaction, commandTimeout);
        }

    }
}