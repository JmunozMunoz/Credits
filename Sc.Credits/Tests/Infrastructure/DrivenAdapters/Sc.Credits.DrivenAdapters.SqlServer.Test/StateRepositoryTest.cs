using Moq;
using Sc.Credits.Domain.Model.Locations;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Repository;
using Sc.Credits.DrivenAdapters.SqlServer.Test.Base;

namespace Sc.Credits.DrivenAdapters.SqlServer.Test
{
    public class StateRepositoryTest
        : CommandRepositoryTestBase<StateRepository, State, ICreditsConnectionFactory>
    {
        protected override State GetEntity() => new State("1", "Test");

        protected override StateRepository Repository => new StateRepository(ConnectionFactoryMock.Object,
            EntitySqlDelegatedHandlersMock.Object);

        public StateRepositoryTest()
            : base(new Mock<ICreditsConnectionFactory>(), new Mock<ISqlDelegatedHandlers<State>>())
        {
        }
    }
}