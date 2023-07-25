using Sc.Credits.Domain.Model.Stores;
using Xunit;

namespace Sc.Credits.Domain.UseCase.Tests.Stores
{
    public class StoreModelsTest
    {
        [Fact]
        public void TestAssuranceCompany()
        {
            Assert.NotNull(new AssuranceCompany("Test"));
        }

        [Fact]
        public void TestBusinessGroup()
        {
            Assert.NotNull(new BusinessGroup("1", "Test"));
        }

        [Fact]
        public void TestCollectType()
        {
            Assert.NotNull(new CollectType("Test"));
        }
    }
}