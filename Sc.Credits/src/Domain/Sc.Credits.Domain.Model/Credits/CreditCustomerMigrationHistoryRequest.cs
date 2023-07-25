namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The credit customer migration history request
    /// </summary>
    public class CreditCustomerMigrationHistoryRequest
    {
        /// <summary>
        /// Gets or sets the document's id
        /// </summary>
        public string IdDocument { get; set; }

        /// <summary>
        /// Gets or sets the document's type
        /// </summary>
        public string DocumentType { get; set; }
    }
}