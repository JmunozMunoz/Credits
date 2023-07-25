using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Enums;

namespace Sc.Credits.Helper.Test.Model
{
    /// <summary>
    /// Source Helper Test
    /// </summary>
    public static class SourceHelperTest
    {
        /// <summary>
        /// Get Credinet Source
        /// </summary>
        /// <returns></returns>
        public static Source GetCredinetSource()
        {
            return new Source("Credinet").SetId((int)Sources.CrediNet);
        }

        /// <summary>
        /// Get Credinet Source
        /// </summary>
        /// <returns></returns>
        public static Source GetCredinetSourcePaymentGateways()
        {
            return new Source("Payment Gateways").SetId((int)Sources.PaymentGateways);
        }

        /// <summary>
        /// Get Refinancing Source
        /// </summary>
        /// <returns></returns>
        public static Source GetRefinancingSource()
        {
            return new Source("Refinancing").SetId((int)Sources.Refinancing);
        }
    }
}