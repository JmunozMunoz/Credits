using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The payment fees response entity
    /// </summary>
    public class PaymentFeesResponse
    {
        /// <summary>
        /// Gets or sets the pending fees
        /// </summary>
        public int PendingFees { get; set; }

        /// <summary>
        /// Gets or sets the arrears fees
        /// </summary>
        public int ArrearsFees { get; set; }

        /// <summary>
        /// Gets or sets the arrears payment
        /// </summary>
        public decimal ArrearsPayment { get; set; }

        /// <summary>
        /// Gets or sets the payment fees
        /// </summary>
        public List<PaymentFee> PaymentFees { get; set; }
    }
}