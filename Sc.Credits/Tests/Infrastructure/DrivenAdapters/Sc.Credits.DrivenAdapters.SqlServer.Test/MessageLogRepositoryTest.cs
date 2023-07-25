using Moq;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Dapper.Extensions;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Repository;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Helpers.Commons.Messaging;
using Sc.Credits.Helpers.Commons.Messaging.Gateway;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.DrivenAdapters.SqlServer.Test
{
    public class MessageLogRepositoryTest
    {
        private readonly Mock<DbConnection> _dbConnectionMock = new Mock<DbConnection>();
        private readonly Mock<IMessagingConnectionFactory> _connectionFactoryMock = new Mock<IMessagingConnectionFactory>();
        private readonly Mock<ISqlDelegatedHandlers<MessageErrorLog>> _messageErrorLogFakesSqlDelegatedHandlersMock = new Mock<ISqlDelegatedHandlers<MessageErrorLog>>();

        public IMessageLogRepository MessageLogRepository => new MessageLogRepository(_connectionFactoryMock.Object,
                _messageErrorLogFakesSqlDelegatedHandlersMock.Object);

        public MessageLogRepositoryTest()
        {
            _connectionFactoryMock.Setup(mock => mock.Create())
                .Returns(_dbConnectionMock.Object);
        }

        [Fact]
        public async Task ShouldAddError()
        {
            _messageErrorLogFakesSqlDelegatedHandlersMock.Setup(mock => mock.InsertAsync(It.IsAny<IDbConnection>(), It.IsAny<MessageErrorLog>(), It.IsAny<string>(),
                    It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<ISqlAdapter>(), It.IsAny<object[]>()))
                .Verifiable();

            await MessageLogRepository.AddErrorAsync(MessageLogHelperTest.GetErrorDefault());

            _messageErrorLogFakesSqlDelegatedHandlersMock.VerifyAll();
        }
    }
}