using Sc.Credits.Domain.Model.Queries;
using System.Collections.Generic;

namespace Sc.Credits.Domain.Model.Customers.Queries.Reading
{
    /// <summary>
    /// Profile reading fields
    /// </summary>
    public static class ProfileReadingFields
    {
        private static readonly ProfileFields _profileFields = Tables.Catalog.Profiles.Fields;

        /// <summary>
        /// Mandatory down payment
        /// </summary>
        public static IEnumerable<Field> MandatoryDownPayment =>
            new List<Field>()
            {
                _profileFields.Id,
                _profileFields.MandatoryDownPayment
            };
    }
}