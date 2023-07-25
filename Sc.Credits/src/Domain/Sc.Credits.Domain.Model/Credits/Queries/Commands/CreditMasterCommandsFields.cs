using Sc.Credits.Domain.Model.Queries;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.Domain.Model.Credits.Queries.Commands
{
    /// <summary>
    /// Credit master commands fields
    /// </summary>
    public static class CreditMasterCommandsFields
    {
        private static readonly CreditMasterFields _creditMasterFields = Tables.Catalog.CreditsMaster.Fields;

        /// <summary>
        /// Last update
        /// </summary>
        public static IEnumerable<Field> LastUpdate =>
            new List<Field>()
            {
                _creditMasterFields.LastDate,
                _creditMasterFields.LastTime
            };

        /// <summary>
        /// Promisory note file name update
        /// </summary>
        public static IEnumerable<Field> PrommisoryNoteFileNameUpdate =>
            LastUpdate
                .Union(new List<Field>()
                {
                    _creditMasterFields.PromissoryNoteFileName
                });

        /// <summary>
        /// Certification update
        /// </summary>
        public static IEnumerable<Field> CertificationUpdate =>
            LastUpdate
                .Union(new List<Field>()
                {
                    _creditMasterFields.CertifiedId,
                    _creditMasterFields.CertifyingAuthority
                });

        /// <summary>
        /// Seller info update
        /// </summary>
        public static IEnumerable<Field> SellerInfoUpdate =>
            LastUpdate
                .Union(new List<Field>()
                {
                    _creditMasterFields.Seller,
                    _creditMasterFields.Invoice,
                    _creditMasterFields.Products
                });

        /// <summary>
        /// New transaction update
        /// </summary>
        public static IEnumerable<Field> NewTransactionUpdate =>
            LastUpdate
                .Union(new List<Field>()
                {
                    _creditMasterFields.LastId
                });

        /// <summary>
        /// Status update
        /// </summary>
        public static IEnumerable<Field> StatusUpdate =>
            new List<Field>()
            {
                _creditMasterFields.StatusId
            };

        /// <summary>
        /// Pay credit
        /// </summary>
        public static IEnumerable<Field> PayCredit =>
            StatusUpdate;
    }
}