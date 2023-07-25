using Sc.Credits.Domain.Model.Credits;
using System.Collections.Generic;

namespace Sc.Credits.Helper.Test.Model
{
    /// <summary>
    /// Payment Fees Helper Test
    /// </summary>
    public static class PaymentFeesHelperTest
    {
        /// <summary>
        /// Get Payment Fees Response
        /// </summary>
        /// <returns></returns>
        public static PaymentFeesResponse GetPaymentFeesResponse()
        {
            List<PaymentFee> paymentFees = new List<PaymentFee>()
            {
                new PaymentFee
                {
                    Fees = 1,
                    Payment = 52430.24M
                },
                new PaymentFee
                {
                    Fees = 2,
                    Payment = 112430.24M
                },
                new PaymentFee
                {
                    Fees = 3,
                    Payment = 172430.24M,
                },
                new PaymentFee
                {
                    Fees = 4,
                    Payment = 216240.24M
                }
            };

            return new PaymentFeesResponse
            {
                PendingFees = 4,
                ArrearsFees = 2,
                PaymentFees = paymentFees
            };
        }
    }
}