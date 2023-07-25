using Sc.Credits.Domain.Model.Credits;

namespace Sc.Credits.Helper.Test.Model
{
    /// <summary>
    /// Payment Type Helper Test
    /// </summary>
    public static class PaymentTypeHelperTest
    {
        /// <summary>
        /// Get Payment Type
        /// </summary>
        /// <returns></returns>
        public static PaymentType GetOrdinaryPaymentType()
        {
            return new PaymentType(1, "Ordinary", "Ordinary");
        }

        /// <summary>
        /// Get Payment Type
        /// </summary>
        /// <returns></returns>
        public static PaymentType GetDownPaymentPaymentType()
        {
            return new PaymentType(9, "Down Payment", "Down Payment");
        }

        /// <summary>
        /// Get Payment Type
        /// </summary>
        /// <returns></returns>
        public static PaymentType GetCreditValueOnlyPaymentType()
        {
            return new PaymentType(2, "Credit Note", "Credit Value Only");
        }
    }
}