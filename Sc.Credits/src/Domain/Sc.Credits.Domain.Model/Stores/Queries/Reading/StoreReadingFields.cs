using Sc.Credits.Domain.Model.Queries;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.Domain.Model.Stores.Queries.Reading
{
    /// <summary>
    /// Store reading fields
    /// </summary>
    public static class StoreReadingFields
    {
        private static readonly StoreFields _storeFields = Tables.Catalog.Stores.Fields;

        /// <summary>
        /// Basic info
        /// </summary>
        public static IEnumerable<Field> BasicInfo =>
            new List<Field>()
            {
                _storeFields.Id,
                _storeFields.StoreName,
                _storeFields.AssuranceCompanyId,
                _storeFields.PaymentTypeId
            };

        /// <summary>
        /// Credits in cancellation request
        /// </summary>
        public static IEnumerable<Field> CreditsInCancellationRequest =>
            BasicInfo
                .Union(new List<Field>()
                {
                    _storeFields.VendorId
                });

        /// <summary>
        /// Credits in cancellation request
        /// </summary>
        public static IEnumerable<Field> VendorIdentifier =>
            BasicInfo
                .Union(new List<Field>()
                {
                    _storeFields.VendorId,
                    _storeFields.Nit
                });

        /// <summary>
        /// Limit months
        /// </summary>
        public static IEnumerable<Field> LimitMonths =>
            BasicInfo
                .Union(new List<Field>()
                {
                    _storeFields.StoreCategoryId,
                    _storeFields.AssurancePercentage,
                    _storeFields.MinimumFee,
                    _storeFields.MonthLimit,
                    _storeFields.StoreProfileCode
                });

        /// <summary>
        /// Credit details
        /// </summary>
        public static IEnumerable<Field> CreditDetails =>
            LimitMonths
                .Union(new List<Field>()
                {
                    _storeFields.MandatoryDownPayment,
                    _storeFields.DownPaymentPercentage,
                    _storeFields.EffectiveAnnualRate,
                    _storeFields.AssuranceType
                });

        /// <summary>
        /// Create credit
        /// </summary>
        public static IEnumerable<Field> CreateCredit =>
            CreditDetails
                .Union(new List<Field>()
                {
                    _storeFields.Status,
                    _storeFields.VendorId
                });

        /// <summary>
        /// Credit creation
        /// </summary>
        public static IEnumerable<Field> CreditCreation =>
            BasicInfo
                .Union(new List<Field>()
                {
                    _storeFields.EffectiveAnnualRate,
                    _storeFields.AllowPromissoryNoteSignature,
                    _storeFields.Phone,
                    _storeFields.CityId,
                    _storeFields.Nit,
                    _storeFields.BusinessGroupId,
                    _storeFields.VendorId
                });

        /// <summary>
        /// Paid credit certificate
        /// </summary>
        public static IEnumerable<Field> PaidCreditCertificate =>
            BasicInfo;

        /// <summary>
        /// Customer credit history
        /// </summary>
        public static IEnumerable<Field> CustomerCreditHistory =>
            BasicInfo;

        /// <summary>
        /// Active credits
        /// </summary>
        public static IEnumerable<Field> ActiveCredits =>
            BasicInfo
                .Union(new List<Field>
                {
                    _storeFields.CollectTypeId,
                    _storeFields.VendorId,
                    _storeFields.BusinessGroupId,
                    _storeFields.EffectiveAnnualRate
                });

        /// <summary>
        /// Token
        /// </summary>
        public static IEnumerable<Field> Token =>
            CreditDetails
                .Union(new List<Field>()
                {
                    _storeFields.Status,
                    _storeFields.VendorId,
                    _storeFields.SendTokenMail,
                    _storeFields.SendTokenSms,
                    _storeFields.HasRiskCalculation
                });

        /// <summary>
        /// Payment fees
        /// </summary>
        public static IEnumerable<Field> PaymentFees =>
            BasicInfo
                .Union(new List<Field>()
                {
                    _storeFields.EffectiveAnnualRate
                });

        /// <summary>
        /// Prommisory note
        /// </summary>
        public static IEnumerable<Field> PrommisoryNote =>
            PaymentFees
                .Union(new List<Field>()
                {
                    _storeFields.Phone
                });

        /// <summary>
        /// Pay credit
        /// </summary>
        public static IEnumerable<Field> BusinessGroupIdentifiers =>
            CreateCredit
             .Union(new List<Field>()
                {
                    _storeFields.Nit,
                    _storeFields.BusinessGroupId
                });

        /// <summary>
        /// Customer payment history
        /// </summary>
        public static IEnumerable<Field> CustomerPaymentHistory =>
            ActiveCredits;

        /// <summary>
        /// Active and pending cancellation credits
        /// </summary>
        public static IEnumerable<Field> ActiveAndPendingCancellationCredits =>
            ActiveCredits;

        /// <summary>
        /// Active and pending cancellation payments
        /// </summary>
        public static IEnumerable<Field> ActiveAndPendingCancellationPayments =>
            ActiveCredits;

        /// <summary>
        /// Refinancing
        /// </summary>
        public static IEnumerable<Field> Refinancing =>
            ActiveCredits;

        /// <summary>
        /// Payment templates
        /// </summary>
        public static IEnumerable<Field> PaymentTemplates =>
            BasicInfo
                .Union(new List<Field>()
                {
                    _storeFields.Phone
                });
    }
}