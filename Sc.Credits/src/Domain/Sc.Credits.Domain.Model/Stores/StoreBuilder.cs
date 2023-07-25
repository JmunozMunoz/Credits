using Sc.Credits.Helpers.ObjectsUtils;

namespace Sc.Credits.Domain.Model.Stores
{
    /// <summary>
    /// The store builder
    /// </summary>
    public class StoreBuilder
    {
        private readonly Store _store;

        /// <summary>
        /// Create store builder
        /// </summary>
        /// <returns></returns>
        public static StoreBuilder CreateBuilder()
        {
            return new StoreBuilder();
        }

        /// <summary>
        /// Creates a new instance of <see cref="StoreBuilder"/>
        /// </summary>
        private StoreBuilder()
        {
            _store = Store.New();
        }

        /// <summary>
        /// Init
        /// </summary>
        /// <param name="credinetAppSettings"></param>
        /// <param name="effectiveAnnualRate"></param>
        /// <returns></returns>
        public StoreBuilder Init(CredinetAppSettings credinetAppSettings, decimal effectiveAnnualRate)
        {
            _store.Init(credinetAppSettings, effectiveAnnualRate);
            return this;
        }

        /// <summary>
        /// Set store basic info
        /// </summary>
        /// <param name="id"></param>
        /// <param name="storeName"></param>
        /// <param name="phones"></param>
        /// <param name="status"></param>
        /// <param name="allowPromissoryNoteSignature"></param>
        /// <param name="storeProfileCode"></param>
        /// <returns></returns>
        public StoreBuilder BasicInfo(string id, string storeName, string[] phones, int status, bool allowPromissoryNoteSignature, int storeProfileCode)
        {
            _store.SetBasicInfo(id, storeName, phones, status, allowPromissoryNoteSignature, storeProfileCode);

            return this;
        }

        /// <summary>
        /// Set store vendor info
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="businessGroupId"></param>
        /// <param name="assuranceCompanyId"></param>
        /// <returns></returns>
        public StoreBuilder VendorInfo(string vendorId, string businessGroupId, long? assuranceCompanyId, string nit)
        {
            _store.SetVendorInfo(vendorId, businessGroupId, assuranceCompanyId, nit);

            return this;
        }

        /// <summary>
        /// Set store payment info
        /// </summary>
        /// <param name="collectTypeId"></param>
        /// <param name="paymentTypeId"></param>
        /// <param name="mandatoryDownPayment"></param>
        /// <param name="assuranceType"></param>
        /// <returns></returns>
        public StoreBuilder PaymentInfo(int collectTypeId, int paymentTypeId, int assuranceType, bool? mandatoryDownPayment)
        {
            _store.SetPaymentInfo(collectTypeId, paymentTypeId, assuranceType, mandatoryDownPayment);

            return this;
        }

        /// <summary>
        /// Set store calculation values
        /// </summary>
        /// <param name="minimumFee"></param>
        /// <param name="downPaymentPercentage"></param>
        /// <param name="monthLimit"></param>
        /// <param name="effectiveAnnualRate"></param>
        /// <param name="assurancePercentage"></param>
        /// <param name="storeCategoryId"></param>
        /// <returns></returns>
        public StoreBuilder CalculationValues(decimal minimumFee, decimal downPaymentPercentage, int monthLimit = 0,
            decimal effectiveAnnualRate = 0, decimal assurancePercentage = 0, int storeCategoryId = 0)
        {
            _store.SetCalculationValues(minimumFee, downPaymentPercentage, monthLimit, effectiveAnnualRate, assurancePercentage, storeCategoryId);

            return this;
        }

        /// <summary>
        /// Set location
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public StoreBuilder SetLocation(string stateId, string cityId)
        {
            _store.SetLocation(stateId, cityId);

            return this;
        }

        /// <summary>
        /// Build
        /// </summary>
        /// <returns></returns>
        public Store Build() => _store;
    }
}