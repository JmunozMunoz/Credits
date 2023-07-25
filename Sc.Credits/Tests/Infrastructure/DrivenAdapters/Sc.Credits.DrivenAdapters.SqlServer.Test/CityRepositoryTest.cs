using Moq;
using Sc.Credits.Domain.Model.Locations;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Repository;
using Sc.Credits.DrivenAdapters.SqlServer.Test.Base;

namespace Sc.Credits.DrivenAdapters.SqlServer.Test
{
    public class CityRepositoryTest
        : CommandRepositoryTestBase<CityRepository, City, ICreditsConnectionFactory>
    {
        protected override CityRepository Repository => new CityRepository(ConnectionFactoryMock.Object,
            EntitySqlDelegatedHandlersMock.Object);

        protected override City GetEntity() => new City("1", "Test", "001");

        public CityRepositoryTest()
            : base(new Mock<ICreditsConnectionFactory>(), new Mock<ISqlDelegatedHandlers<City>>())
        {
        }
    }
}