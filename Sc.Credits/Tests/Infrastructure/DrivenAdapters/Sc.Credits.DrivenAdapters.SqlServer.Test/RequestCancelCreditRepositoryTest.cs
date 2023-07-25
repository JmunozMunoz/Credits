using Moq;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Repository;
using Sc.Credits.DrivenAdapters.SqlServer.Test.Base;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.DrivenAdapters.SqlServer.Test
{
    public class RequestCancelCreditRepositoryTest
        : CommandRepositoryTestBase<RequestCancelCreditRepository, RequestCancelCredit, ICreditsConnectionFactory>
    {
        private readonly Mock<ISqlDelegatedHandlers<CreditMaster>> _creditMasterSqlDelegatedHandlers = new Mock<ISqlDelegatedHandlers<CreditMaster>>();
        private readonly Mock<ISqlDelegatedHandlers<Credit>> _creditSqlDelegatedHandlers = new Mock<ISqlDelegatedHandlers<Credit>>();

        private readonly Mock<ISettings<CredinetAppSettings>> _settingsMock = new Mock<ISettings<CredinetAppSettings>>();

        protected override RequestCancelCredit GetEntity() => RequestCancelCreditHelperTest.GetRequestCancelCredit();

        protected override RequestCancelCreditRepository Repository => new RequestCancelCreditRepository(ConnectionFactoryMock.Object,
            EntitySqlDelegatedHandlersMock.Object,
            _creditMasterSqlDelegatedHandlers.Object,
            _creditSqlDelegatedHandlers.Object,
            _settingsMock.Object);

        public RequestCancelCreditRepositoryTest()
            : base(new Mock<ICreditsConnectionFactory>(), new Mock<ISqlDelegatedHandlers<RequestCancelCredit>>())
        {
            _settingsMock.Setup(mock => mock.Get())
                .Returns(CredinetAppSettingsHelperTest.GetCredinetAppSettings());
        }

        [Fact]
        public async Task ShouldGetByStatusByCreditMaster()
        {
            EntitySqlDelegatedHandlersMock.Setup(mock => mock.QueryAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .Verifiable();

            await Repository.GetByStatusAsync(Guid.NewGuid(), RequestStatuses.Pending);

            EntitySqlDelegatedHandlersMock.Verify();
        }

        [Fact]
        public async Task ShouldGetByStatus()
        {
            EntitySqlDelegatedHandlersMock.Setup(mock => mock.GetListAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .Verifiable();

            await Repository.GetByStatusAsync(new List<Guid>() { Guid.NewGuid() }, RequestStatuses.Pending);

            EntitySqlDelegatedHandlersMock.Verify();
        }

        [Fact]
        public async Task ShouldGetByVendor()
        {
            List<RequestCancelCredit> requestCancelCredits = RequestCancelCreditHelperTest.GetRequestCancelCreditList();

            ICollection<object> creditMasterQueryResults = new Collection<object>();

            requestCancelCredits.ForEach(request =>
            {
                creditMasterQueryResults.Add(new Dictionary<string, object>()
                {
                    {
                        "Id",
                        request.GetCreditMasterId
                    }
                });
            });

            EntitySqlDelegatedHandlersMock.Setup(mock => mock.GetListAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .ReturnsAsync(requestCancelCredits);

            _creditMasterSqlDelegatedHandlers.Setup(mock => mock.QueryAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .ReturnsAsync(creditMasterQueryResults);

            _creditSqlDelegatedHandlers.Setup(mock => mock.QueryAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .Verifiable();

            await Repository.GetByVendorAsync("524242245", 1, 1,false, RequestStatuses.Pending, Enumerable.Empty<Field>(),
                Enumerable.Empty<Field>(), Enumerable.Empty<Field>(), Enumerable.Empty<Field>(), Enumerable.Empty<Field>());

            EntitySqlDelegatedHandlersMock.Verify();
            _creditSqlDelegatedHandlers.Verify();
        }
    }
}