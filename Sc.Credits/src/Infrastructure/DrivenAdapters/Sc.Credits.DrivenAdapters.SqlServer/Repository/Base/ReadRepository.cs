using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sc.Credits.DrivenAdapters.SqlServer.Repository.Base
{
    /// <summary>
    /// The default base implementation of <see cref="IReadRepository{TEntity}"/>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TFields"></typeparam>
    public abstract class ReadRepository<TEntity, TFields>
        : SqlReadRepository, IReadRepository<TEntity>
        where TEntity : class, IAggregateRoot
        where TFields : EntityFields
    {
        /// <summary>
        /// Gets the current table name.
        /// </summary>
        protected string TableName => Queries.Table.Name;

        protected string ColumnName => Queries.Table.Fields.Id.Name;
        protected string Id => Queries.Table.Fields.Id.ToString();

        /// <summary>
        /// The read queries instance.
        /// </summary>
        protected readonly IReadQueries<TFields> Queries;

        /// <summary>
        /// The sql delegated handlers for TEntity.
        /// </summary>
        protected readonly ISqlDelegatedHandlers<TEntity> EntitySqlDelegatedHandlers;

        /// <summary>
        /// Creates new instance of <see cref="ReadRepository{TEntity, TFields}"/>
        /// </summary>
        /// <param name="queries"></param>
        /// <param name="entitySqlDelegatedHandlers"></param>
        /// <param name="connectionFactory"></param>
        protected ReadRepository(IReadQueries<TFields> queries,
            ISqlDelegatedHandlers<TEntity> entitySqlDelegatedHandlers,
            IConnectionFactory connectionFactory)
            : base(connectionFactory)
        {
            Queries = queries;
            EntitySqlDelegatedHandlers = entitySqlDelegatedHandlers;
        }

        /// <summary>
        /// <see cref="IReadRepository{TEntity}.GetByIdAsync(object, IEnumerable{Field})"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public async Task<TEntity> GetByIdAsync(object id, IEnumerable<Field> fields = null) =>
            await ReadUsingConnectionAsync(async (connection) =>
            {
                return await EntitySqlDelegatedHandlers.GetSingleAsync(connection, Queries.ById(id, fields));
            });
        
    }
}