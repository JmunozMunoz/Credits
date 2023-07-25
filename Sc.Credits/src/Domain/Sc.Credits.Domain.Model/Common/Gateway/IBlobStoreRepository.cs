using System.Threading.Tasks;

namespace Sc.Credits.Domain.Model.Common.Gateway
{
    /// <summary>
    /// Blob store repository contract
    /// </summary>
    public interface IBlobStoreRepository
    {
        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="blobContainerName"></param>
        /// <returns></returns>
        Task UploadFileAsync(byte[] file, string path, string fileName, string blobContainerName);

        /// <summary>
        /// Download file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="path"></param>
        /// <param name="blobContainerName"></param>
        /// <returns></returns>
        Task<byte[]> DownloadFileAsync(string fileName, string path, string blobContainerName);
    }
}