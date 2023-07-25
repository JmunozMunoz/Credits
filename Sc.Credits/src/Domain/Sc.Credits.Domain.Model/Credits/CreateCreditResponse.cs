using Newtonsoft.Json;
using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The create credit response entity
    /// </summary>
    public class CreateCreditResponse : CreditDetailResponse
    {
        /// <summary>
        /// Gets or sets the document's type
        /// </summary>
        public string TypeDocument { get; set; }

        /// <summary>
        /// Gets or sets the document's id
        /// </summary>
        public string IdDocument { get; set; }

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

        /// <summary>
        /// Gets or sets the store's id
        /// </summary>
        public string StoreId { get; set; }

        /// <summary>
        /// Gets or sets the alternate payment
        /// </summary>
        [JsonIgnore]
        public bool AlternatePayment { get; set; }

        /// <summary>
        /// Gets or sets the credit master
        /// </summary>
        [JsonIgnore]
        public CreditMaster CreditMaster { get; set; }

        /// <summary>
        /// Gets or sets the customer allow photo signature indicator
        /// </summary>
        [JsonIgnore]
        public new bool CustomerAllowPhotoSignature { get; set; }
    }
}