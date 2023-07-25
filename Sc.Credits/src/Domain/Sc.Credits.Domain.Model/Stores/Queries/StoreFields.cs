using Sc.Credits.Domain.Model.Queries;
using System.Reflection;

namespace Sc.Credits.Domain.Model.Stores.Queries
{
    /// <summary>
    /// Store fields
    /// </summary>
    public class StoreFields
        : EntityFields
    {
        /// <summary>
        /// New store fields
        /// </summary>
        public StoreFields(string alias)
            : base(alias)
        {
        }

        /// <summary>
        /// Gets the store's name field
        /// </summary>
        public Field StoreName => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the collect type's id field
        /// </summary>
        public Field CollectTypeId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the assurance type field
        /// </summary>
        public Field AssuranceType => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the business group id field
        /// </summary>
        public Field BusinessGroupId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the down payment percentage field
        /// </summary>
        public Field DownPaymentPercentage => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the mandatory down payment field
        /// </summary>
        public Field MandatoryDownPayment => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the minimum fee field
        /// </summary>
        public Field MinimumFee => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the vendor's id field
        /// </summary>
        public Field VendorId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the assurance company id field
        /// </summary>
        public Field AssuranceCompanyId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the payment type id field
        /// </summary>
        public Field PaymentTypeId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the month limit field
        /// </summary>
        public Field MonthLimit => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the assurance percentage field
        /// </summary>
        public Field AssurancePercentage => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the effective annual rate field
        /// </summary>
        public Field EffectiveAnnualRate => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the phone field
        /// </summary>
        public Field Phone => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the status field
        /// </summary>
        public Field Status => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the city's id field
        /// </summary>
        public Field CityId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the state's id field
        /// </summary>
        public Field StateId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the allow promissory note signature indicator field
        /// </summary>
        public Field AllowPromissoryNoteSignature => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the store category id field
        /// </summary>
        public Field StoreCategoryId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the send token mail
        /// </summary>
        public Field SendTokenMail => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the send token mail
        /// </summary>
        public Field SendTokenSms => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the nit store
        /// </summary>
        public Field Nit => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the store profile code
        /// </summary>
        public Field StoreProfileCode => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the Store Risk Level
        /// </summary>
        public Field HasRiskCalculation => GetField(MethodBase.GetCurrentMethod().Name);
    }
}