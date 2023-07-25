using Moq;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Locations;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Repository;
using Sc.Credits.DrivenAdapters.SqlServer.Test.Base;
using Sc.Credits.Helper.Test.Model;

namespace Sc.Credits.DrivenAdapters.SqlServer.Test
{
    public class StoreRepositoryTest
        : CommandRepositoryTestBase<StoreRepository, Store, ICreditsConnectionFactory>
    {
        private readonly Mock<ISqlDelegatedHandlers<PaymentType>> _paymentTypeSqlDelegatedHandlersMock = new Mock<ISqlDelegatedHandlers<PaymentType>>();
        private readonly Mock<ISqlDelegatedHandlers<StoreCategory>> _storeCategorySqlDelegatedHandlersMock = new Mock<ISqlDelegatedHandlers<StoreCategory>>();
        private readonly Mock<ISqlDelegatedHandlers<AssuranceCompany>> _assuranceCompanySqlDelegatedHandlersMock = new Mock<ISqlDelegatedHandlers<AssuranceCompany>>();
        private readonly Mock<ISqlDelegatedHandlers<CollectType>> _collectTypeSqlDelegatedHandlersMock = new Mock<ISqlDelegatedHandlers<CollectType>>();
        private readonly Mock<ISqlDelegatedHandlers<StoreIdentification>> _storeIdentificationSqlDelegatedHandlersMock = new Mock<ISqlDelegatedHandlers<StoreIdentification>>();
        private readonly Mock<ISqlDelegatedHandlers<BusinessGroup>> _businessGroupSqlDelegatedHandlersMock = new Mock<ISqlDelegatedHandlers<BusinessGroup>>();
        private readonly Mock<ISqlDelegatedHandlers<City>> _citySqlDelegatedHandlersMock = new Mock<ISqlDelegatedHandlers<City>>();

        protected override Store GetEntity() => StoreHelperTest.GetStore();

        protected override StoreRepository Repository => new StoreRepository(ConnectionFactoryMock.Object,
            EntitySqlDelegatedHandlersMock.Object,
            _paymentTypeSqlDelegatedHandlersMock.Object,
            _storeCategorySqlDelegatedHandlersMock.Object,
            _assuranceCompanySqlDelegatedHandlersMock.Object,
            _collectTypeSqlDelegatedHandlersMock.Object,
            _storeIdentificationSqlDelegatedHandlersMock.Object,
            _businessGroupSqlDelegatedHandlersMock.Object,
            _citySqlDelegatedHandlersMock.Object);

        public StoreRepositoryTest()
            : base(new Mock<ICreditsConnectionFactory>(), new Mock<ISqlDelegatedHandlers<Store>>())
        {
        }

        //[Fact]
        //public async Task ShouldGetTransactionType()
        //{
        //    _creditsContextMock.Setup(mock => mock.FindAsync<CollectType>(It.IsAny<object[]>()))
        //        .ReturnsAsync(new CollectType("Prueba"));

        // CollectType collectType = await _storeRepository.GetCollectTypeAsync(1);

        //    Assert.NotNull(collectType);
        //}

        //[Fact]
        //public void ShouldAddStoreIdentification()
        //{
        //    StoreIdentification entity = new StoreIdentification("123456", "123");

        // ConfigureEntitySetMock(new List<StoreIdentification> { entity });

        // StoreIdentification newEntity = _storeRepository.AddStoreIdentification(entity);

        //    _creditsContextMock.Verify(mock => mock.Add(It.IsAny<StoreIdentification>()), Times.Once());
        //    Assert.NotNull(newEntity);
        //}

        //[Fact]
        //public async Task ShouldGetStoreIdentificationBySpecificationAsync()
        //{
        //    StoreIdentification entity = new StoreIdentification("123456", "123");

        // ConfigureEntitySetMock(new List<StoreIdentification> { entity });

        // StoreIdentification result = await
        // _storeRepository.GetStoreIdentificationBySpecificationAsync(new
        // DirectSpecification<StoreIdentification>(spec => spec.StoreId == "123456"));

        //    Assert.NotNull(result);
        //    Assert.IsType<StoreIdentification>(result);
        //}

        //[Fact]
        //public void ShouldUpdateStoreIdentification()
        //{
        //    StoreIdentification entity = new StoreIdentification("123456", "123");

        // ConfigureEntitySetMock(new List<StoreIdentification> { entity });

        // StoreIdentification editEntity = _storeRepository.UpdateStoreIdentification(entity);

        //    _creditsContextMock.Verify(mock => mock.Update(It.IsAny<StoreIdentification>()), Times.Once());
        //    Assert.NotNull(editEntity);
        //}

        //[Fact]
        //public async Task ShouldGetBusinessGroup()
        //{
        //    List<BusinessGroup> businessGroupList = new List<BusinessGroup>
        //    {
        //        new BusinessGroup("Test1", "Business group 1"),
        //        new BusinessGroup("Test2", "Business group 2"),
        //        new BusinessGroup("Test3", "Business group 3")
        //    };

        // string findId = "Test2";

        // ConfigureEntitySetMock(businessGroupList);

        // BusinessGroup businessGroup = await _storeRepository.GetBusinessGroupAsync(findId);

        //    Assert.NotNull(businessGroup);
        //    Assert.Equal(findId, businessGroup.Id);
        //}

        //[Fact]
        //public async Task ShouldAddBusinessGroup()
        //{
        //    List<BusinessGroup> businessGroupList = new List<BusinessGroup>
        //    {
        //        new BusinessGroup("Test1", "Business group 1"),
        //        new BusinessGroup("Test2", "Business group 2"),
        //        new BusinessGroup("Test3", "Business group 3")
        //    };

        // BusinessGroup businessGroup = new BusinessGroup("Test4", "Business new 4");

        // ConfigureEntitySetMock(businessGroupList);

        // await _storeRepository.AddBusinessGroupAsync(businessGroup);

        //    _creditsContextMock.Verify(mock => mock.AddAsync(It.IsAny<BusinessGroup>(), It.IsAny<CancellationToken>()), Times.Once());
        //}

        //[Fact]
        //public void ShouldUpdateBusinessGroup()
        //{
        //    List<BusinessGroup> businessGroupList = new List<BusinessGroup>
        //    {
        //        new BusinessGroup("Test1", "Business group 1"),
        //        new BusinessGroup("Test2", "Business group 2"),
        //        new BusinessGroup("Test3", "Business group 3")
        //    };

        // BusinessGroup businessGroupUpdate = new BusinessGroup("Test1", "Business edit 1");

        // ConfigureEntitySetMock(businessGroupList);

        // _storeRepository.UpdateBusinessGroup(businessGroupUpdate);

        //    _creditsContextMock.Verify(mock => mock.Update(It.IsAny<BusinessGroup>()), Times.Once());
        //}
    }
}