using Moq;
using Sc.Credits.Domain.Model.Base;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.DrivenAdapters.SqlServer.Test.Base
{
    public abstract class ReadRepositoryTestBase<TRepository, TEntity, TConnectionFactory>
        where TRepository : IReadRepository<TEntity>
        where TEntity : class, IAggregateRoot
        where TConnectionFactory : class, IConnectionFactory
    {
        protected readonly Mock<TConnectionFactory> ConnectionFactoryMock;
        protected readonly Mock<ISqlDelegatedHandlers<TEntity>> EntitySqlDelegatedHandlersMock;

        protected abstract TRepository Repository { get; }

        protected abstract TEntity GetEntity();

        private readonly Mock<DbConnection> _dbConnectionMock = new Mock<DbConnection>();

        protected ReadRepositoryTestBase(Mock<TConnectionFactory> connectionFactoryMock,
            Mock<ISqlDelegatedHandlers<TEntity>> entitySqlDelegatedHandlersMock)
        {
            ConnectionFactoryMock = connectionFactoryMock;
            EntitySqlDelegatedHandlersMock = entitySqlDelegatedHandlersMock;

            ConnectionFactoryMock.Setup(mock => mock.Create())
                .Returns(_dbConnectionMock.Object);

            EntitySqlDelegatedHandlersMock.Setup(mock => mock.GetSingleAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(),
                    It.IsAny<IDbTransaction>(), It.IsAny<int?>()))
                .ReturnsAsync(GetEntity());
        }

        [Fact]
        public async Task ShouldGetEntityById()
        {
            TEntity result = await Repository.GetByIdAsync(default);

            Assert.NotNull(result);
            Assert.IsType<TEntity>(result);
        }
    }
}