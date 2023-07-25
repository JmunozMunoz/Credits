using System;
using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The cancel payment detail response entity
    /// </summary>
    public class CancelPaymentDetailResponsePaged
    {
        /// <summary>
        /// Total records of Cancel Payment request
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// CreditDetailResponses list
        /// </summary>
        public List<CancelPaymentDetailResponse> PaymentDetailResponses { get; set; }
    }
}