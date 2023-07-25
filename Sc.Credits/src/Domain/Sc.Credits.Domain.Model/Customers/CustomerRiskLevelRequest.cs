namespace Sc.Credits.Domain.Model.Customers
{
    public class CustomerRiskLevelRequest
    {
        /// <summary>
        /// Gets or sets the customer id
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the customer document's id
        /// </summary>
        public string CustomerDocumentId { get; set; }

        /// <summary>
        /// Gets or sets the credit value
        /// </summary>
        public decimal CreditValue { get; set; }

        /// <summary>
        /// Gets or sets the store's id
        /// </summary>
        public string StoreId { get; set; }

        /// <summary>
        /// Gets or sets the source
        /// </summary>
        public string Source { get; set; }
    }
}