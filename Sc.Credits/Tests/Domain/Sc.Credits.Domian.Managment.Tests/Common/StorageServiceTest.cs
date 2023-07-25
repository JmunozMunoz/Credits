using Moq;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Model.Common.Gateway;
using System.Threading.Tasks;
using Xunit;

namespace Sc.Credits.Domain.Managment.Tests.Common
{
    public class StorageServiceTest
    {
        private readonly Mock<IBlobStoreRepository> _blobStoreRepositoryMock = new Mock<IBlobStoreRepository>();

        private IStorageService StorageService => new StorageService(_blobStoreRepositoryMock.Object);

        [Fact]
        public async Task ShouldUploadFile()
        {
            _blobStoreRepositoryMock.Setup(mock => mock.UploadFileAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Verifiable();

            await StorageService.UploadFileAsync(new byte[] { 1, 2, 3 }, "TestPath", "TestFile.pdf", "TestContainerName");

            _blobStoreRepositoryMock.VerifyAll();
        }
    }
}