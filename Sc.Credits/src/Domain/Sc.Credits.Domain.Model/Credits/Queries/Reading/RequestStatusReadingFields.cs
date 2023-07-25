using Sc.Credits.Domain.Model.Queries;
using Sc.Credits.Domain.Model.Queries.Extensions;
using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Credits.Queries.Reading
{
    /// <summary>
    /// Request status reading fields
    /// </summary>
    public static class RequestStatusReadingFields
    {
        private static readonly RequestStatusFields _requestStatusFields = Tables.Catalog.RequestStatus.Fields;

        /// <summary>
        /// Credit payment history
        /// </summary>
        public static IEnumerable<Field> CustomerPaymentHistory =>
            _requestStatusFields.GetAllFields();
    }
}