using Sc.Credits.Domain.Model.Queries;
using System.Reflection;

namespace Sc.Credits.Domain.Model.Customers.Queries
{
    /// <summary>
    /// Profile fields
    /// </summary>
    public class ProfileFields
        : EntityFields
    {
        /// <summary>
        /// Creates a new instance of <see cref="ProfileFields"/>
        /// </summary>
        protected ProfileFields(string alias)
            : base(alias)
        {
        }

        /// <summary>
        /// Gets the name field
        /// </summary>
        public Field Name => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the mandatory down payment field
        /// </summary>
        public Field MandatoryDownPayment => GetField(MethodBase.GetCurrentMethod().Name);
    }
}