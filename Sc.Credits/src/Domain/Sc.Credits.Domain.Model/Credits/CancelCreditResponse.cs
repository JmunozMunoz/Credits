using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The cancel credit response entity
    /// </summary>
    public class CancelCreditResponse : CancelCredit
    {
        /// <summary>
        /// Gets or sets the request cancel credit's id
        /// </summary>
        public Guid IdRequestCancelCredit { get; set; }

        /// <summary>
        /// Gets or sets the document's id
        /// </summary>
        public string IdDocument { get; set; }

        /// <summary>
        /// Gets or sets the document's type
        /// </summary>
        public string TypeDocument { get; set; }

        /// <summary>
        /// Gets or sets the full name
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the credit number
        /// </summary>
        public long CreditNumber { get; set; }

        /// <summary>
        /// Gets or sets the store's id
        /// </summary>
        public string StoreId { get; set; }

        /// <summary>
        /// Gets or sets the cancellation type
        /// </summary>
        public string CancellationType { get; set; }

        /// <summary>
        /// Gets or sets the value to cancel
        /// </summary>
        public decimal? ValueCancel { get; set; }
    }
}