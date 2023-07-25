using Moq;
using Sc.Credits.Domain.Model.Base;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;

namespace Sc.Credits.DrivenAdapters.SqlServer.Test.Base
{
    public abstract class TransactionRepositoryTestBase<TRepository, TEntity, TConnectionFactory>
        : CommandRepositoryTestBase<TRepository, TEntity, TConnectionFactory>
        where TRepository : ITransactionRepository<TEntity>
        where TEntity : class, IAggregateRoot
        where TConnectionFactory : class, IConnectionFactory
    {
        protected TransactionRepositoryTestBase(Mock<TConnectionFactory> connectionFactory,
            Mock<ISqlDelegatedHandlers<TEntity>> entitySqlDelegatedHandlersMock)
            : base(connectionFactory, entitySqlDelegatedHandlersMock)
        {
        }
    }
}