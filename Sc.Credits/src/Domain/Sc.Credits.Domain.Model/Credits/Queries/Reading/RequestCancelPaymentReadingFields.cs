using Sc.Credits.Domain.Model.Queries;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.Domain.Model.Credits.Queries.Reading
{
    /// <summary>
    /// Request cancel payment reading fields
    /// </summary>
    public static class RequestCancelPaymentReadingFields
    {
        private static readonly RequestCancelPaymentFields _requestCancelPaymentFields = Tables.Catalog.RequestCancelPayments.Fields;

        /// <summary>
        /// Active and pending cancellation credits
        /// </summary>
        public static IEnumerable<Field> ActiveAndPendingCancellationCredits =>
            new List<Field>()
            {
                _requestCancelPaymentFields.Id,
                _requestCancelPaymentFields.RequestStatusId,
                _requestCancelPaymentFields.CreditMasterId,
                _requestCancelPaymentFields.CreditId,
                _requestCancelPaymentFields.StoreId
            };

        /// <summary>
        /// Request cancel credit
        /// </summary>
        public static IEnumerable<Field> RequestCancelCredit =>
            ActiveAndPendingCancellationCredits;

        /// <summary>
        /// Cancel credit
        /// </summary>
        public static IEnumerable<Field> CancelCredit =>
            RequestCancelCredit;

        /// <summary>
        /// Active and pending cancellation payments
        /// </summary>
        public static IEnumerable<Field> ActiveAndPendingCancellationPayments =>
            ActiveAndPendingCancellationCredits
                .Union(new List<Field>()
                {
                    _requestCancelPaymentFields.Date,
                    _requestCancelPaymentFields.Time
                });

        /// <summary>
        /// Request cancel payment
        /// </summary>
        public static IEnumerable<Field> RequestCancelPayment =>
            RequestCancelCredit;

        /// <summary>
        /// Pending
        /// </summary>
        public static IEnumerable<Field> Pending =>
            new List<Field>()
            {
                _requestCancelPaymentFields.Id,
                _requestCancelPaymentFields.RequestStatusId,
                _requestCancelPaymentFields.CreditMasterId,
                _requestCancelPaymentFields.CreditId,
                _requestCancelPaymentFields.UserName,
                _requestCancelPaymentFields.Date,
                _requestCancelPaymentFields.Reason,
                _requestCancelPaymentFields.UserId
            };

        /// <summary>
        /// Cancel payment
        /// </summary>
        public static IEnumerable<Field> CancelPayment =>
            CancelCredit
                .Union(new List<Field>()
                {
                    _requestCancelPaymentFields.Date,
                    _requestCancelPaymentFields.Time
                });

        /// <summary>
        /// Reject cancel payment
        /// </summary>
        public static IEnumerable<Field> RejectCancelPayment =>
            new List<Field>()
            {
                _requestCancelPaymentFields.Id,
                _requestCancelPaymentFields.RequestStatusId,
                _requestCancelPaymentFields.CreditMasterId,
                _requestCancelPaymentFields.CreditId
            };

        /// <summary>
        /// Customer payment history
        /// </summary>
        public static IEnumerable<Field> CustomerPaymentHistory =>
            new List<Field>()
            {
                _requestCancelPaymentFields.Id,
                _requestCancelPaymentFields.RequestStatusId,
                _requestCancelPaymentFields.CreditMasterId,
                _requestCancelPaymentFields.CreditId
            };

        /// <summary>
        /// Migration history
        /// </summary>
        public static IEnumerable<Field> MigrationHistory =>
            CustomerPaymentHistory;
    }
}