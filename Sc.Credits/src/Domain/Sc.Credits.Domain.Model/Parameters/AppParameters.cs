using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Sc.Credits.Domain.Model.Parameters
{
    /// <summary>
    /// App parameters
    /// </summary>
    public class AppParameters
    {
        private readonly List<Parameter> _parameters;

        /// <summary>
        /// Creates a new instance of <see cref="AppParameters"/>
        /// </summary>
        /// <param name="parameters"></param>
        public AppParameters(List<Parameter> parameters)
        {
            _parameters = parameters;
        }

        /// <summary>
        /// Assurance tax
        /// </summary>
        public decimal AssuranceTax => GetValue<decimal>("AssuranceTax");

        /// <summary>
        /// Decimal numbers round
        /// </summary>
        public int DecimalNumbersRound => GetValue<int>("DecimalNumbersRound");

        /// <summary>
        /// Partial credit limit
        /// </summary>
        public decimal PartialCreditLimit => GetValue<decimal>("PartialCreditLimit");

        /// <summary>
        /// Minimum credit value
        /// </summary>
        public decimal MinimumCreditValue => GetValue<decimal>("MinimumCreditValue");

        /// <summary>
        /// Arrears grace period
        /// </summary>
        public int ArrearsGracePeriod => GetValue<int>("ArrearsGracePeriod");

        /// <summary>
        /// Arrears effective annual rate
        /// </summary>
        public decimal ArrearsEffectiveAnnualRate => GetValue<decimal>("ArrearsEffectiveAnnualRate");

        /// <summary>
        /// Effective annual rate
        /// </summary>
        public decimal EffectiveAnnualRate => GetValue<decimal>("EffectiveAnnualRate");

        /// <summary>
        /// Maximum residue value
        /// </summary>
        public decimal MaximumResidueValue => GetValue<decimal>("MaximumResidueValue");

        /// <summary>
        /// Maximum days request cancellation payments
        /// </summary>
        public int MaximumDaysRequestCancellationPayments => GetValue<int>("MaximumDaysRequestCancellationPayments");

        /// <summary>
        /// Maximum payment adjustment residue
        /// </summary>
        public decimal MaximumPaymentAdjustmentResidue => GetValue<int>("MaximumPaymentAdjustmentResidue");

        /// <summary>
        /// Cell phone
        /// </summary>
        public string CellPhone => GetValue<string>("CellPhone");

        /// <summary>
        /// Nit
        /// </summary>
        public string Nit => GetValue<string>("Nit");

        /// <summary>
        /// Have assurance
        /// </summary>
        public bool HaveAssurance => GetValue<bool>("HaveAssurance");

        /// <summary>
        /// Have assurance tax
        /// </summary>
        public bool HaveAssuranceTax => GetValue<bool>("HaveAssuranceTax");

        /// <summary>
        /// Maximum months credit history
        /// </summary>
        public int MaximumMonthsCreditHistory => GetValue<int>("MaximumMonthsCreditHistory");

        /// <summary>
        /// Maximum months payment history
        /// </summary>
        public int MaximumMonthsPaymentHistory => GetValue<int>("MaximumMonthsPaymentHistory");

        /// <summary>
        /// Photo signature pay credit days
        /// </summary>
        public int PhothoSignaturePaidCreditDays => GetValue<int>("PhothoSignaturePaidCreditDays");

        /// <summary>
        /// Interest rate decimal numbers
        /// </summary>
        public int InterestRateDecimalNumbers => GetValue<int>("InterestRateDecimalNumbers");

        /// <summary>
        /// Arrears adjustment date
        /// </summary>
        public DateTime ArrearsAdjustmentDate => GetValue<DateTime>("ArrearsAdjustmentDate");

        /// <summary>
        /// Default risk level
        /// </summary>
        public string DefaultRiskLevel => GetValue<string>("DefaultRiskLevel");

        /// <summary>
        /// Gets the maximmum gap between total payment and updated payment on updated payment plan value rate basis
        /// </summary>
        public decimal MaximumUpdatedPaymentPlanGapRate => GetValue<decimal>("MaximumUpdatedPaymentPlanGapRate");

        /// <summary>
        ///
        /// </summary>
        public int DaysElapsedToRejectARequestCancelCredit => GetValue<int>("DaysElapsedToRejectARequestCancelCredit");

        /// <summary>
        ///
        /// </summary>
        public int DaysElapsedToRejectARequestCancelPayment => GetValue<int>("DaysElapsedToRejectARequestCancelPayment");


        /// <summary>
        /// Gets the save simulation register.
        /// </summary>
        /// <value>
        /// The save simulation register.
        /// </value>
        public bool SaveSimulationRecord => GetValue<bool>("SaveSimulationRecord");

        /// <summary>
        /// Government arrears effective annual rate
        /// </summary>
        public decimal GovernmentAnnualEffectiveRate => GetValue<decimal>("GovernmentAnnualEffectiveRate");

        /// <summary>
        /// Maximum credit value according to store profile
        /// </summary>
        public decimal MaximumCreditValueAccordingToStoreProfile => GetValue<decimal>("MaximumCreditValueAccordingToStoreProfile");

        /// <summary>
        /// Store profiles
        /// </summary>
        public List<int> StoreProfiles => GetValue<string>("StoreProfiles").Split(',').Select(profile=> int.Parse(profile)).ToList();

        /// <summary>
        /// Sources With Risk Level Calculation
        /// </summary>
        public List<int> SourcesWithRiskLevelCalculation => GetValue<string>("SourcesWithRiskLevelCalculation").Split(',').Select(profile => int.Parse(profile)).ToList();

        /// <summary>
        /// Credit days paid according to store profile
        /// </summary>
        public int CreditDaysPaidAccordingToStoreProfile => GetValue<int>("CreditDaysPaidAccordingToStoreProfile");

        /// <summary>
        /// Credit days paid according to store profile
        /// </summary>
        public string VirtualSalesTokenSources => GetValue<string>("VirtualSalesTokenSources");

        /// <summary>
        /// Get value by key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        private T GetValue<T>(string key)
        {
            string value = _parameters.SingleOrDefault(p => p.GetKey.Trim().ToLower() == key.Trim().ToLower())?.GetValue;
            return string.IsNullOrEmpty(value) ? default : (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(value);
        }
    }
}