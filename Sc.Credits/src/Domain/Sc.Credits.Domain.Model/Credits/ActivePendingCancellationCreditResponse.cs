using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The active and pending cancellation credit response entity
    /// </summary>
    public class ActivePendingCancellationCreditResponse
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
        /// Map into field Id in MasterCredits entity and creditMasterId in Credits entity.
        /// </summary>
        public Guid CreditId { get; set; }

        /// <summary>
        /// Gets or sets the credit number
        /// </summary>
        public long CreditNumber { get; set; }

        /// <summary>
        /// Gets or sets the store's id
        /// </summary>
        public string StoreId { get; set; }

        /// <summary>
        /// Gets or sets the store's name
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// Gets or sets the create date
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the credit value
        /// </summary>
        public decimal CreditValue { get; set; }

        /// <summary>
        /// Gets or sets the status' id
        /// </summary>
        public int StatusId { get; set; }

        /// <summary>
        /// Gets or sets the status' name
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// Gets or sets the has payments verification
        /// </summary>
        public bool HasPayments { get; set; }

        /// <summary>
        /// Gets or sets the balance
        /// </summary>
        public decimal Balance { get; set; }
    }
}