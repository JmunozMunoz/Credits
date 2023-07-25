using Moq;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Repository;
using Sc.Credits.Helper.Test.Model;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.DrivenAdapters.SqlServer.Test
{
    public class AppParametersRepositoryTest
    {
        private readonly Mock<DbConnection> _dbConnectionMock = new Mock<DbConnection>();
        private readonly Mock<ICreditsConnectionFactory> _connectionFactoryMock = new Mock<ICreditsConnectionFactory>();

        private readonly Mock<ISqlDelegatedHandlers<AuthMethod>> _authMethodSqlDelegatedHandlersMock = new Mock<ISqlDelegatedHandlers<AuthMethod>>();
        private readonly Mock<ISqlDelegatedHandlers<Parameter>> _parameterSqlDelegatedHandlersMock = new Mock<ISqlDelegatedHandlers<Parameter>>();
        private readonly Mock<ISqlDelegatedHandlers<PaymentType>> _paymentTypeSqlDelegatedHandlersMock = new Mock<ISqlDelegatedHandlers<PaymentType>>();
        private readonly Mock<ISqlDelegatedHandlers<Source>> _sourceSqlDelegatedHandlersMock = new Mock<ISqlDelegatedHandlers<Source>>();
        private readonly Mock<ISqlDelegatedHandlers<Status>> _statusSqlDelegatedHandlersMock = new Mock<ISqlDelegatedHandlers<Status>>();
        private readonly Mock<ISqlDelegatedHandlers<TransactionType>> _transactionTypeSqlDelegatedHandlersMock = new Mock<ISqlDelegatedHandlers<TransactionType>>();

        private AppParametersRepository AppParametersRepository => new AppParametersRepository(_connectionFactoryMock.Object,
            _parameterSqlDelegatedHandlersMock.Object,
            _transactionTypeSqlDelegatedHandlersMock.Object,
            _statusSqlDelegatedHandlersMock.Object,
            _sourceSqlDelegatedHandlersMock.Object,
            _paymentTypeSqlDelegatedHandlersMock.Object,
            _authMethodSqlDelegatedHandlersMock.Object);

        public AppParametersRepositoryTest()
        {
            _connectionFactoryMock.Setup(mock => mock.Create())
                .Returns(_dbConnectionMock.Object);
        }

        [Fact]
        public async Task ShouldGetAllParameters()
        {
            _parameterSqlDelegatedHandlersMock.Setup(mock => mock.GetListAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .ReturnsAsync(ParameterHelperTest.GetParameters());

            List<Parameter> parameters = await AppParametersRepository.GetAllParametersAsync();

            Assert.True(parameters.Any());
        }

        [Fact]
        public async Task ShouldGetTransactionType()
        {
            _transactionTypeSqlDelegatedHandlersMock.Setup(mock => mock.GetSingleAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .ReturnsAsync(TransactionTypeHelperTest.GetCreateCreditType());

            TransactionType transactionType = await AppParametersRepository.GetTransactionTypeAsync(1);

            Assert.NotNull(transactionType);
        }

        [Fact]
        public async Task ShouldGetStatus()
        {
            _statusSqlDelegatedHandlersMock.Setup(mock => mock.GetSingleAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .ReturnsAsync(StatusHelperTest.GetActiveStatus());

            Status status = await AppParametersRepository.GetStatusAsync(1);

            Assert.NotNull(status);
        }

        [Fact]
        public async Task ShouldGetSource()
        {
            _sourceSqlDelegatedHandlersMock.Setup(mock => mock.GetSingleAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .ReturnsAsync(SourceHelperTest.GetCredinetSource());

            Source source = await AppParametersRepository.GetSourceAsync(1);

            Assert.NotNull(source);
        }

        [Fact]
        public async Task ShouldGetPaymentType()
        {
            _paymentTypeSqlDelegatedHandlersMock.Setup(mock => mock.GetSingleAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .ReturnsAsync(PaymentTypeHelperTest.GetOrdinaryPaymentType());

            PaymentType paymentType = await AppParametersRepository.GetPaymentTypeAsync(1);

            Assert.NotNull(paymentType);
        }

        [Fact]
        public async Task ShouldGetAuthMethod()
        {
            _authMethodSqlDelegatedHandlersMock.Setup(mock => mock.GetSingleAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .ReturnsAsync(AuthMethodHelperTest.GetTokenAuthMethod());

            AuthMethod authMethod = await AppParametersRepository.GetAuthMethodAsync(1);

            Assert.NotNull(authMethod);
        }
    }
}