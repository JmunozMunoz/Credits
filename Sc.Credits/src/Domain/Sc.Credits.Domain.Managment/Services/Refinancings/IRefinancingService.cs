using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Refinancings;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Refinancings
{
    /// <summary>
    /// Refinancing service contract
    /// </summary>
    public interface IRefinancingService
    {
        /// <summary>
        /// Get customer credits
        /// </summary>
        /// <param name="customerCreditsRequest"></param>
        /// <returns></returns>
        Task<CustomerCreditsResponse> GetCustomerCreditsAsync(CustomerCreditsRequest customerCreditsRequest);

        /// <summary>
        /// Calculate fees
        /// </summary>
        /// <param name="calculateFeesRequest"></param>
        /// <returns></returns>
        Task<CalculateFeesResponse> CalculateFeesAsync(CalculateFeesRequest calculateFeesRequest);

        /// <summary>
        /// Create credit
        /// </summary>
        /// <param name="refinancingCreditRequest"></param>
        /// <returns></returns>
        Task<RefinancingCreditResponse> CreateCreditAsync(RefinancingCreditRequest refinancingCreditRequest);

        /// <summary>
        /// Generate token
        /// </summary>
        /// <param name="generateTokenRequest"></param>
        /// <returns></returns>
        Task<TokenResponse> GenerateTokenAsync(GenerateTokenRequest generateTokenRequest);
    }
}