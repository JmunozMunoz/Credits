using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Common
{
    /// <summary>
    /// Storage service contract
    /// </summary>
    public interface IStorageService
    {
        /// <summary>
        /// Upload file to storage
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="containerName"></param>
        /// <returns></returns>
        Task UploadFileAsync(byte[] bytes, string path, string fileName, string containerName);
    }
}