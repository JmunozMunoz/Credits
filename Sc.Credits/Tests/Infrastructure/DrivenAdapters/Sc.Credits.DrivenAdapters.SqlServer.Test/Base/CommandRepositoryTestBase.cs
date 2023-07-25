using Moq;
using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Dapper.Extensions;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace Sc.Credits.DrivenAdapters.SqlServer.Test.Base
{
    public abstract class CommandRepositoryTestBase<TRepository, TEntity, TConnectionFactory>
        : ReadRepositoryTestBase<TRepository, TEntity, TConnectionFactory>
        where TRepository : ICommandRepository<TEntity>
        where TEntity : class, IAggregateRoot
        where TConnectionFactory : class, IConnectionFactory
    {
        protected CommandRepositoryTestBase(Mock<TConnectionFactory> connectionFactoryMock,
            Mock<ISqlDelegatedHandlers<TEntity>> entitySqlDelegatedHandlersMock)
            : base(connectionFactoryMock, entitySqlDelegatedHandlersMock)
        {
        }

        [Fact]
        public async Task ShouldAdd()
        {
            TEntity entity = GetEntity();

            EntitySqlDelegatedHandlersMock.Setup(mock => mock.InsertAsync(It.IsAny<IDbConnection>(), It.IsAny<TEntity>(), It.IsAny<string>(),
                    It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<ISqlAdapter>(), It.IsAny<object[]>()))
                .Verifiable();

            await Repository.AddAsync(entity, Transaction.Current);

            EntitySqlDelegatedHandlersMock.Verify();
        }

        [Fact]
        public async Task ShouldUpdate()
        {
            TEntity entity = GetEntity();

            EntitySqlDelegatedHandlersMock.Setup(mock => mock.ExcecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .Verifiable();

            await Repository.UpdateAsync(entity, new List<Field>()
                {
                    new Field("Id", "Test")
                },
            Transaction.Current);

            EntitySqlDelegatedHandlersMock.Verify();
        }
    }
}