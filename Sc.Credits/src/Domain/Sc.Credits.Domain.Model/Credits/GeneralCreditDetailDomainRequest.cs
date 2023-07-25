using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Parameters;
using Sc.Credits.Domain.Model.Stores;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.Model.Credits
{
    public class GeneralCreditDetailDomainRequest
    {

        /// <summary>
        /// Store
        /// </summary>
        public Store Store { get; private set; }

        /// <summary>
        /// Credit value
        /// </summary>
        public decimal CreditValue { get; private set; }

        /// <summary>
        /// Frequency
        /// </summary>
        public Frequencies Frequency { get; private set; }

        /// <summary>
        /// Fees
        /// </summary>
        public int Fees { get; private set; }

        /// <summary>
        /// downPayment
        /// </summary>
        public decimal DownPayment { get; private set; }

        /// <summary>
        /// App parameters
        /// </summary>
        public AppParameters AppParameters { get; private set; }

        /// <summary>
        /// Decimal numbers round
        /// </summary>
        public int DecimalNumbersRound => AppParameters.DecimalNumbersRound;

        /// <summary>
        /// Interest rate decimal numbers round
        /// </summary>
        public int InterestRateDecimalNumbers => AppParameters.InterestRateDecimalNumbers;

        /// <summary>
        /// Partial credit limit
        /// </summary>
        public decimal PartialCreditLimit => AppParameters.PartialCreditLimit;

        /// <summary>
        /// Assurance tax
        /// </summary>
        public decimal AssuranceTax => !HaveAssuranceTax ? 0 : AppParameters.AssuranceTax;

        /// <summary>
        /// Have assurance
        /// </summary>
        public bool HaveAssurance => Store.GetAssurancePercentage != 0 && AppParameters.HaveAssurance;

        /// <summary>
        /// Have assurance tax
        /// </summary>
        public bool HaveAssuranceTax => Store.GetAssurancePercentage != 0 && AppParameters.HaveAssuranceTax;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralCreditDetailDomainRequest"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="creditValue">The credit value.</param>
        /// <param name="frequency">The frequency.</param>
        /// <param name="appParameters">The application parameters.</param>
        public GeneralCreditDetailDomainRequest(Store store, decimal creditValue, int frequency,
            AppParameters appParameters)
        {
            Store = store;
            CreditValue = creditValue;
            Frequency = (Frequencies)frequency;
            AppParameters = appParameters;
        }

        /// <summary>
        /// Set fees by months
        /// </summary>
        /// <param name="months"></param>
        /// <returns></returns>
        public void SetFeesByMonths(int months)
        {
            Fees = GetFees(months);
        }

        /// <summary>
        /// Setdowns the payment.
        /// </summary>
        /// <param name="downPayment">Down payment.</param>
        /// <returns></returns>
        public void SetdownPayment(decimal downPayment)
        {
            DownPayment = downPayment;
        }

        /// <summary>
        /// Set fees
        /// </summary>
        /// <param name="fees"></param>
        /// <returns></returns>
        public void SetFees(int fees)
        {
            Fees = fees;
        }

        /// <summary>
        /// Get fees
        /// </summary>
        /// <returns></returns>
        public int GetFees(int months)
        {
            int fees = 0;

            switch (Frequency)
            {
                case Frequencies.Biweekly:
                case Frequencies.ScBiweekly:
                    fees = months * 2;
                    break;

                case Frequencies.Monthly:
                    fees = months;
                    break;
            }

            return fees;
        }

        /// <summary>
        /// Get months
        /// </summary>
        /// <returns></returns>
        public int GetMonths()
        {
            int months = 0;

            switch (Frequency)
            {
                case Frequencies.Biweekly:
                case Frequencies.ScBiweekly:
                    months = Fees / 2;
                    break;

                case Frequencies.Monthly:
                    months = Fees;
                    break;
            }

            return months;
        }
    }
}
