using Sc.Credits.Domain.Model.Queries;
using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Credits.Queries.Commands
{
    /// <summary>
    /// Request cancel credit commands fields
    /// </summary>
    public static class RequestCancelCreditCommandsFields
    {
        private static readonly RequestCancelCreditFields _requestCancelCreditFields = Tables.Catalog.RequestCancelCredits.Fields;

        /// <summary>
        /// Status update
        /// </summary>
        public static IEnumerable<Field> StatusUpdate =>
            new List<Field>()
            {
                _requestCancelCreditFields.ProcessDate,
                _requestCancelCreditFields.ProcessTime,
                _requestCancelCreditFields.ProcessUserId,
                _requestCancelCreditFields.ProcessUserName,
                _requestCancelCreditFields.RequestStatusId
            };
    }
}