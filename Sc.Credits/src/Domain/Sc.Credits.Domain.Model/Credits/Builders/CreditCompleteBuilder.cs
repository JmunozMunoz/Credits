using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Helpers.ObjectsUtils;
using System;

namespace Sc.Credits.Domain.Model.Credits.Builders
{
    /// <summary>
    /// Credit complete builder is an implementation of <see cref="ICreditCompleteBuilder"/>
    /// </summary>
    public class CreditCompleteBuilder : ICreditCompleteBuilder
    {
        protected CreditMaster _creditMaster;

        /// <summary>
        /// Credit master builder
        /// </summary>
        /// <param name="creditMaster"></param>
        protected CreditCompleteBuilder(CreditMaster creditMaster)
        {
            _creditMaster = creditMaster;
        }

        #region IBuilder Members

        /// <summary>
        /// <see cref="IBuilder{T}.Build"/>
        /// </summary>
        /// <returns></returns>
        public CreditMaster Build()
        {
            return _creditMaster;
        }

        #endregion IBuilder Members

        #region ICreditMasterBuilder Members

        /// <summary>
        /// <see cref="ICreditMasterBuilder.SellerInfo(string, string, string)"/>
        /// </summary>
        /// <param name="seller"></param>
        /// <param name="products"></param>
        /// <param name="invoice"></param>
        /// <returns></returns>
        public ICreditMasterBuilder SellerInfo(string seller, string products, string invoice)
        {
            _creditMaster.SetSellerInfo(seller, products, invoice);
            return this;
        }

        /// <summary>
        /// <see cref="ICreditMasterBuilder.CreditDate(DateTime)"/>
        /// </summary>
        /// <param name="creditDate"></param>
        /// <returns></returns>
        public ICreditMasterBuilder CreditDate(DateTime creditDate)
        {
            _creditMaster.SetCreditDate(creditDate);

            return this;
        }

        /// <summary>
        /// <see cref="ICreditMasterBuilder.AdditionalInfo(Store, string, string, string)"/>
        /// </summary>
        /// <param name="store"></param>
        /// <param name="token"></param>
        /// <param name="location"></param>
        /// <param name="riskLevel"></param
        /// <returns></returns>
        public ICreditDetailBuilder AdditionalInfo(Store store, string token, string location, string riskLevel)
        {
            _creditMaster.SetStore(store);
            _creditMaster.SetToken(token);
            _creditMaster.SetLocation(location);
            _creditMaster.SetRiskLevel(riskLevel);

            return this;
        }

        #endregion ICreditMasterBuilder Members

        #region ICreditDetailBuilder Members

        /// <summary>
        /// <see cref="ICreditDetailBuilder.FeesInfo(int, int, decimal)"/>
        /// </summary>
        /// <param name="fees"></param>
        /// <param name="frequency"></param>
        /// <param name="feeValue"></param>
        /// <returns></returns>
        public ICreditDetailBuilder FeesInfo(int fees, int frequency, decimal feeValue)
        {
            _creditMaster.SetFeesInfo(fees, frequency, feeValue);

            return this;
        }

        /// <summary>
        /// <see cref="ICreditDetailBuilder.AdditionalDetailInfo(Status, AuthMethod, decimal, decimal)"/>
        /// </summary>
        /// <param name="status"></param>
        /// <param name="authMethod"></param>
        /// <param name="downPayment"></param>
        /// <param name="totalDownPayment"></param>
        /// <returns></returns>
        public ICreditPaymentBuilder AdditionalDetailInfo(Status status, AuthMethod authMethod, decimal downPayment = 0, decimal totalDownPayment = 0)
        {
            _creditMaster.SetSeedMasters(status, authMethod);
            _creditMaster.SetDownPayment(downPayment, totalDownPayment);

            return this;
        }

        #endregion ICreditDetailBuilder Members

        #region ICreditPaymentBuilder Members

        /// <summary>
        /// <see cref="ICreditPaymentBuilder.InitialPayment(TransactionType, PaymentType, DateTime, DateTime)"/>
        /// </summary>
        /// <param name="transactionType"></param>
        /// <param name="paymentType"></param>
        /// <param name="dueDate"></param>
        /// <param name="calculationDate"></param>
        /// <returns></returns>
        public ICreditPaymentBuilder InitialPayment(TransactionType transactionType, PaymentType paymentType, DateTime dueDate, DateTime calculationDate)
        {
            _creditMaster.SetInitialPayment(transactionType, paymentType, dueDate, calculationDate);

            return this;
        }

        /// <summary>
        /// <see cref="ICreditPaymentBuilder.AssuranceValues(decimal, decimal, decimal, decimal, decimal)"/>
        /// </summary>
        /// <param name="assurancePercentage"></param>
        /// <param name="assuranceValue"></param>
        /// <param name="assuranceFee"></param>
        /// <param name="assuranceTotalFeeValue"></param>
        /// <param name="assuranceTotalValue"></param>
        public ICreditCompleteBuilder AssuranceValues(decimal assurancePercentage, decimal assuranceValue, decimal assuranceFee, decimal assuranceTotalFeeValue,
            decimal assuranceTotalValue)
        {
            _creditMaster.SetAssuranceValues(assurancePercentage, assuranceValue, assuranceFee, assuranceTotalFeeValue, assuranceTotalValue);

            return this;
        }

        #endregion ICreditPaymentBuilder Members

        #region ICreditCompleteBuilder Members

        /// <summary>
        /// Restart
        /// </summary>
        /// <param name="source"></param>
        /// <param name="customer"></param>
        /// <param name="creditValue"></param>
        /// <param name="creditNumber"></param>
        /// <param name="userInfo"></param>
        /// <param name="credinetAppSettings"></param>
        /// <returns></returns>
        public ICreditPostInitBuilder Restart(Source source, Customer customer, decimal creditValue, long creditNumber, UserInfo userInfo,
            CredinetAppSettings credinetAppSettings)
        {
            return CreditBuilder
                .CreateBuilder()
                .Init(source, customer, creditValue, creditNumber, userInfo, credinetAppSettings);
        }

        #endregion ICreditCompleteBuilder Members
    }
}