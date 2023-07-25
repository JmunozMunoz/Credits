using Dapper;
using Sc.Credits.Helpers.Commons.Extensions;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using static Sc.Credits.DrivenAdapters.SqlServer.Dapper.Extensions.SqlMapperExtensions;

namespace Sc.Credits.DrivenAdapters.SqlServer.Dapper.Extensions
{
    /// <summary>
    /// SQL mapper extensions
    /// </summary>
    public static partial class SqlMapperExtensions
    {
        /// <summary>
        /// Get single
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static async Task<T> GetSingleAsync<T>(this IDbConnection connection, string query, object parameters,
            IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var dynamicQuery = await connection.QuerySingleOrDefaultAsync(query, parameters,
                transaction, commandTimeout);

            if (dynamicQuery is IDictionary<string, object>)
            {
                T entity = TypeInstance.New<T>();

                IDictionary<string, object> dictionary = dynamicQuery as IDictionary<string, object>;

                ReflectionUtils.MapDictionaryToEntity(dictionary, entity);

                return entity;
            }

            return null;
        }

        /// <summary>
        /// Get list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="sqlAdapter"></param>
        /// <returns></returns>
        public static async Task<List<T>> GetListAsync<T>(this IDbConnection connection, string query, object parameters,
            IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var dynamicQuery = await connection.QueryAsync(query, parameters,
                transaction, commandTimeout);

            List<T> list = new List<T>();

            if (dynamicQuery is ICollection<object>)
            {
                ICollection<object> responses = dynamicQuery as ICollection<object>;

                foreach (var response in responses)
                {
                    T entity = TypeInstance.New<T>();

                    IDictionary<string, object> dictionary = response as IDictionary<string, object>;

                    ReflectionUtils.MapDictionaryToEntity(dictionary, entity);

                    list.Add(entity);
                }
            }

            return list;
        }

        /// <summary>
        /// Any
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static async Task<bool> AnyAsync(this IDbConnection connection, string query, object parameters,
            IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var dynamicQuery = await connection.QueryFirstOrDefaultAsync(query, parameters,
                transaction, commandTimeout);

            if (dynamicQuery is IDictionary<string, object>)
            {
                IDictionary<string, object> dictionary = dynamicQuery as IDictionary<string, object>;

                return dictionary.Count > 0;
            }

            return false;
        }

        /// <summary>
        /// Insert
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="entityToInsert"></param>
        /// <param name="tableName"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="sqlAdapter"></param>
        /// <param name="entitiesForMerge"></param>
        /// <returns></returns>
        public static async Task<int> InsertAsync<T>(this IDbConnection connection, T entityToInsert, string tableName, IDbTransaction transaction = null,
            int? commandTimeout = null, ISqlAdapter sqlAdapter = null, params object[] entitiesForMerge) where T : class
        {
            sqlAdapter = sqlAdapter ?? GetFormatter(connection);

            DynamicParameters parameters = new DynamicParameters();

            BuildInsertComposition(entityToInsert, sqlAdapter, parameters, out string columnList, out string parameterList, entitiesForMerge);

            Type type = typeof(T);

            tableName = string.IsNullOrEmpty(tableName) ? GetTableName(type) : tableName;

            return await sqlAdapter.InsertAsync(connection, transaction, commandTimeout, tableName, columnList,
                            parameterList, parameters);
        }

        /// <summary>
        /// <see cref="ISqlAdapter.DeleteAsync(IDbConnection connection, IDbTransaction transaction, string tableName, string column, string id, ISqlAdapter sqlAdapter = null,)"/>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="tableName"></param>
        /// <param name="column"></param>
        /// <param name="id"></param>
        /// <returns>is Delete</returns>
        public static async Task<bool> DeleteAsync(this IDbConnection connection, string tableName, string column, string id, IDbTransaction transaction = null, ISqlAdapter sqlAdapter = null)
        {
            sqlAdapter = sqlAdapter ?? GetFormatter(connection);

            return await sqlAdapter.DeleteAsync(connection, transaction, tableName, column, id);
        }
    }

    /// <summary>
    /// Sql adapter
    /// </summary>
    public partial interface ISqlAdapter
    {
        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="tableName"></param>
        /// <param name="columnList"></param>
        /// <param name="parameterList"></param>
        /// <param name="entityToInsert"></param>
        /// <returns></returns>
        Task<int> InsertAsync(IDbConnection connection, IDbTransaction transaction,
            int? commandTimeout, string tableName, string columnList, string parameterList,
            object entityToInsert);

        /// <summary>
        /// DeleteAsync
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="tableName"></param>
        /// <param name="column"></param>
        /// <param name="id"></param>
        /// <returns>is Delete</returns>
        Task<bool> DeleteAsync(IDbConnection connection, IDbTransaction transaction, string tableName, string column, string id);
    }

    /// <summary>
    /// <see cref="ISqlAdapter"/>
    /// </summary>
    public partial class SqlServerAdapter
    {
        /// <summary>
        /// <see cref="ISqlAdapter.InsertAsync(IDbConnection, IDbTransaction, int?, string, string,
        /// string, object)"/>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="tableName"></param>
        /// <param name="columnList"></param>
        /// <param name="parameterList"></param>
        /// <param name="entityToInsert"></param>
        /// <returns></returns>
        public async Task<int> InsertAsync(IDbConnection connection, IDbTransaction transaction,
            int? commandTimeout, string tableName, string columnList, string parameterList,
            object entityToInsert)
        {
            var cmd = $"INSERT INTO {tableName} ({columnList}) VALUES ({parameterList});";
            var result = await connection.ExecuteAsync(cmd, entityToInsert, transaction, commandTimeout)
                .ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// <see cref="ISqlAdapter.DeleteAsync(IDbConnection connection, IDbTransaction transaction, string tableName, string column, string id)"/>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="tableName"></param>
        /// <param name="column"></param>
        /// <param name="id"></param>
        /// <returns>is Delete</returns>
        public async Task<bool> DeleteAsync(IDbConnection connection, IDbTransaction transaction,string tableName, string column,  string id)
        {
            var cmd = $"DELETE {tableName} WHERE {column} = '{id}';";
            var result = await connection.ExecuteAsync(cmd, transaction)
                .ConfigureAwait(false);

            return (result > 0);
        }
    }
}