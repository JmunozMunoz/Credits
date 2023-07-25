using Sc.Credits.Domain.Model.Queries;
using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Stores.Queries.Commands
{
    /// <summary>
    /// Store command fields
    /// </summary>
    public static class StoreCommandsFields
    {
        private static readonly StoreFields _storeFields = Tables.Catalog.Stores.Fields;

        /// <summary>
        /// Update
        /// </summary>
        public static IEnumerable<Field> Update =>
            new List<Field>()
            {
                _storeFields.StoreName,
                _storeFields.Phone,
                _storeFields.Status,
                _storeFields.AllowPromissoryNoteSignature,
                _storeFields.VendorId,
                _storeFields.BusinessGroupId,
                _storeFields.AssuranceCompanyId,
                _storeFields.CollectTypeId,
                _storeFields.PaymentTypeId,
                _storeFields.AssuranceType,
                _storeFields.MandatoryDownPayment,
                _storeFields.MinimumFee,
                _storeFields.DownPaymentPercentage,
                _storeFields.MonthLimit,
                _storeFields.EffectiveAnnualRate,
                _storeFields.AssurancePercentage,
                _storeFields.StateId,
                _storeFields.CityId,
                _storeFields.StoreProfileCode,
                _storeFields.Nit
            };


        public static IEnumerable<Field> CustomerStoreInfo =>
            new List<Field>()
            {
                _storeFields.Id
            };
    }
}