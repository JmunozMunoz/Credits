using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The current amortization schedule fee
    /// </summary>
    public class CurrentAmortizationScheduleFee : AmortizationScheduleFee
    {
        /// <summary>
        /// Gets or sets the fee status
        /// </summary>
        public string FeeStatus { get; set; }

        /// <summary>
        /// Gets or sets the arrear payment value
        /// </summary>
        public decimal ArrearPaymentValue { get; set; }

        /// <summary>
        /// Gets or sets the interest days
        /// </summary>
        public int InterestDays { get; set; }

        /// <summary>
        /// Gets or sets the arrear days
        /// </summary>
        public int ArrearDays { get; set; }

        /// <summary>
        /// Gets or sets the charge value
        /// </summary>
        public decimal ChargeValue { get; set; }

        /// <summary>
        /// Paid fee
        /// </summary>
        /// <param name="feeNumber"></param>
        /// <param name="feeDate"></param>
        /// <returns></returns>
        public static CurrentAmortizationScheduleFee PaidFee(int feeNumber, DateTime feeDate)
            => new CurrentAmortizationScheduleFee
            {
                Fee = feeNumber,
                FeeDate = feeDate,
                FeeStatus = FeeStatuses.PAID,
                Balance = 0M,
                InterestDays = 0,
                InterestValue = 0M,
                CreditValuePayment = 0M,
                FinalBalance = 0M,
                ArrearDays = 0,
                ArrearPaymentValue = 0M,
                ChargeValue = 0,
                FeeValue = 0
            };
    }
}