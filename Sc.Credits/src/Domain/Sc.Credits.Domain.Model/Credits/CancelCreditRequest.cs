namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The cancel credit request entity
    /// </summary>
    public class CancelCreditRequest : CancelCredit
    {
        /// <summary>
        /// Gets or sets the reason
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets the store's id
        /// </summary>
        public string StoreId { get; set; }

        /// <summary>
        /// Gets or sets the cancellation type
        /// </summary>
        public int CancellationType { get; set; }

        /// <summary>
        /// Gets or sets the value to cancel
        /// </summary>
        public decimal ValueCancel { get; set; }
    }
}