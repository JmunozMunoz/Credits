using Sc.Credits.Domain.Model.Queries;
using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Credits.Queries.Reading
{
    /// <summary>
    /// Request cancel credit reading fields
    /// </summary>
    public static class RequestCancelCreditReadingFields
    {
        private static readonly RequestCancelCreditFields _requestCancelCreditFields = Tables.Catalog.RequestCancelCredits.Fields;

        /// <summary>
        /// Pending
        /// </summary>
        public static IEnumerable<Field> Pending =>
            new List<Field>()
            {
                _requestCancelCreditFields.Id,
                _requestCancelCreditFields.RequestStatusId,
                _requestCancelCreditFields.CreditMasterId,
                _requestCancelCreditFields.UserName,
                _requestCancelCreditFields.Date,
                _requestCancelCreditFields.Reason,
                _requestCancelCreditFields.UserId,
                _requestCancelCreditFields.CancellationType,
                _requestCancelCreditFields.ValueCancel
            };
    }
}