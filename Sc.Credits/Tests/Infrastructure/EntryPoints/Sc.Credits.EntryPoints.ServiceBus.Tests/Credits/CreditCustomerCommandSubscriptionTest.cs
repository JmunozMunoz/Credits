using Moq;
using org.reactivecommons.api;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.EntryPoints.ServicesBus.Credits;
using Sc.Credits.Helper.Test.Model;
using Sc.Credits.Helper.Test.ServiceBus;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.ObjectsUtils;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.EntryPoints.ServiceBus.Tests.Credits
{
    public class CreditCustomerCommandSubscriptionTest
    {
        private readonly Mock<IDirectAsyncGateway<CreditCustomerMigrationHistoryRequest>> _directAsyncCreditCustomerMigrationHistoryRequestMock = new Mock<IDirectAsyncGateway<CreditCustomerMigrationHistoryRequest>>();

        private readonly Mock<ICreditCustomerService> _creditCustomerServiceMock = new Mock<ICreditCustomerService>();
        private readonly Mock<ICommons> _commonsMock = new Mock<ICommons>();
        private readonly Mock<IAppParametersService> _appParametersServiceMock = new Mock<IAppParametersService>();

        private readonly Mock<ILoggerService<CreditCustomerCommandSubscription>> _loggerServiceMock =
            new Mock<ILoggerService<CreditCustomerCommandSubscription>>();

        private ICreditCustomerCommandSubscription CreditCustomerCommandSubscription =>
            new CreditCustomerCommandSubscription(_directAsyncCreditCustomerMigrationHistoryRequestMock.Object,
                _creditCustomerServiceMock.Object,
                _commonsMock.Object,
                _loggerServiceMock.Object);

        public CreditCustomerCommandSubscriptionTest()
        {
            _appParametersServiceMock.Setup(mock => mock.GetSettings())
                .Returns(new CredinetAppSettings
                {
                    CultureInfo = "es-CO"
                });

            _commonsMock.SetupGet(mock => mock.AppParameters)
                .Returns(_appParametersServiceMock.Object);

            _commonsMock.SetupGet(mock => mock.CredinetAppSettings)
                .Returns(_appParametersServiceMock.Object.GetSettings());
        }

        [Fact]
        public async Task ShouldSubscribe()
        {
            bool called = false;
            void callback() => called = true;

            ServiceBusHelperTest.SetupSubscribeCommandCallback(_directAsyncCreditCustomerMigrationHistoryRequestMock, callback);

            await CreditCustomerCommandSubscription.SubscribeAsync();

            Assert.True(called);
        }

        [Fact]
        public async Task ShouldInvokeSubscriptionCommand()
        {
            CreditCustomerMigrationHistoryRequest creditCustomerMigrationHistoryRequest = ModelHelperTest.InstanceModel<CreditCustomerMigrationHistoryRequest>();

            ServiceBusHelperTest.InvokeSubscriptionCommand(_directAsyncCreditCustomerMigrationHistoryRequestMock, creditCustomerMigrationHistoryRequest);

            await CreditCustomerCommandSubscription.SubscribeAsync();

            _creditCustomerServiceMock.Verify(mock => mock.SendMigrationHistoryAsync(It.IsAny<CreditCustomerMigrationHistoryRequest>()));
        }
    }
}