using Sc.Credits.Domain.Model.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.Model.Refinancings.Queries.Reading
{
    public class RefinancingLogDetailReadingFields
    {
        private static readonly RefinancingLogDetailFields _refinancingLogDetailsFields = Tables.Catalog.RefinancingLogDetails.Fields;

        /// <summary>
        /// Basic info
        /// </summary>
        public static IEnumerable<Field> BasicInfo =>
            new List<Field>()
            {
                _refinancingLogDetailsFields.Id,
                _refinancingLogDetailsFields.RefinancingLogId,
                _refinancingLogDetailsFields.CreditMasterId,
                _refinancingLogDetailsFields.Balance,
                _refinancingLogDetailsFields.Value

            };
    }
}
