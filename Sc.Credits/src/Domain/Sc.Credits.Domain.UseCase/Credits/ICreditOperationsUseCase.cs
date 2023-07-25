using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Stores;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.UseCase.Credits
{
    public interface ICreditOperationsUseCase
    {
        /// <summary>
        /// Creates the credit details.
        /// </summary>
        /// <param name="generalCreditDetailDomainRequest">The simulated domain request.</param>
        /// <returns></returns>
        CreditDetailResponse CreateCreditDetails(GeneralCreditDetailDomainRequest generalCreditDetailDomainRequest);

        /// <summary>
        /// Get Interest Rate
        /// </summary>
        /// <param name="effectiveAnnualRate"></param>
        /// <param name="frequency"></param>
        /// <returns></returns>
        decimal GetInterestRate(decimal effectiveAnnualRate, int frequency);

        /// <summary>
        /// Calculates the assurance value.
        /// </summary>
        /// <param name="creditValue">The credit value.</param>
        /// <param name="assurancePercentage">The assurance percentage.</param>
        /// <returns></returns>
        decimal CalculateAssuranceValue(decimal creditValue, decimal assurancePercentage);

    }
}
