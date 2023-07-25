namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// Credit payment event response
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CreditPaymentEventResponse<T>
    {
        /// <summary>
        /// Transaction id
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Error code
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Result
        /// </summary>
        public T Result { get; set; }

        /// <summary>
        /// Credit payment event response
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="errorCode"></param>
        /// <param name="message"></param>
        /// <param name="result"></param>
        public CreditPaymentEventResponse(string transactionId, int errorCode, string message, T result)
        {
            TransactionId = transactionId;
            ErrorCode = errorCode;
            Message = message;
            Result = result;
        }

        /// <summary>
        /// Build
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="errorCode"></param>
        /// <param name="message"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static CreditPaymentEventResponse<T> Build(string transactionId, int errorCode, string message, T result) =>
                    new CreditPaymentEventResponse<T>(transactionId, errorCode, message, result);

        /// <summary>
        /// Build successful response
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static CreditPaymentEventResponse<T> BuildSuccessfulResponse(string transactionId, T result) =>
                    Build(transactionId, 0, "", result);
    }
}