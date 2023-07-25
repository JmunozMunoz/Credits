using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The cancel payment request entity
    /// </summary>
    public class CancelPaymentRequest
    {
        /// <summary>
        /// Gets or sets the payment id
        /// </summary>
        public Guid PaymentId { get; set; }

        /// <summary>
        /// Gets or sets the user id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the user's name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the store's id
        /// </summary>
        public string StoreId { get; set; }

        /// <summary>
        /// Gets or sets the reason
        /// </summary>
        public string Reason { get; set; }
    }
}