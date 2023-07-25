namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The create credit transaction response entity
    /// </summary>
    public class CreateCreditTransactionResponse
    {
        /// <summary>
        /// Create credit response
        /// </summary>
        public CreateCreditResponse CreateCreditResponse { get; }

        /// <summary>
        /// Create credit transaction
        /// </summary>
        public Credit CreateCreditTransaction { get; }

        /// <summary>
        /// Payment credit response
        /// </summary>
        public PaymentCreditResponse PaymentCreditResponse { get; }

        /// <summary>
        /// Has payment
        /// </summary>
        public bool HasPayment => PaymentCreditResponse != null;

        /// <summary>
        /// Creates a new instance of <see cref="CreateCreditTransactionResponse"/>
        /// </summary>
        /// <param name="createCreditResponse"></param>
        /// <param name="createCreditTransaction"></param>
        /// <param name="paymentCreditResponse"></param>
        public CreateCreditTransactionResponse(CreateCreditResponse createCreditResponse,
            Credit createCreditTransaction,
            PaymentCreditResponse paymentCreditResponse)
        {
            CreateCreditResponse = createCreditResponse;
            CreateCreditTransaction = createCreditTransaction;
            PaymentCreditResponse = paymentCreditResponse;
        }
    }
}