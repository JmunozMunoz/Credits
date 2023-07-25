using Moq;
using Sc.Credits.Domain.Model.Sequences;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Dapper.Extensions;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Repository;
using Sc.Credits.Helper.Test.Model;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.DrivenAdapters.SqlServer.Test
{
    public class SequenceRepositoryTest
    {
        private readonly Mock<ICreditsConnectionFactory> _connectionFactoryMock = new Mock<ICreditsConnectionFactory>();
        private readonly Mock<DbConnection> _dbConnectionMock = new Mock<DbConnection>();

        private readonly Mock<ISqlDelegatedHandlers<Sequence>> _sequenceSqlDelegatedHandlers = new Mock<ISqlDelegatedHandlers<Sequence>>();

        private SequenceRepository SequenceRepository => new SequenceRepository(_connectionFactoryMock.Object,
            _sequenceSqlDelegatedHandlers.Object);

        public SequenceRepositoryTest()
        {
            _connectionFactoryMock.Setup(mock => mock.Create())
                .Returns(_dbConnectionMock.Object);
        }

        [Fact]
        public void ShouldCallConstructorThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new SequenceRepository(null, null));
        }

        [Fact]
        public async Task ShouldGetNextAndAddSequence()
        {
            _sequenceSqlDelegatedHandlers.Setup(mock => mock.InsertAsync(It.IsAny<IDbConnection>(), It.IsAny<Sequence>(), It.IsAny<string>(),
                    It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<ISqlAdapter>(), It.IsAny<object[]>()))
                .Verifiable();

            long nextSequence = await SequenceRepository.GetNextAsync("5a85d5f45aq5q", "Credits");

            Assert.Equal(1, nextSequence);
            _sequenceSqlDelegatedHandlers.Verify();
            _sequenceSqlDelegatedHandlers.Verify(mock => mock.ExcecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                It.IsAny<int?>()), Times.Never);
        }

        [Fact]
        public async Task ShouldGetNextAndUpdateSequence()
        {
            _sequenceSqlDelegatedHandlers.Setup(mock => mock.ExcecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .Verifiable();

            _sequenceSqlDelegatedHandlers.Setup(mock => mock.GetSingleAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .ReturnsAsync(SequenceHelperTest.GetSequenceList().First());

            long nextSequence = await SequenceRepository.GetNextAsync("5ce5a1ae195f4428203a1d9e", "Credits");

            Assert.Equal(2, nextSequence);
            _sequenceSqlDelegatedHandlers.Verify();
            _sequenceSqlDelegatedHandlers.Verify(mock => mock.InsertAsync(It.IsAny<IDbConnection>(), It.IsAny<Sequence>(), It.IsAny<string>(),
                It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<ISqlAdapter>(), It.IsAny<object[]>()), Times.Never);
        }
    }
}