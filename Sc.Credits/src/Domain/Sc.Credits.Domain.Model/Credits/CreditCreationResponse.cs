using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.Model.Credits
{
    public class CreditCreationResponse
    {
        /// <summary>
        /// Gets or sets the credit's id
        /// </summary>
        public Guid CreditId { get; set; }

        /// <summary>
        /// Gets or sets the credit's number
        /// </summary>
        public long CreditNumber { get; set; }

        /// <summary>
        /// Gets or sets the effective annual rate
        /// </summary>
        public decimal EffectiveAnnualRate { get; set; }

        /// <summary>
        /// Gets or sets the down payment id
        /// </summary>
        public Guid? DownPaymentId { get; set; }
    }
}
