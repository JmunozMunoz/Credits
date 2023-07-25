using Moq;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Dapper.Extensions;
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
    public class CreditMasterRepositoryTest
        : TransactionRepositoryTestBase<CreditMasterRepository, CreditMaster, ICreditsConnectionFactory>
    {
        private readonly Mock<ISqlDelegatedHandlers<Credit>> _creditSqlDelegatedHandlers = new Mock<ISqlDelegatedHandlers<Credit>>();
        private readonly Mock<ISqlDelegatedHandlers<TransactionReference>> _transactionReferenceDelegatedHandlers = new Mock<ISqlDelegatedHandlers<TransactionReference>>();

        private readonly Mock<ISettings<CredinetAppSettings>> _settingsMock = new Mock<ISettings<CredinetAppSettings>>();

        protected override CreditMaster GetEntity() => CreditMasterHelperTest.GetCreditMaster();

        protected override CreditMasterRepository Repository => new CreditMasterRepository(ConnectionFactoryMock.Object,
            EntitySqlDelegatedHandlersMock.Object,
            _creditSqlDelegatedHandlers.Object,
            _transactionReferenceDelegatedHandlers.Object,
            _settingsMock.Object);

        public CreditMasterRepositoryTest()
            : base(new Mock<ICreditsConnectionFactory>(), new Mock<ISqlDelegatedHandlers<CreditMaster>>())
        {
            _settingsMock.Setup(mock => mock.Get())
                .Returns(CredinetAppSettingsHelperTest.GetCredinetAppSettings());
        }

        [Fact]
        public async Task ShouldAddCredit()
        {
            EntitySqlDelegatedHandlersMock.Setup(mock => mock.InsertAsync(It.IsAny<IDbConnection>(), It.IsAny<CreditMaster>(), It.IsAny<string>(),
                    It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<ISqlAdapter>(), It.IsAny<object[]>()))
                .Verifiable();

            _creditSqlDelegatedHandlers.Setup(mock => mock.InsertAsync(It.IsAny<IDbConnection>(), It.IsAny<Credit>(), It.IsAny<string>(),
                  It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<ISqlAdapter>(), It.IsAny<object[]>()))
              .Verifiable();

            await Repository.AddAsync(CreditMasterHelperTest.GetCreditMaster());

            EntitySqlDelegatedHandlersMock.Verify();
            _creditSqlDelegatedHandlers.Verify();
        }

        [Fact]
        public async Task ShouldAddTransaction()
        {
            EntitySqlDelegatedHandlersMock.Setup(mock => mock.ExcecuteAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .Verifiable();

            _creditSqlDelegatedHandlers.Setup(mock => mock.InsertAsync(It.IsAny<IDbConnection>(), It.IsAny<Credit>(), It.IsAny<string>(),
                  It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<ISqlAdapter>(), It.IsAny<object[]>()))
              .Verifiable();

            _transactionReferenceDelegatedHandlers.Setup(mock => mock.InsertAsync(It.IsAny<IDbConnection>(), It.IsAny<TransactionReference>(), It.IsAny<string>(),
                  It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<ISqlAdapter>(), It.IsAny<object[]>()))
                .Verifiable();

            CreditMaster creditMaster = CreditMasterHelperTest.GetCreditMaster();

            creditMaster.Current.SetTransactionReference("NewTransactionId");

            await Repository.AddTransactionAsync(creditMaster);

            EntitySqlDelegatedHandlersMock.Verify();
            _creditSqlDelegatedHandlers.Verify();
            _transactionReferenceDelegatedHandlers.Verify();
        }

        [Fact]
        public async Task ShouldGetWithCurrentById()
        {
            EntitySqlDelegatedHandlersMock.Setup(mock => mock.QueryAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .Verifiable();

            await Repository.GetWithCurrentAsync(Guid.NewGuid());

            EntitySqlDelegatedHandlersMock.Verify();
        }

        [Fact]
        public async Task ShouldGetWithCurrentByIdWithCustomerAndStore()
        {
            EntitySqlDelegatedHandlersMock.Setup(mock => mock.QueryAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .Verifiable();

            await Repository.GetWithCurrentAsync(Guid.NewGuid(), CustomerHelperTest.GetCustomer(), StoreHelperTest.GetStore());

            EntitySqlDelegatedHandlersMock.Verify();
        }

        [Fact]
        public async Task ShouldGetWithCurrentByIds()
        {
            EntitySqlDelegatedHandlersMock.Setup(mock => mock.QueryAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .Verifiable();

            await Repository.GetWithCurrentAsync(new List<Guid>() { Guid.NewGuid() }, CustomerHelperTest.GetCustomer(),
                statuses: Enumerable.Empty<Statuses>());

            EntitySqlDelegatedHandlersMock.Verify();
        }

        [Fact]
        public async Task ShouldGetWithTransactionsById()
        {
            EntitySqlDelegatedHandlersMock.Setup(mock => mock.QueryAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .ReturnsAsync(new Collection<object>()
                {
                    new Dictionary<string, object>()
                    {
                        {
                            "Id",
                            Guid.NewGuid()
                        }
                    }
                });

            _creditSqlDelegatedHandlers.Setup(mock => mock.QueryAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                It.IsAny<int?>()))
              .Verifiable();

            await Repository.GetWithTransactionsAsync(Guid.NewGuid());

            _creditSqlDelegatedHandlers.Verify();
        }

        [Fact]
        public async Task ShouldGetWithTransactionsByCustomer()
        {
            EntitySqlDelegatedHandlersMock.Setup(mock => mock.QueryAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .Verifiable();

            _creditSqlDelegatedHandlers.Setup(mock => mock.QueryAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                It.IsAny<int?>()))
              .Verifiable();

            await Repository.GetWithTransactionsAsync(CustomerHelperTest.GetCustomer());

            EntitySqlDelegatedHandlersMock.Verify();
            _creditSqlDelegatedHandlers.Verify();
        }

        [Fact]
        public async Task ShouldGetWithTransactionsByIds()
        {
            EntitySqlDelegatedHandlersMock.Setup(mock => mock.QueryAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .Verifiable();

            _creditSqlDelegatedHandlers.Setup(mock => mock.QueryAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                It.IsAny<int?>()))
              .Verifiable();

            await Repository.GetWithTransactionsAsync(new List<Guid>() { Guid.NewGuid() });

            EntitySqlDelegatedHandlersMock.Verify();
            _creditSqlDelegatedHandlers.Verify();
        }

        [Fact]
        public async Task ShouldGetWithTransactionsByCreditId()
        {
            EntitySqlDelegatedHandlersMock.Setup(mock => mock.QueryAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .ReturnsAsync(new Collection<object>()
                {
                    new Dictionary<string, object>()
                    {
                        {
                            "Id",
                            Guid.NewGuid()
                        }
                    }
                });

            _creditSqlDelegatedHandlers.Setup(mock => mock.QueryAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                It.IsAny<int?>()))
              .Verifiable();

            await Repository.GetWithTransactionsByCreditIdAsync(Guid.NewGuid());

            EntitySqlDelegatedHandlersMock.Verify();
            _creditSqlDelegatedHandlers.Verify();
        }

        [Fact]
        public async Task ShouldGetActiveCreditsByCollectType()
        {
            EntitySqlDelegatedHandlersMock.Setup(mock => mock.QueryAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .Verifiable();

            await Repository.GetActiveCreditsByCollectTypeAsync(CustomerHelperTest.GetCustomer(), StoreHelperTest.GetStore());

            EntitySqlDelegatedHandlersMock.Verify();
        }

        [Fact]
        public async Task ShouldGetActiveCredits()
        {
            EntitySqlDelegatedHandlersMock.Setup(mock => mock.QueryAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .Verifiable();

            await Repository.GetActiveCreditsAsync(CustomerHelperTest.GetCustomer());

            EntitySqlDelegatedHandlersMock.Verify();
        }

        [Fact]
        public async Task ShouldGetPaidCreditsForCertificate()
        {
            EntitySqlDelegatedHandlersMock.Setup(mock => mock.QueryAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .Verifiable();

            await Repository.GetPaidCreditsForCertificateAsync(new List<Guid>() { Guid.NewGuid() });

            EntitySqlDelegatedHandlersMock.Verify();
        }

        [Fact]
        public async Task ShouldGetCustomerCreditHistory()
        {
            EntitySqlDelegatedHandlersMock.Setup(mock => mock.QueryAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .Verifiable();

            await Repository.GetCustomerCreditHistoryAsync(CustomerHelperTest.GetCustomer(), StoreHelperTest.GetStore().Id, maximumMonthsCreditHistory: 6);

            EntitySqlDelegatedHandlersMock.Verify();
        }

        [Fact]
        public async Task ShouldGetActiveAndPendingCancellationCredits()
        {
            EntitySqlDelegatedHandlersMock.Setup(mock => mock.QueryAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .Verifiable();

            _creditSqlDelegatedHandlers.Setup(mock => mock.QueryAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                It.IsAny<int?>()))
              .Verifiable();

            await Repository.GetActiveAndPendingCancellationCreditsAsync(CustomerHelperTest.GetCustomer(), StoreHelperTest.GetStore().GetVendorId,
                Enumerable.Empty<Field>(), Enumerable.Empty<Field>());

            EntitySqlDelegatedHandlersMock.Verify();
            _creditSqlDelegatedHandlers.Verify();
        }

        [Fact]
        public async Task ShouldGetActiveAndPendingCancellationPayments()
        {
            EntitySqlDelegatedHandlersMock.Setup(mock => mock.QueryAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .Verifiable();

            _creditSqlDelegatedHandlers.Setup(mock => mock.QueryAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                It.IsAny<int?>()))
              .Verifiable();

            await Repository.GetActiveAndPendingCancellationPaymentsAsync(CustomerHelperTest.GetCustomer());

            EntitySqlDelegatedHandlersMock.Verify();
            _creditSqlDelegatedHandlers.Verify();
        }

        [Fact]
        public async Task ShouldGetCustomerPaymentHistory()
        {
            EntitySqlDelegatedHandlersMock.Setup(mock => mock.QueryAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .Verifiable();

            _creditSqlDelegatedHandlers.Setup(mock => mock.QueryAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                It.IsAny<int?>()))
              .Verifiable();

            await Repository.GetCustomerPaymentHistoryAsync(CustomerHelperTest.GetCustomer(), StoreHelperTest.GetStore(),
                 Enumerable.Empty<Field>(), Enumerable.Empty<Field>(), maximumMonthsPaymentHistory: 6);

            EntitySqlDelegatedHandlersMock.Verify();
            _creditSqlDelegatedHandlers.Verify();
        }

        [Fact]
        public async Task ShouldGetPayments()
        {
            EntitySqlDelegatedHandlersMock.Setup(mock => mock.QueryAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .Verifiable();

            _creditSqlDelegatedHandlers.Setup(mock => mock.QueryAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .Verifiable();

            await Repository.GetPaymentsAsync(new List<Guid>() { Guid.NewGuid() }, Enumerable.Empty<Field>(),
                Enumerable.Empty<Field>(), Enumerable.Empty<Field>(), Enumerable.Empty<Field>(), Enumerable.Empty<Field>());

            EntitySqlDelegatedHandlersMock.Verify();
            _creditSqlDelegatedHandlers.Verify();
        }

        [Fact]
        public async Task ShouldGetTransactions()
        {
            EntitySqlDelegatedHandlersMock.Setup(mock => mock.QueryAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .Verifiable();

            _creditSqlDelegatedHandlers.Setup(mock => mock.QueryAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .Verifiable();

            await Repository.GetTransactionsAsync(new List<Guid>() { Guid.NewGuid() }, Enumerable.Empty<Field>(), Enumerable.Empty<Field>());

            EntitySqlDelegatedHandlersMock.Verify();
            _creditSqlDelegatedHandlers.Verify();
        }

        [Fact]
        public async Task IsDuplicated()
        {
            EntitySqlDelegatedHandlersMock.Setup(mock => mock.AnyAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .Verifiable();

            await Repository.IsDuplicatedAsync("05642", DateTime.Now, Guid.NewGuid());

            EntitySqlDelegatedHandlersMock.Verify();
        }

        [Fact]
        public async Task CustomerPhotoSignatureAllowed()
        {
            EntitySqlDelegatedHandlersMock.Setup(mock => mock.AnyAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .Verifiable();

            await Repository.ValidateACreditPaidAccordingToTime(Guid.NewGuid(), 30, Statuses.Active);

            EntitySqlDelegatedHandlersMock.Verify();
        }
    }
}