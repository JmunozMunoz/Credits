using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Credits
{
    /// <summary>
    /// Partial cancelation credit service contract
    /// </summary>
    public interface IPartialCancelationCreditService : ICancellationStrategy
    {
        /// <summary>
        /// Get validation to partially cancel a credit
        /// </summary>
        /// <returns></returns>
        Task<bool> GetValidationToPartiallyCancelACreditAsync(PartialCancellationRequest partialCancellationRequest);
    }
}