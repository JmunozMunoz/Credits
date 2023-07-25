namespace Sc.Credits.Domain.Model.Credits.Builders
{
    /// <summary>
    /// Credit detail builder contract, extends <see cref="IBuilder{CreditMaster}"/>
    /// </summary>
    public interface ICreditDetailBuilder : IBuilder<CreditMaster>
    {
        /// <summary>
        /// Fees info
        /// </summary>
        /// <param name="fees"></param>
        /// <param name="frequency"></param>
        /// <param name="feeValue"></param>
        /// <returns></returns>
        ICreditDetailBuilder FeesInfo(int fees, int frequency, decimal feeValue);

        /// <summary>
        /// Additional info
        /// </summary>
        /// <param name="status"></param>
        /// <param name="authMethod"></param>
        /// <param name="downPayment"></param>
        /// <param name="totalDownPayment"></param>
        /// <returns></returns>
        ICreditPaymentBuilder AdditionalDetailInfo(Status status, AuthMethod authMethod, decimal downPayment = 0, decimal totalDownPayment = 0);
    }
}