using Sc.Credits.Domain.Model.Credits;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Model.Common.Gateway
{
    /// <summary>
    /// Token Repository contract
    /// </summary>
    public interface ITokenRepository
    {
        /// <summary>
        /// Is Valid Credit Token
        /// </summary>
        /// <param name="creditTokenRequest"></param>
        /// <returns></returns>
        Task<bool> IsValidCreditTokenAsync(CreditTokenRequest creditTokenRequest);

        /// <summary>
        /// Generate credit token
        /// </summary>
        /// <param name="creditTokenRequest"></param>
        /// <returns></returns>
        Task<TokenResponse> GenerateCreditTokenAsync(CreditTokenRequest creditTokenRequest);

        /// <summary>
        /// Credit token call request
        /// </summary>
        /// <param name="creditTokenCallRequest"></param>
        /// <returns></returns>
        Task CreditTokenCallRequestAsync(CreditTokenCallRequest creditTokenCallRequest);
    }
}