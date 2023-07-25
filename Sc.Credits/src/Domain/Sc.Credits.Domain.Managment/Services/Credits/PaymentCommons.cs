using Sc.Credits.Domain.UseCase.Credits;

namespace Sc.Credits.Domain.Managment.Services.Credits
{
    /// <summary>
    /// Payment commons
    /// </summary>
    public class PaymentCommons
    {
        /// <summary>
        /// <see cref="ICreditPaymentUsesCase"/>
        /// </summary>
        internal ICreditPaymentUsesCase CreditPaymentUsesCase { get; }

        /// <summary>
        /// Creates a new instance of <see cref="PaymentCommons"/>
        /// </summary>
        /// <param name="creditPaymentUsesCase"></param>
        public PaymentCommons(ICreditPaymentUsesCase creditPaymentUsesCase)
        {
            CreditPaymentUsesCase = creditPaymentUsesCase;
        }
    }
}