using Moq;
using Sc.Credits.Domain.Model.Refinancings;
using Sc.Credits.Domain.Model.Refinancings.Gateway;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Dapper.Extensions;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Repository;
using Sc.Credits.Helper.Test.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace Sc.Credits.DrivenAdapters.SqlServer.Test
{
    public class RefinancingRepositoryTest
    {
        private readonly Mock<DbConnection> _dbConnectionMock = new Mock<DbConnection>();
        private readonly Mock<ICreditsConnectionFactory> _connectionFactoryMock = new Mock<ICreditsConnectionFactory>();

        private readonly Mock<ISqlDelegatedHandlers<RefinancingApplication>> _refinancingApplicationSqlDelegatedHandlers = new Mock<ISqlDelegatedHandlers<RefinancingApplication>>();
        private readonly Mock<ISqlDelegatedHandlers<RefinancingLog>> _refinancingLogSqlDelegatedHandlers = new Mock<ISqlDelegatedHandlers<RefinancingLog>>();
        private readonly Mock<ISqlDelegatedHandlers<RefinancingLogDetail>> _refinancingLogDetailSqlDelegatedHandlers = new Mock<ISqlDelegatedHandlers<RefinancingLogDetail>>();

        public IRefinancingRepository RefinancingRepository => new RefinancingRepository(_connectionFactoryMock.Object,
            _refinancingApplicationSqlDelegatedHandlers.Object,
            _refinancingLogSqlDelegatedHandlers.Object,
            _refinancingLogDetailSqlDelegatedHandlers.Object);

        public RefinancingRepositoryTest()
        {
            _connectionFactoryMock.Setup(mock => mock.Create())
                .Returns(_dbConnectionMock.Object);
        }

        [Fact]
        public async Task ShouldGetApplication()
        {
            RefinancingApplication refinancingApplication = RefinancingApplicationHelperTest.GetDefault();

            _refinancingApplicationSqlDelegatedHandlers.Setup(mock => mock.GetSingleAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .ReturnsAsync(refinancingApplication);

            RefinancingApplication result = await RefinancingRepository.GetApplicationAsync(refinancingApplication.Id);

            Assert.Equal(refinancingApplication, result);
        }

        [Fact]
        public async Task ShouldAddLog()
        {
            _refinancingLogSqlDelegatedHandlers.Setup(mock => mock.InsertAsync(It.IsAny<IDbConnection>(), It.IsAny<RefinancingLog>(), It.IsAny<string>(),
                    It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<ISqlAdapter>(), It.IsAny<object[]>()))
                .Verifiable();

            _refinancingLogDetailSqlDelegatedHandlers.Setup(mock => mock.InsertAsync(It.IsAny<IDbConnection>(), It.IsAny<RefinancingLogDetail>(), It.IsAny<string>(),
                    It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<ISqlAdapter>(), It.IsAny<object[]>()))
                .Verifiable();

            RefinancingLog refinancingLog = new RefinancingLog(applicationId: Guid.NewGuid(), creditMasterId: Guid.NewGuid(),
                    value: 80000, storeId: "testStoreId", customerId: Guid.NewGuid());

            List<RefinancingLogDetail> details = CreditMasterHelperTest.GetCreditMasterList()
                .Select(creditMaster =>
                    refinancingLog.CreateDetail(creditMaster.Id, creditMaster.Current.Id, 20000, 3000))
                .ToList();

            refinancingLog.SetDetails(details);

            await RefinancingRepository.AddLogAsync(refinancingLog, Transaction.Current);

            _refinancingLogSqlDelegatedHandlers.Verify();
            _refinancingLogDetailSqlDelegatedHandlers.Verify();
        }
    }
}