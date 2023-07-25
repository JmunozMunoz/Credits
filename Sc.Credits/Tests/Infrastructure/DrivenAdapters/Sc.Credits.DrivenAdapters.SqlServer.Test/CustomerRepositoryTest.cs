using Moq;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Repository;
using Sc.Credits.DrivenAdapters.SqlServer.Test.Base;
using Sc.Credits.Helper.Test.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.DrivenAdapters.SqlServer.Test
{
    public class CustomerRepositoryTest
        : CommandRepositoryTestBase<CustomerRepository, Customer, ICreditsConnectionFactory>
    {
        private readonly Mock<ISqlDelegatedHandlers<Profile>> _profileSqlDelegatedHandlers = new Mock<ISqlDelegatedHandlers<Profile>>();

        protected override Customer GetEntity() => CustomerHelperTest.GetCustomer();

        protected override CustomerRepository Repository => new CustomerRepository(ConnectionFactoryMock.Object,
            EntitySqlDelegatedHandlersMock.Object,
            _profileSqlDelegatedHandlers.Object);

        public CustomerRepositoryTest()
            : base(new Mock<ICreditsConnectionFactory>(), new Mock<ISqlDelegatedHandlers<Customer>>())
        {
        }

        [Fact]
        public async Task ShouldGetById()
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
                        },
                        {
                            "ProfileId",
                            1
                        }
                    }
                });

            _profileSqlDelegatedHandlers.Setup(mock => mock.GetSingleAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .ReturnsAsync(ProfileHelperTest.GetProfile());

            Customer result = await Repository.GetByIdAsync(Guid.NewGuid(), Enumerable.Empty<Field>(), new List<Field> { new Field("Id", "Profile") });

            Assert.NotNull(result);
        }

        [Fact]
        public async Task ShouldGetByDocumentInfo()
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
                        },
                        {
                            "ProfileId",
                            1
                        }
                    }
                });

            _profileSqlDelegatedHandlers.Setup(mock => mock.GetSingleAsync(It.IsAny<IDbConnection>(), It.IsAny<SqlQuery>(), It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>()))
                .ReturnsAsync(ProfileHelperTest.GetProfile());

            Customer result = await Repository.GetByDocumentInfoAsync("1252251452", "CC", Enumerable.Empty<Field>(), new List<Field> { new Field("Id", "Profile") });

            Assert.NotNull(result);
        }
    }
}