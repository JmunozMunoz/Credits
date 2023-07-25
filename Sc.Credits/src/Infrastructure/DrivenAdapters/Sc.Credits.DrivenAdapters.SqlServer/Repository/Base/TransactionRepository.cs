using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace Sc.Credits.DrivenAdapters.SqlServer.Repository.Base
{
    /// <summary>
    /// The base implementation of <see cref="ITransactionRepository{TEntity}"/>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TFields"></typeparam>
    public abstract class TransactionRepository<TEntity, TFields>
        : CommandRepository<TEntity, TFields>, ITransactionRepository<TEntity>
        where TEntity : class, IAggregateRoot
        where TFields : EntityFields
    {
        /// <summary>
        /// Creates new instance of <see cref="TransactionRepository{TEntity, TFields}"/>
        /// </summary>
        /// <param name="queries"></param>
        /// <param name="entitySqlDelegatedHandlers"></param>
        /// <param name="connectionFactory"></param>
        protected TransactionRepository(ICommandQueries<TFields> queries,
            ISqlDelegatedHandlers<TEntity> entitySqlDelegatedHandlers,
            IConnectionFactory connectionFactory)
            : base(queries, entitySqlDelegatedHandlers, connectionFactory)
        {
        }

        /// <summary>
        /// <see cref="ITransactionRepository{TEntity}.ExcecuteOnTransactionAsync(Func{Transaction, Task})"/>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public async Task ExcecuteOnTransactionAsync(Func<Transaction, Task> func)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.Serializable
                },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                await func(Transaction.Current);

                scope.Complete();
            }
        }

        /// <summary>
        /// <see
        /// cref="ITransactionRepository{TEntity}.ExcecuteOnTransactionAsync{TResult}(Func{Transaction, Task{TResult}})"/>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public async Task<TResult> ExcecuteOnTransactionAsync<TResult>(Func<Transaction, Task<TResult>> func)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.Serializable
                },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                TResult result = await func(Transaction.Current);

                scope.Complete();

                return result;
            }
        }
    }
}