namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// Active credits request
    /// </summary>
    public class ActiveCreditsRequest
    {
        /// <summary>
        /// Transaction id
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Type document
        /// </summary>
        public string TypeDocument { get; set; }

        /// <summary>
        /// Id document
        /// </summary>
        public string IdDocument { get; set; }

        /// <summary>
        /// Store id
        /// </summary>
        public string StoreId { get; set; }
    }
}