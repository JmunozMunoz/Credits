using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.Model.Refinancings;
using Sc.Credits.Domain.Model.Stores;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace Sc.Credits.Domain.UseCase.Refinancings
{
    /// <summary>
    /// Refinancing uses case contract
    /// </summary>
    public interface IRefinancingUsesCase
    {
        /// <summary>
        /// Customer credits
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="activeCredits"></param>
        /// <param name="application"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        CustomerCreditsResponse CustomerCredits(Customer customer, List<CreditMaster> activeCredits,
            RefinancingApplication application, AppParameters parameters);

        /// <summary>
        /// Fees response
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <param name="refinancingCreditMasters"></param>
        /// <param name="parameters"></param>
        /// <param name="application"></param>
        /// <returns></returns>
        CalculateFeesResponse FeesResponse(Customer customer, Store store, List<CreditMaster> refinancingCreditMasters,
            AppParameters parameters, RefinancingApplication application);

        /// <summary>
        /// Refinance
        /// </summary>
        /// <param name="refinancingDomainRequest"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<RefinancingCreditResponse> RefinanceAsync(RefinancingDomainRequest refinancingDomainRequest, Transaction transaction, bool setCreditLimit = true);

        /// <summary>
        /// Create log
        /// </summary>
        /// <param name="refinancingDomainRequest"></param>
        /// <param name="refinancingCreditResponse"></param>
        /// <returns></returns>
        RefinancingLog CreateLog(RefinancingDomainRequest refinancingDomainRequest, RefinancingCreditResponse refinancingCreditResponse);
    }
}