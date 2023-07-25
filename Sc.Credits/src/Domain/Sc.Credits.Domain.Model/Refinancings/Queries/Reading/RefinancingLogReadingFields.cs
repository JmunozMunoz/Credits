using Sc.Credits.Domain.Model.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.Model.Refinancings.Queries.Reading
{
    public class RefinancingLogReadingFields
    {
        private static readonly RefinancingLogFields _refinancingLogFields = Tables.Catalog.RefinancingLogs.Fields;

        /// <summary>
        /// Basic info
        /// </summary>
        public static IEnumerable<Field> BasicInfo =>
            new List<Field>()
            {
                _refinancingLogFields.Id,
                _refinancingLogFields.CreditMasterId,
                _refinancingLogFields.ReferenceText
            };

    }
}
