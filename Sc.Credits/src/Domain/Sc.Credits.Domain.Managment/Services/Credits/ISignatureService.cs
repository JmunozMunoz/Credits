using Sc.Credits.Domain.Model.Credits;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Credits
{
    /// <summary>
    /// Signature service contract
    /// </summary>
    public interface ISignatureService
    {
        /// <summary>
        /// Sign
        /// </summary>
        /// <param name="creditMaster"></param>
        /// <param name="fileName"></param>
        /// <param name="promisoryNoteFile"></param>
        /// <returns></returns>
        Task<(string Authority, string Id)> SignAsync(CreditMaster creditMaster, string fileName, byte[] promisoryNoteFile);
    }
}