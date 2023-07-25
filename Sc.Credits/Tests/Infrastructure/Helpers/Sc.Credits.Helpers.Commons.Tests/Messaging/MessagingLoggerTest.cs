using Moq;
using org.reactivecommons.api.domain;
using Sc.Credits.DrivenAdapters.ServiceBus.Model;
using Sc.Credits.Helpers.Commons.Messaging;
using Sc.Credits.Helpers.Commons.Messaging.Gateway;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.Helpers.Commons.Tests.Messaging
{
    public class MessagingLoggerTest
    {
        private readonly Mock<IMessageLogRepository> _messageLogRepositoryMock = new Mock<IMessageLogRepository>();

        public IMessagingLogger MessagingLogger =>
            new MessagingLogger(_messageLogRepositoryMock.Object);

        [Fact]
        public async Task ShouldLogTopicError()
        {
            await MessagingLogger.LogTopicErrorAsync(@event: new DomainEvent<dynamic>(
                name: "Credit.TransactionType(Create Credit)", eventId: "f3a01a4a-bd62-ce5c-1e1e-08d817d691d5",
                new CreditEvent()),
                resourceName: "credits_events");

            _messageLogRepositoryMock.Verify(mock => mock.AddErrorAsync(It.IsAny<MessageErrorLog>()), Times.Once);
        }
    }
}