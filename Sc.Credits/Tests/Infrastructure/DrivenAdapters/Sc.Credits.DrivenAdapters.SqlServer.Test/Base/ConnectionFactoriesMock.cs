using Moq;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;

namespace Sc.Credits.DrivenAdapters.SqlServer.Test.Base
{
    public static class ConnectionFactoriesMock
    {
        //public static Mock<ICreditsConnectionFactory> Credits => ;

        public static Mock<IMessagingConnectionFactory> Messaging => new Mock<IMessagingConnectionFactory>();
    }
}