using Sc.Credits.Domain.Model.Common.Gateway;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Common
{
    /// <summary>
    /// Storage service is an implementation of <see cref="IStorageService"/>
    /// </summary>
    public class StorageService : IStorageService
    {
        private readonly IBlobStoreRepository _blobStoreRepository;

        /// <summary>
        /// Creates a new instance of <see cref="StorageService"/>
        /// </summary>
        /// <param name="blobStoreRepository"></param>
        public StorageService(IBlobStoreRepository blobStoreRepository)
        {
            _blobStoreRepository = blobStoreRepository;
        }

        /// <summary>
        /// <see cref="IStorageService.UploadFileAsync(byte[], string, string, string)"/>
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public Task UploadFileAsync(byte[] bytes, string path, string fileName, string containerName) =>
            _blobStoreRepository.UploadFileAsync(bytes, path, fileName, containerName);
    }
}