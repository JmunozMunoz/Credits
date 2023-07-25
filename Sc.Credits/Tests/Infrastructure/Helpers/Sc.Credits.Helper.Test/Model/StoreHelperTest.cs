namespace Sc.Credits.Helper.Test.Model
{
    using Domain.Model.Enums;
    using Domain.Model.Stores;
    using Sc.Credits.Domain.Model.Locations;
    using Sc.Credits.Helpers.ObjectsUtils;
    using System.Collections.Generic;

    /// <summary>
    /// Store Helper Test
    /// </summary>
    public static class StoreHelperTest
    {
        /// <summary>
        /// Get Store
        /// </summary>
        /// <returns></returns>
        public static Store GetStore(string id = null)
        {
            id = id ?? "454555458";
            Store store = StoreBuilder.CreateBuilder()
                .BasicInfo(id, storeName: "Store test", phones: new string[] { "2222222" }, status: 1, allowPromissoryNoteSignature: false, 1)
                .VendorInfo(vendorId: "1231243", businessGroupId: "2", assuranceCompanyId: null, nit: "")
                .PaymentInfo(collectTypeId: (int)CollectTypes.Ordinary, paymentTypeId: (int)PaymentTypes.Ordinary, mandatoryDownPayment: false,
                    assuranceType: (int)AssuranceTypes.TypeA)
                .CalculationValues(minimumFee: 40000M, downPaymentPercentage: 0.10M, monthLimit: 9, effectiveAnnualRate: 0.283200M, assurancePercentage: 0.100000M, storeCategoryId: 3)
                .Build();
            store.SetCity(new City("1", "Envigado", "1"));
            store.SetBusinessGroup(new BusinessGroup("1", "BusinessGroupName"));

            return store;
        }

        /// <summary>
        /// Get No Credits Store
        /// </summary>
        /// <returns></returns>
        public static Store GetStoreWithStatus(int storeStatus)
        {
            return StoreBuilder.CreateBuilder()
                .BasicInfo(id: "454555458", storeName: "Store test", phones: new string[] { "2222222" }, storeStatus, allowPromissoryNoteSignature: false, storeProfileCode: 9)
                .VendorInfo(vendorId: "1231243", businessGroupId: "2", assuranceCompanyId: null, nit: "")
                .PaymentInfo(collectTypeId: (int)CollectTypes.Ordinary, paymentTypeId: (int)PaymentTypes.Ordinary, mandatoryDownPayment: false,
                    assuranceType: (int)AssuranceTypes.TypeA)
                .CalculationValues(minimumFee: 40000M, downPaymentPercentage: 0.10M, monthLimit: 9, effectiveAnnualRate: 0.283200M, assurancePercentage: 0.100000M)
                .Build();
        }

        /// <summary>
        /// Get Store
        /// </summary>
        /// <returns></returns>
        public static Store GetStoreWithDefaultValues(CredinetAppSettings credinetAppSettings)
        {
            decimal effectiveAnnualRate = 0.25401M;
            return StoreBuilder.CreateBuilder().Init(credinetAppSettings, effectiveAnnualRate)
                .BasicInfo("454555458", storeName: "Store test", phones: new string[] { "2222222" }, status: 1, allowPromissoryNoteSignature: false, storeProfileCode: 9)
                .VendorInfo(vendorId: "1231243", businessGroupId: "2", assuranceCompanyId: null, nit: "")
                .PaymentInfo(collectTypeId: (int)CollectTypes.Ordinary, paymentTypeId: (int)PaymentTypes.Ordinary, mandatoryDownPayment: false,
                    assuranceType: (int)AssuranceTypes.TypeA)
                .CalculationValues(minimumFee: 40000M, downPaymentPercentage: 0.10M, monthLimit: 9, effectiveAnnualRate: 0.283200M, assurancePercentage: 0.100000M, storeCategoryId: 3)
                .Build();
        }

        /// <summary>
        /// Get Store Custom
        /// </summary>
        /// <param name="id"></param>
        /// <param name="minimumFee"></param>
        /// <param name="downPaymentPercentage"></param>
        /// <param name="storeName"></param>
        /// <param name="mandatoryDownPayment"></param>
        /// <param name="assuranceType"></param>
        /// <param name="vendorId"></param>
        /// <param name="effectiveAnualRate"></param>
        /// <returns></returns>
        public static Store GetStoreCustom(string id, decimal minimumFee, decimal downPaymentPercentage, string storeName, bool mandatoryDownPayment,
            AssuranceTypes assuranceType, string vendorId, decimal effectiveAnualRate)
        {
            return StoreBuilder.CreateBuilder()
                .BasicInfo(id, storeName, phones: new string[] { "2222222" }, status: 1, allowPromissoryNoteSignature: false, storeProfileCode: 9)
                .VendorInfo(vendorId, businessGroupId: "2", assuranceCompanyId: null, nit: "")
                .PaymentInfo(collectTypeId: (int)CollectTypes.Ordinary, paymentTypeId: (int)PaymentTypes.Ordinary,
                    (int)assuranceType, mandatoryDownPayment)
                .CalculationValues(minimumFee, downPaymentPercentage, monthLimit: 9, effectiveAnnualRate: effectiveAnualRate, assurancePercentage: 0.100000M)
                .Build();
        }

        /// <summary>
        /// Get store request from store
        /// </summary>
        /// <param name="store"></param>
        /// <param name="paymentType"></param>
        /// <returns></returns>
        public static StoreRequest GetStoreRequest(Store store, PaymentTypes paymentType) =>
            new StoreRequest
            {
                AssuranceCompanyId = store.GetAssuranceCompanyId,
                AssuranceType = store.GetAssuranceType,
                BusinessGroupId = store.GetBusinessGroupId,
                CollectTypeId = store.GetCollectTypeId,
                DownPaymentPercentage = store.GetDownPaymentPercentage,
                Id = store.Id,
                MandatoryDownPayment = store.GetMandatoryDownPayment,
                Name = store.StoreName,
                PaymentTypeId = (int)paymentType,
                Phones = new string[] { store.GetPhone },
                ScCode = "01",
                VendorId = store.GetVendorId,
                BusinessGroupName = "Test business group",
                ScCodeBusinessGroup = "1",
                Status = 1,
                State = new StateRequest
                {
                    Code = "01",
                    Name = "Antioquia"
                },
                City = new CityRequest
                {
                    Code = "0001",
                    Name = "Medellín"
                }
            };

        /// <summary>
        /// Get Store Limit Client
        /// </summary>
        /// <returns></returns>
        public static Store GetStoreLimitClient()
        {
            return StoreBuilder.CreateBuilder()
                .BasicInfo(id: "454555458", storeName: "Store test", phones: new string[] { "2222222" }, status: 1, allowPromissoryNoteSignature: false, storeProfileCode: 9)
                .VendorInfo(vendorId: "14524", businessGroupId: "2", assuranceCompanyId: null, nit: "")
                .PaymentInfo(collectTypeId: (int)CollectTypes.Ordinary, paymentTypeId: (int)PaymentTypes.Ordinary, mandatoryDownPayment: false,
                    assuranceType: (int)AssuranceTypes.TypeA)
                .CalculationValues(minimumFee: 40000M, downPaymentPercentage: 0.10M, monthLimit: 9, effectiveAnnualRate: 0.283200M, assurancePercentage: 0.100000M)
                .Build();
        }

        /// <summary>
        /// Get stores
        /// </summary>
        /// <returns></returns>
        public static List<Store> GetStores()
        {
            return new List<Store>
            {
                StoreBuilder.CreateBuilder()
                    .BasicInfo(id: "123456", storeName: "Prueba 1", phones: new string[] { "2222222" }, status: 1, allowPromissoryNoteSignature:false, storeProfileCode:9)
                    .VendorInfo(vendorId: "987654", businessGroupId: "2", assuranceCompanyId: null, nit: "")
                    .PaymentInfo(collectTypeId: (int)CollectTypes.All, paymentTypeId: (int)PaymentTypes.Ordinary, mandatoryDownPayment: false,
                        assuranceType: (int)AssuranceTypes.TypeA)
                    .CalculationValues(minimumFee: 20000, downPaymentPercentage: 0.10M, monthLimit: 9, effectiveAnnualRate: 0.283200M, assurancePercentage: 0.100000M)
                    .Build(),
                StoreBuilder.CreateBuilder()
                    .BasicInfo(id: "854456", storeName: "Prueba 2", phones: new string[] { "2222222" }, status: 1, allowPromissoryNoteSignature:false, storeProfileCode:9)
                    .VendorInfo(vendorId: "3652124", businessGroupId: "2", assuranceCompanyId: null, nit: "")
                    .PaymentInfo(collectTypeId: (int)CollectTypes.Ordinary, paymentTypeId: (int)PaymentTypes.Ordinary, mandatoryDownPayment: false,
                        assuranceType: (int)AssuranceTypes.TypeA)
                    .CalculationValues(minimumFee: 30000, downPaymentPercentage: 0.12M, monthLimit: 9, effectiveAnnualRate: 0.283200M, assurancePercentage: 0.100000M)
                    .Build(),
                StoreBuilder.CreateBuilder()
                    .BasicInfo(id: "965655", storeName: "Tienda de Prueba", phones: new string[] { "2222222" }, status: 1, allowPromissoryNoteSignature:false, storeProfileCode:9)
                    .VendorInfo(vendorId: "987654", businessGroupId: "2", assuranceCompanyId: null, nit: "")
                    .PaymentInfo(collectTypeId: (int)CollectTypes.AlternatePayment, paymentTypeId: (int)PaymentTypes.Ordinary, mandatoryDownPayment: false,
                        assuranceType: (int)AssuranceTypes.TypeB)
                    .CalculationValues(minimumFee: 50000, downPaymentPercentage: 0.10M, monthLimit: 9, effectiveAnnualRate: 0.283200M, assurancePercentage: 0.100000M)
                    .Build(),
                StoreBuilder.CreateBuilder()
                    .BasicInfo(id: "987754", storeName: "Toto", phones: new string[] { "2222222" }, status: 1, allowPromissoryNoteSignature:false, storeProfileCode:9)
                    .VendorInfo(vendorId: "987654", businessGroupId: "2", assuranceCompanyId: null, nit: "")
                    .PaymentInfo(collectTypeId: (int)CollectTypes.Ordinary, paymentTypeId: (int)PaymentTypes.Ordinary, mandatoryDownPayment: false,
                        assuranceType: (int)AssuranceTypes.TypeA)
                    .CalculationValues(minimumFee: 5000, downPaymentPercentage: 0.2M, monthLimit: 9, effectiveAnnualRate: 0.283200M, assurancePercentage: 0.100000M)
                    .Build(),
                StoreBuilder.CreateBuilder()
                    .BasicInfo(id: "asd556", storeName: "Nike", phones: new string[] { "2222222" }, status: 1, allowPromissoryNoteSignature:false, storeProfileCode:9)
                    .VendorInfo(vendorId: "8as7f8a", businessGroupId: "2", assuranceCompanyId: null, nit: "")
                    .PaymentInfo(collectTypeId: (int)CollectTypes.AlternatePayment, paymentTypeId: (int)PaymentTypes.Ordinary, mandatoryDownPayment: false,
                        assuranceType: (int)AssuranceTypes.TypeB)
                    .CalculationValues(minimumFee: 15000, downPaymentPercentage: 0.15M, monthLimit: 9, effectiveAnnualRate: 0.283200M, assurancePercentage: 0.100000M)
                    .Build(),
                StoreBuilder.CreateBuilder()
                    .BasicInfo(id: "9a5d8s", storeName: "Store test", phones: new string[] { "2222222" }, status: 1, allowPromissoryNoteSignature:false, storeProfileCode:9)
                    .VendorInfo(vendorId: "8as7f8a", businessGroupId: "2", assuranceCompanyId: null, nit: "")
                    .PaymentInfo(collectTypeId: (int)CollectTypes.All, paymentTypeId: (int)PaymentTypes.Ordinary, mandatoryDownPayment: false,
                        assuranceType: (int)AssuranceTypes.TypeA)
                    .CalculationValues(minimumFee: 20000, downPaymentPercentage: 0.15M, monthLimit: 9, effectiveAnnualRate: 0.283200M, assurancePercentage: 0.100000M)
                    .Build()
            };
        }

        public static Store GetStoreWithAllowPromissoryNoteSignature()
        {
             Store store =  StoreBuilder.CreateBuilder()
                .BasicInfo(id: "454555458", storeName: "Store test", phones: new string[] { "2222222" }, status: 1, allowPromissoryNoteSignature: true, storeProfileCode: 9)
                .VendorInfo(vendorId: "1231243", businessGroupId: "2", assuranceCompanyId: null, nit: "")
                .PaymentInfo(collectTypeId: (int)CollectTypes.Ordinary, paymentTypeId: (int)PaymentTypes.Ordinary, mandatoryDownPayment: false,
                    assuranceType: (int)AssuranceTypes.TypeA)
                .CalculationValues(minimumFee: 40000M, downPaymentPercentage: 0.10M, monthLimit: 9, effectiveAnnualRate: 0.283200M, assurancePercentage: 0.100000M)
                .Build();

            store.SetCity(new City("", "", ""));
            return store;
        }

        public static Store GetStoreCustomWithCustomStoreCategory(string id, decimal minimumFee, decimal downPaymentPercentage, string storeName, bool mandatoryDownPayment,
            AssuranceTypes assuranceType, string vendorId, decimal effectiveAnualRate, string name, int regularFeesNumber, int maximumFeesNumber,
            decimal minimumFeeValue, decimal maximumCreditValue, decimal feeCutoffValue)
        {
            Store store = GetStoreCustom(id, minimumFee, downPaymentPercentage, storeName, mandatoryDownPayment,
             assuranceType, vendorId, effectiveAnualRate);
            store.SetStoreCategory(new StoreCategory(name, regularFeesNumber, maximumFeesNumber, minimumFeeValue, maximumCreditValue, feeCutoffValue));
            return store;
        }

        public static Store GetStoreWithCustomStoreCategory(string name, int regularFeesNumber, int maximumFeesNumber,
            decimal minimumFeeValue, decimal maximumCreditValue, decimal feeCutoffValue, string id = null)
        {
            Store store = GetStore(id);
            store.SetStoreCategory(new StoreCategory(name, regularFeesNumber, maximumFeesNumber, minimumFeeValue, maximumCreditValue, feeCutoffValue));
            return store;
        }
    }
}