namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The Payment fee entity
    /// </summary>
    public class PaymentFee
    {
        /// <summary>
        /// Gets or sets the number of fees to pay
        /// </summary>
        public int Fees { get; set; }

        /// <summary>
        /// Gets or sets the payment value of the fees
        /// </summary>
        public decimal Payment { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="PaymentFee"/>
        /// </summary>
        public PaymentFee() { }

        /// <summary>
        /// Creates a new instance of <see cref="PaymentFee"/>
        /// </summary>
        /// <param name="fees"></param>
        /// <param name="payment"></param>
        public PaymentFee(int fees, decimal payment)
        {
            Fees = fees;
            Payment = payment;
        }
    }
}