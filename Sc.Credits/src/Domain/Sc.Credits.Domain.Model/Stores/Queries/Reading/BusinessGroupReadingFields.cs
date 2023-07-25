using Sc.Credits.Domain.Model.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sc.Credits.Domain.Model.Stores.Queries.Reading
{
    public class BusinessGroupReadingFields
    {

        private static readonly BusinessGroupFields _businessGroupFields = Tables.Catalog.BusinessGroup.Fields;

        /// <summary>
        /// Pay credit
        /// </summary>
        public static IEnumerable<Field> PayCredit =>
             (new List<Field>()
                {
                    _businessGroupFields.Id,
                    _businessGroupFields.Name
                });
    }
}
