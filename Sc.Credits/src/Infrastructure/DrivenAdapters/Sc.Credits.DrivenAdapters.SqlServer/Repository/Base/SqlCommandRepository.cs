using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using System;
using System.Data.Common;
using System.Threading.Tasks;
using System.Transactions;

namespace Sc.Credits.DrivenAdapters.SqlServer.Repository.Base
{
    /// <summary>
    /// The sql command repository base class.
    /// </summary>
    public abstract class SqlCommandRepository<TEntity, TFields>
        : ReadRepository<TEntity, TFields>
        where TEntity : class, IAggregateRoot
        where TFields : EntityFields
    {
        /// <summary>
        /// The command queries for TEntity.
        /// </summary>
        protected new readonly ICommandQueries<TFields> Queries;

        /// <summary>
        /// Creates new instance of <see cref="SqlCommandRepository{TEntity, TFields}"/>
        /// </summary>
        /// <param name="queries"></param>
        /// <param name="entitySqlDelegatedHandlers"></param>
        /// <param name="connectionFactory"></param>
        protected SqlCommandRepository(ICommandQueries<TFields> queries,
            ISqlDelegatedHandlers<TEntity> entitySqlDelegatedHandlers,
            IConnectionFactory connectionFactory)
            : base(queries, entitySqlDelegatedHandlers, connectionFactory)
        {
            Queries = queries;
        }

        /// <summary>
        /// Runs a command function using the current connection.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="commandFunc"></param>
        /// <returns></returns>
        protected async Task CommandUsingConnectionAsync(Func<DbConnection, Task> commandFunc)
        {
            using (DbConnection connection = ConnectionFactory.Create())
            {
                await connection.OpenAsync();

                await commandFunc(connection);
            }
        }

        /// <summary>
        /// Runs a command enlisting in specific transaction.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="commandFunc"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        protected async Task CommandEnlistingTransactionAsync(Func<DbConnection, Task> commandFunc, Transaction transaction)
        {
            using (DbConnection connection = ConnectionFactory.Create())
            {
                await connection.OpenAsync();

                if (transaction != null)
                {
                    connection.EnlistTransaction(transaction);
                }

                await commandFunc(connection);
            }
        }
    }
}