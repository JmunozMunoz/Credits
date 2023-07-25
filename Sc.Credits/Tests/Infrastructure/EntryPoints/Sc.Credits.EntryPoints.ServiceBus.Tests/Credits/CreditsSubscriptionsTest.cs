using Moq;
using Sc.Credits.EntryPoints.ServicesBus.Credits;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.EntryPoints.ServiceBus.Tests.Credits
{
    public class CreditsSubscriptionsTest
    {
        private readonly Mock<ICreditCommandSubscription> _creditCommandSubscriptionMock = new Mock<ICreditCommandSubscription>();
        private readonly Mock<ICreditCreateCommandSubscription> _creditCreateCommandSubscriptionMock = new Mock<ICreditCreateCommandSubscription>();
        private readonly Mock<ICreditQueryCommandSubscription> _creditQueryCommandSubscriptionMock = new Mock<ICreditQueryCommandSubscription>();

        public CreditsSubscriptions CreditsSubscriptions =>
            new CreditsSubscriptions(_creditCommandSubscriptionMock.Object,
                _creditCreateCommandSubscriptionMock.Object,
                _creditQueryCommandSubscriptionMock.Object);

        [Fact]
        public async Task ShouldSubscribeAll()
        {
            _creditCommandSubscriptionMock.Setup(mock => mock.SubscribeAsync()).Verifiable();
            _creditCreateCommandSubscriptionMock.Setup(mock => mock.SubscribeAsync()).Verifiable();
            _creditQueryCommandSubscriptionMock.Setup(mock => mock.SubscribeAsync()).Verifiable();

            await CreditsSubscriptions.SubscribeAllAsync();

            _creditCommandSubscriptionMock.VerifyAll();
            _creditCreateCommandSubscriptionMock.VerifyAll();
            _creditQueryCommandSubscriptionMock.VerifyAll();
        }
    }
}