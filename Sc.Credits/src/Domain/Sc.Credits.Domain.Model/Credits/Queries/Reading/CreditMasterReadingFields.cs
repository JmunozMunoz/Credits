using Sc.Credits.Domain.Model.Queries;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.Domain.Model.Credits.Queries.Reading
{
    /// <summary>
    /// Credit master reading fields
    /// </summary>
    public static class CreditMasterReadingFields
    {
        private static readonly CreditMasterFields _creditMasterFields = Tables.Catalog.CreditsMaster.Fields;

        /// <summary>
        /// Paid credit certificate
        /// </summary>
        public static IEnumerable<Field> PaidCreditCertificate =>
            new List<Field>()
            {
                _creditMasterFields.Id,
                _creditMasterFields.CreditDate,
                _creditMasterFields.StatusId,
                _creditMasterFields.CreditNumber,
                _creditMasterFields.LastId
            };

        /// <summary>
        /// Promissory note
        /// </summary>
        public static IEnumerable<Field> PromissoryNote =>
            PaidCreditCertificate
                .Union(new List<Field>()
                {
                    _creditMasterFields.EffectiveAnnualRate,
                    _creditMasterFields.Invoice,
                    _creditMasterFields.Token
                });

        /// <summary>
        /// Customer credit history
        /// </summary>
        public static IEnumerable<Field> CustomerCreditHistory =>
            PaidCreditCertificate
                .Union(new List<Field>()
                {
                    _creditMasterFields.CreditTime,
                    _creditMasterFields.StoreId
                });

        /// <summary>
        /// Customer payment history
        /// </summary>
        public static IEnumerable<Field> CustomerPaymentHistory =>
            CustomerCreditHistory;

        /// <summary>
        /// Active credits
        /// </summary>
        public static IEnumerable<Field> ActiveCredits =>
            new List<Field>()
                {
                    _creditMasterFields.Id,
                    _creditMasterFields.EffectiveAnnualRate,
                    _creditMasterFields.CreditDate,
                    _creditMasterFields.CreditTime,
                    _creditMasterFields.CreditNumber,
                    _creditMasterFields.LastId,
                    _creditMasterFields.StatusId
            };

        /// <summary>
        /// Active credits
        /// </summary>
        public static IEnumerable<Field> ActiveCreditsWithToken =>
            ActiveCredits.Union(new List<Field>() {
                _creditMasterFields.Token,
                _creditMasterFields.Invoice
            });

        /// <summary>
        /// Active and pending cancellation credits
        /// </summary>
        public static IEnumerable<Field> ActiveAndPendingCancellationCredits =>
            ActiveCredits;

        /// <summary>
        /// Request cancel credit
        /// </summary>
        public static IEnumerable<Field> RequestCancelCredit =>
            new List<Field>()
            {
                _creditMasterFields.Id,
                _creditMasterFields.LastId
            };

        /// <summary>
        /// Pending cancellation credits
        /// </summary>
        public static IEnumerable<Field> PendingCancellationCredits =>
            ActiveAndPendingCancellationCredits;

        /// <summary>
        /// Cancel credit reject
        /// </summary>
        public static IEnumerable<Field> CancelCreditReject =>
            RequestCancelCredit;

        /// <summary>
        /// Active and pending cancellation payments
        /// </summary>
        public static IEnumerable<Field> ActiveAndPendingCancellationPayments =>
            ActiveAndPendingCancellationCredits;

        /// <summary>
        /// Pending cancellation payments
        /// </summary>
        public static IEnumerable<Field> PendingCancellationPayments =>
            PendingCancellationCredits;

        /// <summary>
        /// Request cancel payment
        /// </summary>
        public static IEnumerable<Field> RequestCancelPayment =>
            RequestCancelCredit;

        /// <summary>
        /// Payment fees
        /// </summary>
        public static IEnumerable<Field> PaymentFees =>
            ActiveCredits;

        /// <summary>
        /// Payment templates
        /// </summary>
        public static IEnumerable<Field> PaymentTemplates =>
            PromissoryNote;

        /// <summary>
        /// Payment templates
        /// </summary>
        public static IEnumerable<Field> PendingSendNotification =>
            ActiveCredits.Union(new List<Field>()
                {
                    _creditMasterFields.StoreId
                });
    }
}