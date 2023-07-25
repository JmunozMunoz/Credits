using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Common
{
    /// <summary>
    /// Template service contract
    /// </summary>
    public interface ITemplatesService
    {
        /// <summary>
        /// Get template
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        Task<string> GetAsync(string templateName);
    }
}