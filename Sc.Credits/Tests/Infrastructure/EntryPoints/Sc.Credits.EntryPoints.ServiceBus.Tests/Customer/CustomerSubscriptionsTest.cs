using Moq;
using Sc.Credits.EntryPoints.ServicesBus.Customer;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.EntryPoints.ServiceBus.Tests.Customer
{
    public class CustomerSubscriptionsTest
    {
        private readonly Mock<ICustomerUpdateEventSubscription> _customerUpdateEventSubscriptionMock =
            new Mock<ICustomerUpdateEventSubscription>();

        private readonly Mock<ICustomerStatusUpdateEventSubscription> _customerStatusUpdateEventSubscriptionMock =
            new Mock<ICustomerStatusUpdateEventSubscription>();

        private readonly Mock<ICustomerUpdateMailMobileEventSubscription> _customerUpdateMailMobileEventSubscriptionMock =
            new Mock<ICustomerUpdateMailMobileEventSubscription>();

        private readonly Mock<ICustomerUpdateMigrationEventSubscription> _customerUpdateMigrationEventSubscriptionMock =
            new Mock<ICustomerUpdateMigrationEventSubscription>();

        private readonly Mock<ICustomerUpdateStudyEventSubscription> _customerUpdateStudyEventSubscriptionMock =
            new Mock<ICustomerUpdateStudyEventSubscription>();

        private readonly Mock<ICustomerCreditLimitUpdateEventSubscription> _customerCreditLimitUpdateEventSubscriptionMock =
            new Mock<ICustomerCreditLimitUpdateEventSubscription>();

        public CustomerSubscriptions CustomerSubscriptions =>
            new CustomerSubscriptions(_customerUpdateEventSubscriptionMock.Object,
                _customerStatusUpdateEventSubscriptionMock.Object,
                _customerUpdateMailMobileEventSubscriptionMock.Object,
                _customerUpdateMigrationEventSubscriptionMock.Object,
                _customerUpdateStudyEventSubscriptionMock.Object,
                _customerCreditLimitUpdateEventSubscriptionMock.Object);

        [Fact]
        public async Task ShouldSubscribeAll()
        {
            _customerUpdateEventSubscriptionMock.Setup(mock => mock.SubscribeAsync()).Verifiable();
            _customerStatusUpdateEventSubscriptionMock.Setup(mock => mock.SubscribeAsync()).Verifiable();
            _customerUpdateMailMobileEventSubscriptionMock.Setup(mock => mock.SubscribeAsync()).Verifiable();
            _customerUpdateMigrationEventSubscriptionMock.Setup(mock => mock.SubscribeAsync()).Verifiable();
            _customerUpdateStudyEventSubscriptionMock.Setup(mock => mock.SubscribeAsync()).Verifiable();
            _customerCreditLimitUpdateEventSubscriptionMock.Setup(mock => mock.SubscribeAsync()).Verifiable();

            await CustomerSubscriptions.SubscribeAllAsync();

            _customerUpdateEventSubscriptionMock.Verify();
            _customerStatusUpdateEventSubscriptionMock.Verify();
            _customerUpdateMailMobileEventSubscriptionMock.Verify();
            _customerUpdateMigrationEventSubscriptionMock.Verify();
            _customerUpdateStudyEventSubscriptionMock.Verify();
            _customerCreditLimitUpdateEventSubscriptionMock.Verify();
        }
    }
}