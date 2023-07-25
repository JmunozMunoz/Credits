using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace Sc.Credits.DrivenAdapters.SqlServer.Repository.Base
{
    /// <summary>
    /// The default base implementation of <see cref="ICommandRepository{TEntity}"/>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TFields"></typeparam>
    public abstract class CommandRepository<TEntity, TFields>
        : SqlCommandRepository<TEntity, TFields>, ICommandRepository<TEntity>
        where TEntity : class, IAggregateRoot
        where TFields : EntityFields
    {
        /// <summary>
        /// The command queries instance of TEntity.
        /// </summary>
        protected new readonly ICommandQueries<TFields> Queries;

        /// <summary>
        /// Creates new instance of <see cref="CommandRepository{TEntity, TFields}"/>
        /// </summary>
        /// <param name="queries"></param>
        /// <param name="entitySqlDelegatedHandlers"></param>
        /// <param name="connectionFactory"></param>
        protected CommandRepository(ICommandQueries<TFields> queries,
            ISqlDelegatedHandlers<TEntity> entitySqlDelegatedHandlers,
            IConnectionFactory connectionFactory)
            : base(queries, entitySqlDelegatedHandlers, connectionFactory)
        {
            Queries = queries;
        }

        /// <summary>
        /// <see cref="ICommandRepository{TEntity}.AddAsync(TEntity, Transaction)"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public virtual async Task AddAsync(TEntity entity, Transaction transaction = null) =>
            await CommandEnlistingTransactionAsync(async (connection) =>
            {
                await EntitySqlDelegatedHandlers.InsertAsync(connection, entity, TableName);
            },
            transaction);

        /// <summary>
        /// <see cref="ICommandRepository{TEntity}.DeleteAsync(TEntity, Transaction)"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(TEntity entity, Transaction transaction = null)
        {
            bool result = false;
            await CommandEnlistingTransactionAsync(async (connection) =>
            {
                result = await EntitySqlDelegatedHandlers.DeleteAsync(connection, TableName, ColumnName, Id);
            },
            transaction);

            return result;
        }

        /// <summary>
        /// <see cref="ICommandRepository{TEntity}.UpdateAsync(TEntity, IEnumerable{Field},
        /// Transaction, object[])"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="fields"></param>
        /// <param name="transaction"></param>
        /// <param name="entitiesForMerge"></param>
        public async Task UpdateAsync(TEntity entity, IEnumerable<Field> fields, Transaction transaction = null, params object[] entitiesForMerge) =>
            await CommandEnlistingTransactionAsync(async (connection) =>
            {
                await EntitySqlDelegatedHandlers.ExcecuteAsync(connection, Queries.Update(entity, fields, entitiesForMerge));
            },
            transaction);
    }
}