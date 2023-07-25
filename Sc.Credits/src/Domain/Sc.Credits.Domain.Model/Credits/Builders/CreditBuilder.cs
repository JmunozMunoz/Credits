using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Helpers.ObjectsUtils;

namespace Sc.Credits.Domain.Model.Credits.Builders
{
    /// <summary>
    /// Credit builder is an implementation of <see cref="ICreditBuilder"/>
    /// </summary>
    public class CreditBuilder : CreditCompleteBuilder, ICreditBuilder
    {
        /// <summary>
        /// Create builder
        /// </summary>
        /// <returns></returns>
        public static ICreditInitBuilder CreateBuilder()
        {
            return new CreditBuilder(CreditMaster.New());
        }

        /// <summary>
        /// New credit builder
        /// </summary>
        protected CreditBuilder(CreditMaster creditMaster)
            : base(creditMaster)
        {
        }

        #region ICreditInitBuilder Members

        /// <summary>
        /// Init
        /// </summary>
        /// <param name="source"></param>
        /// <param name="customer"></param>
        /// <param name="creditValue"></param>
        /// <param name="creditNumber"></param>
        /// <param name="userInfo"></param>
        /// <param name="credinetAppSettings"></param>
        /// <param name="setCreditLimit"></param>
        /// <returns></returns>
        public ICreditPostInitBuilder Init(Source source, Customer customer, decimal creditValue, long creditNumber, UserInfo userInfo,
            CredinetAppSettings credinetAppSettings, bool setCreditLimit = true)
        {
            _creditMaster = new CreditMaster(source, customer, creditValue, creditNumber, userInfo, credinetAppSettings, setCreditLimit);

            return this;
        }

        #endregion ICreditInitBuilder Members

        #region ICreditPostInitBuilder Members

        /// <summary>
        /// <see cref="ICreditPostInitBuilder.Rates(decimal, decimal)"/>
        /// </summary>
        /// <param name="interestRate"></param>
        /// <param name="effectiveAnnualRate"></param>
        /// <returns></returns>
        public ICreditMasterBuilder Rates(decimal interestRate, decimal effectiveAnnualRate)
        {
            _creditMaster.SetRates(interestRate, effectiveAnnualRate);

            return this;
        }

        #endregion ICreditPostInitBuilder Members
    }
}