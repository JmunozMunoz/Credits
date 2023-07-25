using Sc.Credits.Domain.Model.Queries;
using System.Collections.Generic;
using System.Linq;

namespace Sc.Credits.Domain.Model.Credits.Queries.Commands
{
    /// <summary>
    /// Request cancel payment commands fields
    /// </summary>
    public static class RequestCancelPaymentCommandsFields
    {
        private static readonly RequestCancelPaymentFields _requestCancelPaymentFields = Tables.Catalog.RequestCancelPayments.Fields;

        /// <summary>
        /// Status update
        /// </summary>
        public static IEnumerable<Field> StatusUpdate =>
            new List<Field>()
            {
                _requestCancelPaymentFields.ProcessDate,
                _requestCancelPaymentFields.ProcessTime,
                _requestCancelPaymentFields.ProcessUserId,
                _requestCancelPaymentFields.ProcessUserName,
                _requestCancelPaymentFields.RequestStatusId
            };

        /// <summary>
        /// Cancel payment
        /// </summary>
        public static IEnumerable<Field> CancelPayment =>
            StatusUpdate
                .Union(new List<Field>()
                {
                    _requestCancelPaymentFields.CreditCancelId
                });
    }
}