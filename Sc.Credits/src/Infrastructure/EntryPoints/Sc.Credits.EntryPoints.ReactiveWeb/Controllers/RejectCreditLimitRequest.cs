namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// Reject Credit Limit Request
    /// </summary>
    public class RejectCreditLimitRequest
    {
        /// <summary>
        /// Id document
        /// </summary>
        public string IdDocument { get; set; }

        /// <summary>
        /// Document type
        /// </summary>
        public string DocumentType { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        public string UserId { get; set; }
    }
}