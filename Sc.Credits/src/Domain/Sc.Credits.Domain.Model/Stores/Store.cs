using Sc.Credits.Domain.Model.Base;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Enums;
using Sc.Credits.Domain.Model.Locations;
using Sc.Credits.Helpers.Commons.Dapper.Annotations;
using Sc.Credits.Helpers.ObjectsUtils;
using System.Linq;

namespace Sc.Credits.Domain.Model.Stores
{
    /// <summary>
    /// Store class
    /// </summary>
    public class Store :
        Entity<string>, IAggregateRoot
    {
        private decimal _minimumFee;
        private decimal _downPaymentPercentage;
        private int _assuranceType;
        private bool _mandatoryDownPayment;
        private string _vendorId;
        private int _collectTypeId;
        private string _businessGroupId;
        private long _assuranceCompanyId;
        private int _paymentTypeId;
        private int _monthLimit;
        private decimal _effectiveAnnualRate;
        private decimal _assurancePercentage;
        private string _phone;
        private int _status;
        private bool _allowPromissoryNoteSignature;
        private string _stateId;
        private string _cityId;
        private int _storeCategoryId;
        private int _storeProfileCode;
        private bool _sendTokenMail;
        private bool _sendTokenSms;
        private string _nit;
        private bool _hasRiskCalculation;

        /// <summary>
        /// Gets the store's name
        /// </summary>
        public string StoreName { get; private set; }

        /// <summary>
        /// Gets the allow promissory note signature indicator
        /// </summary>
        [Write(false)]
        public bool GetAllowPromissoryNoteSignature => _allowPromissoryNoteSignature;

        /// <summary>
        /// Gets the down payment percentage
        /// </summary>
        [Write(false)]
        public decimal GetDownPaymentPercentage => _downPaymentPercentage;

        /// <summary>
        /// Gets the assurance type
        /// </summary>
        [Write(false)]
        public int GetAssuranceType => _assuranceType;

        /// <summary>
        /// Gets the mandatory down payment indicator
        /// </summary>
        [Write(false)]
        public bool GetMandatoryDownPayment => _mandatoryDownPayment;

        /// <summary>
        /// Gets the vendor id
        /// </summary>
        [Write(false)]
        public string GetVendorId => _vendorId;

        /// <summary>
        /// Gets the business group id
        /// </summary>
        [Write(false)]
        public string GetBusinessGroupId => _businessGroupId;

        /// <summary>
        /// Gets the assurance company id
        /// </summary>
        [Write(false)]
        public long GetAssuranceCompanyId => _assuranceCompanyId;

        /// <summary>
        /// Gets the collect type id
        /// </summary>
        [Write(false)]
        public int GetCollectTypeId => _collectTypeId;

        /// <summary>
        /// Gets the effective annual rate
        /// </summary>
        [Write(false)]
        public decimal GetEffectiveAnnualRate => _effectiveAnnualRate;

        /// <summary>
        /// Gets the assurance percentage
        /// </summary>
        [Write(false)]
        public decimal GetAssurancePercentage => _assurancePercentage;

        /// <summary>
        /// Gets the phone
        /// </summary>
        [Write(false)]
        public string GetPhone => _phone;

        /// <summary>
        /// Gets the status
        /// </summary>
        [Write(false)]
        public int GetStatus => _status;

        /// <summary>
        /// Gets the state's id
        /// </summary>
        [Write(false)]
        public string GetStateId => _stateId;

        /// <summary>
        /// Gets the city id
        /// </summary>
        [Write(false)]
        public string GetCityId => _cityId;

        /// <summary>
        /// Gets the payment type id
        /// </summary>
        [Write(false)]
        public int GetPaymentTypeId => _paymentTypeId;

        /// <summary>
        /// Gets the stores category id
        /// </summary>
        [Write(false)]
        public int GetStoreCategoryId => _storeCategoryId;

        /// <summary>
        /// Gets the send token mail indicator.
        /// </summary>
        [Write(false)]
        public bool GetSendTokenMail => _sendTokenMail;

        /// <summary>
        /// Gets the send token sms indicator.
        /// </summary>
        [Write(false)]
        public bool GetSendTokenSms => _sendTokenSms;

        /// <summary>
        /// Gets the send token sms indicator.
        /// </summary>
        [Write(false)]
        public int GetStoreProfileCode => _storeProfileCode;

        /// <summary>
        /// Gets the Store Risk Level
        /// </summary>
        [Write(false)]
        public bool GetHasRiskCalculation => _hasRiskCalculation;

        /// <summary>
        /// Gets or sets the collect type
        /// </summary>
        [Write(false)]
        public CollectType CollectType { get; set; }

        /// <summary>
        /// Gets or sets the business group
        /// </summary>
        [Write(false)]
        public BusinessGroup BusinessGroup { get; set; }

        /// <summary>
        /// Gets or sets the assurance company
        /// </summary>
        [Write(false)]
        public AssuranceCompany AssuranceCompany { get; set; }

        /// <summary>
        /// Gets or sets the payment type
        /// </summary>
        [Write(false)]
        public PaymentType PaymentType { get; set; }

        /// <summary>
        /// Gets or sets the store category
        /// </summary>
        [Write(false)]
        public StoreCategory StoreCategory { get; set; }

        /// <summary>
        /// Gets or sets the city
        /// </summary>
        [Write(false)]
        public City City { get; set; }

        /// <summary>
        /// Gets the create credit unatorized indicator
        /// </summary>
        [Write(false)]
        public bool CreateCreditUnautorized =>
            _status != (int)StoreStatuses.Active && _status != (int)StoreStatuses.NoPayments;

        /// <summary>
        /// Gets the payment unautorized indicator
        /// </summary>
        [Write(false)]
        public bool PaymentUnautorized =>
            _status != (int)StoreStatuses.Active && _status != (int)StoreStatuses.NoCredits;

        /// <summary>
        /// Gets the nit
        /// </summary>
        [Write(false)]
        public string GetNit => _nit;

        /// <summary>
        /// Creates a new instance of <see cref="Store"/>
        /// </summary>
        protected Store()
        {
        }

        /// <summary>
        /// New
        /// </summary>
        /// <returns></returns>
        public static Store New() =>
            new Store();

        /// <summary>
        /// Init
        /// </summary>
        /// <param name="credinetAppSettings"></param>
        /// <param name="effectiveAnnualRate"></param>
        internal void Init(CredinetAppSettings credinetAppSettings, decimal effectiveAnnualRate)
        {
            _assuranceType = (int)AssuranceTypes.TypeA;
            _paymentTypeId = (int)PaymentTypes.Ordinary;
            _collectTypeId = (int)CollectTypes.Ordinary;
            _status = (int)StoreStatuses.Active;

            _sendTokenMail = credinetAppSettings.StoreSendTokenMailDefault;
            _sendTokenSms = credinetAppSettings.StoreSendTokenSmsDefault;
            _storeCategoryId = credinetAppSettings.StoreCategoryIdDefault;
            _assuranceCompanyId = credinetAppSettings.StoreAssuranceCompanyIdDefault;
            _assurancePercentage = credinetAppSettings.StoreAssurancePercentageDefault;
            _downPaymentPercentage = credinetAppSettings.StoreDownPaymentPercentageDefault;
            _effectiveAnnualRate = effectiveAnnualRate;
            _mandatoryDownPayment = credinetAppSettings.StoreMandatoryDownPaymentDefault;
            _minimumFee = credinetAppSettings.StoreMinimumFeeDefault;
            _monthLimit = credinetAppSettings.StoreMonthLimitDefault;
            _hasRiskCalculation = credinetAppSettings.StoreHasRiskCalculationDefault;
        }

        /// <summary>
        /// Set basic info
        /// </summary>
        /// <param name="id"></param>
        /// <param name="storeName"></param>
        /// <param name="phones"></param>
        /// <param name="status"></param>
        /// <param name="allowPromissoryNoteSignature"></param>
        /// <param name="storeProfileCode"></param>
        internal void SetBasicInfo(string id, string storeName, string[] phones, int status, bool allowPromissoryNoteSignature, int storeProfileCode)
        {
            Id = id;
            StoreName = string.IsNullOrEmpty(storeName) ? StoreName : storeName;
            _phone = phones?.FirstOrDefault(p => !string.IsNullOrEmpty(p)) ?? string.Empty;
            _status = status == 0 ? _status : status;
            _allowPromissoryNoteSignature = allowPromissoryNoteSignature;
            _storeProfileCode = storeProfileCode;
        }

        /// <summary>
        /// Set vendor info
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="businessGroupId"></param>
        /// <param name="assuranceCompanyId"></param>
        /// <param name="nit"></param>
        internal void SetVendorInfo(string vendorId, string businessGroupId, long? assuranceCompanyId, string nit)
        {
            _vendorId = string.IsNullOrEmpty(vendorId) ? _vendorId : vendorId;
            _businessGroupId = string.IsNullOrEmpty(businessGroupId) ? _businessGroupId : businessGroupId;
            _assuranceCompanyId = assuranceCompanyId ?? _assuranceCompanyId;
            _nit = nit;
        }

        /// <summary>
        /// Set payment info
        /// </summary>
        /// <param name="collectTypeId"></param>
        /// <param name="paymentTypeId"></param>
        /// <param name="mandatoryDownPayment"></param>
        /// <param name="assuranceType"></param>
        internal void SetPaymentInfo(int collectTypeId, int paymentTypeId, int assuranceType, bool? mandatoryDownPayment)
        {
            _mandatoryDownPayment = mandatoryDownPayment ?? _mandatoryDownPayment;
            _assuranceType = assuranceType == 0 ? _assuranceType : assuranceType;
            _collectTypeId = collectTypeId == 0 ? _collectTypeId : collectTypeId;
            _paymentTypeId = paymentTypeId == 0 ? _paymentTypeId : paymentTypeId;
        }

        /// <summary>
        /// Set calculation values
        /// </summary>
        /// <param name="minimumFee"></param>
        /// <param name="downPaymentPercentage"></param>
        /// <param name="monthLimit"></param>
        /// <param name="effectiveAnnualRate"></param>
        /// <param name="assurancePercentage"></param>
        /// <param name="storeCategoryId"></param>
        internal void SetCalculationValues(decimal minimumFee, decimal downPaymentPercentage, int monthLimit, decimal effectiveAnnualRate, decimal assurancePercentage, int storeCategoryId)
        {
            _minimumFee = minimumFee == 0 ? _minimumFee : minimumFee;
            _downPaymentPercentage = downPaymentPercentage == 0 ? _downPaymentPercentage : downPaymentPercentage;
            _monthLimit = monthLimit == 0 ? _monthLimit : monthLimit;
            _effectiveAnnualRate = effectiveAnnualRate == 0 ? _effectiveAnnualRate : effectiveAnnualRate;
            _assurancePercentage = assurancePercentage == 0 ? _assurancePercentage : assurancePercentage;
            _storeCategoryId = storeCategoryId == 0 ? _storeCategoryId : storeCategoryId;
        }

        /// <summary>
        /// Set location
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="cityId"></param>
        internal void SetLocation(string stateId, string cityId)
        {
            _stateId = stateId;
            _cityId = cityId;
        }

        /// <summary>
        /// Update store
        /// </summary>
        /// <param name="storeRequest"></param>
        /// <param name="state"></param>
        /// <param name="city"></param>
        public void Update(StoreRequest storeRequest, State state, City city)
        {
            SetBasicInfo(Id, storeRequest.Name, storeRequest.Phones, storeRequest.Status, _allowPromissoryNoteSignature, storeRequest.StoreProfileCode);
            SetVendorInfo(storeRequest.VendorId, storeRequest.BusinessGroupId, storeRequest.AssuranceCompanyId, storeRequest.Nit);
            SetPaymentInfo(storeRequest.CollectTypeId, storeRequest.PaymentTypeId, storeRequest.AssuranceType,
                storeRequest.MandatoryDownPayment);
            SetCalculationValues(storeRequest.MinimumFee, storeRequest.DownPaymentPercentage, _monthLimit, _effectiveAnnualRate,
                _assurancePercentage, _storeCategoryId);
            SetLocation(state?.Id, city?.Id);
        }

        /// <summary>
        /// Set assurance company
        /// </summary>
        /// <param name="assuranceCompany"></param>
        public void SetAssuranceCompany(AssuranceCompany assuranceCompany)
        {
            AssuranceCompany = assuranceCompany;
            _assuranceCompanyId = assuranceCompany.Id;
        }

        /// <summary>
        /// Set payment type
        /// </summary>
        /// <param name="paymentType"></param>
        public void SetPaymentType(PaymentType paymentType)
        {
            PaymentType = paymentType;
            _paymentTypeId = paymentType.Id;
        }

        /// <summary>
        /// Initial fee
        /// </summary>
        /// <param name="creditValue"></param>
        /// <returns></returns>
        public decimal DownPayment(decimal creditValue) => creditValue * _downPaymentPercentage;

        /// <summary>
        /// Gets the time limit in months
        /// </summary>
        /// <param name="creditValue"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <returns></returns>
        public int GetTimeLimitInMonths(decimal creditValue, int decimalNumbersRound)
        {
            ValidateCreditValue(creditValue, decimalNumbersRound);

            return StoreCategory.GetTimeLimitInMonths(creditValue);
        }

        /// <summary>
        /// Set store category
        /// </summary>
        /// <param name="storeCategory"></param>
        public void SetStoreCategory(StoreCategory storeCategory)
        {
            StoreCategory = storeCategory;
            _storeCategoryId = storeCategory.Id;
        }

        /// <summary>
        /// Set city
        /// </summary>
        /// <param name="city"></param>
        public void SetCity(City city)
        {
            City = city;
        }

        /// <summary>
        /// Indicates is valid credit amount
        /// </summary>
        /// <param name="creditValue"></param>
        /// <param name="decimalNumbersRound"></param>
        /// <returns></returns>
        private void ValidateCreditValue(decimal creditValue, int decimalNumbersRound) => StoreCategory.ValidateCreditValue(creditValue, decimalNumbersRound);

        /// <summary>
        /// Set business group
        /// </summary>
        /// <param name="businessGroup"></param>
        public void SetBusinessGroup(BusinessGroup businessGroup)
        {
            BusinessGroup = businessGroup;
        }
    }
}