namespace Sc.Credits.Domain.Model.Credits.Builders
{
    /// <summary>
    /// Credit post init builder contract
    /// </summary>
    public interface ICreditPostInitBuilder
    {
        /// <summary>
        /// Set rates
        /// </summary>
        /// <param name="interestRate"></param>
        /// <param name="effectiveAnnualRate"></param>
        /// <returns></returns>
        ICreditMasterBuilder Rates(decimal interestRate, decimal effectiveAnnualRate);
    }
}