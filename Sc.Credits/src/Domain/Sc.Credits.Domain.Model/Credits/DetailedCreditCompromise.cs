using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.Model.Credits
{
    public class DetailedCreditCompromise 
    {


        /// <summary>
        /// Gets or sets the store's name
        /// </summary>
        public string StoreName { get; set; }


        /// <summary>
        /// Gets or sets the credit number
        /// </summary>
        public long CreditNumber { get; set; }


        /// <summary>
        /// Get or sets credits master Id
        /// </summary>
        public Guid CreditMasterId { get; set; }

        /// <summary>
        /// Gets or sets the credit's date of creation
        /// </summary>
        public DateTime CreateDate { get; set; }


        /// <summary>
        /// Gets or sets the arrears days
        /// </summary>
        public long ArrearsDays { get; set; }

        /// <summary>
        /// Gets or sets the arrears fees
        /// </summary>
        public int ArrearsFees { get; set; }

        /// <summary>
        /// Gets or sets the payment fees
        /// </summary>
        public int PaymentFees { get; set; }

        /// <summary>
        /// Gets or sets the  fees
        /// </summary>
        public int Fees { get; set; }



        /// <summary>
        /// Gets or sets the arrears payment
        /// </summary>
        public decimal ArrearsPayment { get; set; }


        /// <summary>
        /// Gets or sets the credit's value
        /// </summary>
        public decimal CreditValue { get; set; }

        /// <summary>
        /// Gets or sets the minimum payment
        /// </summary>
        public decimal MinimumPayment { get; set; }


        /// <summary>
        /// Gets or sets the charge value
        /// </summary>
        public decimal ChargeValue { get; set; }


        /// <summary>
        /// Gets or sets the total payment
        /// </summary>
        public decimal TotalPayment { get; set; }


        /// <summary>
        /// Gets or sets the updated payment plan value
        /// </summary>
        public bool UpdatedPaymentPlan { get; set; }



        public decimal UpdatedPaymentPlanValue { get; set; }
        






    }
}
