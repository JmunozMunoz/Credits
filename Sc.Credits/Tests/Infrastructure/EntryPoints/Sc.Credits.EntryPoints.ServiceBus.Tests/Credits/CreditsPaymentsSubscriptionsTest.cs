using Moq;
using Sc.Credits.EntryPoints.ServicesBus.Credits;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.EntryPoints.ServiceBus.Tests.Credits
{
    public class CreditsPaymentsSubscriptionsTest
    {
        private readonly Mock<IActiveCreditsCommandSubscription> _activeCreditsCommandSubscriptionMock = new Mock<IActiveCreditsCommandSubscription>();
        private readonly Mock<IPayCreditsCommandSubscription> _payCreditsCommandSubscriptionMock = new Mock<IPayCreditsCommandSubscription>();

        public CreditsPaymentsSubscription creditsPaymentsSubscriptions =>
            new CreditsPaymentsSubscription(_activeCreditsCommandSubscriptionMock.Object, _payCreditsCommandSubscriptionMock.Object);

        [Fact]
        public async Task ShouldSubscribeAll()
        {
            _activeCreditsCommandSubscriptionMock.Setup(mock => mock.SubscribeAsync()).Verifiable();
            await creditsPaymentsSubscriptions.SubscribeAllAsync();
            _activeCreditsCommandSubscriptionMock.VerifyAll();
        }
    }
}