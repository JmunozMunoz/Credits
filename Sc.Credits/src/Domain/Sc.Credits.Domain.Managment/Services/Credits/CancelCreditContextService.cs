using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sc.Credits.Domain.Managment.Services.Credits
{
    /// <summary>
    /// Cancel credit context servicee is an implementation of <see cref="ICancelCreditContextService"/>
    /// </summary>
    public class CancelCreditContextService : ICancelCreditContextService
    {
        private ICancellationStrategy _cancellationStrategy;
        private readonly IPartialCancelationCreditService _partialCancelationCreditService;
        private readonly ITotalCancelationCreditService _totalCancelationCreditService;

        /// <summary>
        /// Creates a new instance of <see cref="CancelCreditContextService"/>
        /// </summary>
        /// <param name="partialCancelationCreditService"></param>
        /// <param name="totalCancelationCreditService"></param>
        public CancelCreditContextService(IPartialCancelationCreditService partialCancelationCreditService,
            ITotalCancelationCreditService totalCancelationCreditService)
        {
            _partialCancelationCreditService = partialCancelationCreditService;
            _totalCancelationCreditService = totalCancelationCreditService;
        }

        /// <summary>
        /// Build types of cancellations
        /// </summary>
        /// <param name="cancellationKey"></param>
        private void BuildTypesOfCancellations(int cancellationKey)
        {
            Dictionary<int, ICancellationStrategy> dictionayStrategy = new Dictionary<int, ICancellationStrategy>
            {
                { (int)CancellationTypes.Parcial, _partialCancelationCreditService },
                { (int)CancellationTypes.Total, _totalCancelationCreditService }
            };

            _cancellationStrategy = dictionayStrategy[cancellationKey];
        }

        /// <summary>
        /// <see cref="ICancelCreditContextService.ApproveCancellationAsync(UserInfo, RequestCancelCredit, CreditMaster)"/>
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="requestCancelCredit"></param>
        /// <param name="creditMaster"></param>
        /// <returns></returns>
        public async Task ApproveCancellationAsync(UserInfo userInfo, RequestCancelCredit requestCancelCredit, CreditMaster creditMaster)
        {
            BuildTypesOfCancellations(requestCancelCredit.GetCancellationType);
            await _cancellationStrategy.ApproveCancellationAsync(userInfo, requestCancelCredit, creditMaster);
        }

        /// <summary>
        /// <see cref="ICancelCreditContextService.ValidateCancellationRequest(CancelCreditRequest, CreditMaster)"/>
        /// </summary>
        /// <param name="cancelCreditRequest"></param>
        /// <param name="creditMaster"></param>
        /// <returns></returns>
        public async Task<decimal> ValidateCancellationRequest(CancelCreditRequest cancelCreditRequest, CreditMaster creditMaster)
        {
            BuildTypesOfCancellations(cancelCreditRequest.CancellationType);
            return await _cancellationStrategy.ValidateCancellationRequest(cancelCreditRequest, creditMaster);
        }
    }
}