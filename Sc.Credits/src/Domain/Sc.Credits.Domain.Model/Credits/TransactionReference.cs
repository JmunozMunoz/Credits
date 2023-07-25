using System;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The transaction reference entity
    /// </summary>
    public class TransactionReference
    {
        /// <summary>
        /// Gets or sets the transaction's id
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the Credit's Id
        /// </summary>
        public Guid CreditId { get; set; }

        /// <summary>
        /// Create a new entity of <see cref="TransactionReference"/>
        /// </summary>
        public TransactionReference()
        {
        }

        /// <summary>
        /// Create a new entity of <see cref="TransactionReference"/>
        /// </summary>
        public TransactionReference(string transactionId, Guid creditId)
        {
            TransactionId = transactionId;
            CreditId = creditId;
        }
    }
}