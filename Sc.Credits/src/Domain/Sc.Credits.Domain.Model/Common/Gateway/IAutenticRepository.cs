using System.Threading.Tasks;

namespace Sc.Credits.Domain.Model.Common.Gateway
{
    /// <summary>
    /// Autentic repository contract
    /// </summary>
    public interface IAutenticRepository
    {
        /// <summary>
        /// OAuth Autentic
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="parameters"></param>
        /// <param name="traceId"></param>
        /// <returns></returns>
        Task<OAuthResponseAutentic> OAuthAutenticAsync(string endPoint, OAuthRequestAutentic parameters, string traceId);

        /// <summary>
        /// Get signature Pdf
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="parameters"></param>
        /// <param name="tokenType"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<(byte[] File, string Id)> GetSignaturePdfAsync(string endPoint, RequestAutentic parameters, string tokenType, string token, string traceId);
    }
}