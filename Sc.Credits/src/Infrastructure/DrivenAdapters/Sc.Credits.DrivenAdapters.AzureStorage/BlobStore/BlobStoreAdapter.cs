using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Sc.Credits.Domain.Model.Common.Gateway;
using System.IO;
using System.Threading.Tasks;

namespace Sc.Credits.DrivenAdapters.AzureStorage.BlobStore
{
    /// <summary>
    /// <see cref="IBlobStoreRepository"/>
    /// </summary>
    public class BlobStoreAdapter : IBlobStoreRepository
    {
        private CloudBlobContainer _cloudBlobContainer;
        private readonly string _storageConnectionString;

        /// <summary>
        /// Blob store adapter
        /// </summary>
        /// <param name="storageConnectionString"></param>
        public BlobStoreAdapter(string storageConnectionString)
        {
            _storageConnectionString = storageConnectionString;
        }

        /// <summary>
        /// <see cref="IBlobStoreRepository.UploadFileAsync(byte[], string, string, string)"/>
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="blobContainerName"></param>
        /// <returns></returns>
        public async Task UploadFileAsync(byte[] file, string path, string fileName, string blobContainerName)
        {
            InitContainer(blobContainerName);

            var cloudBlockBlob = _cloudBlobContainer.GetBlockBlobReference($"{path}/{fileName}");

            await cloudBlockBlob.UploadFromByteArrayAsync(file, 0, file.Length);
        }

        /// <summary>
        /// <see cref="IBlobStoreRepository.DownloadFileAsync(string, string, string)"/>
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="path"></param>
        /// <param name="blobContainerName"></param>
        /// <returns></returns>
        public async Task<byte[]> DownloadFileAsync(string fileName, string path, string blobContainerName)
        {
            InitContainer(blobContainerName);

            var cloudBlockBlob = string.IsNullOrEmpty(path) ?
                _cloudBlobContainer.GetBlockBlobReference($"{fileName}") :
                _cloudBlobContainer.GetBlockBlobReference($"{path}/{fileName}");

            using (MemoryStream memoryStream = new MemoryStream())
            {
                await cloudBlockBlob.DownloadToStreamAsync(memoryStream);
                memoryStream.Position = 0;
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Init container
        /// </summary>
        /// <param name="blobContainerName"></param>
        private void InitContainer(string blobContainerName)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(_storageConnectionString);

            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

            _cloudBlobContainer = cloudBlobClient.GetContainerReference(blobContainerName);

            _cloudBlobContainer.CreateIfNotExists();

            var permissions = _cloudBlobContainer.GetPermissions();
            permissions.PublicAccess = BlobContainerPublicAccessType.Container;
            _cloudBlobContainer.SetPermissions(permissions);
        }
    }
}