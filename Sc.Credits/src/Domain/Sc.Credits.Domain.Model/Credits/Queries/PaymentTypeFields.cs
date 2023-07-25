using Sc.Credits.Domain.Model.Queries;
using System.Reflection;

namespace Sc.Credits.Domain.Model.Credits.Queries
{
    /// <summary>
    /// Payment type fields
    /// </summary>
    public class PaymentTypeFields
        : EntityFields
    {
        /// <summary>
        /// Creates a new instance of <see cref="PaymentTypeFields"/>
        /// </summary>
        protected PaymentTypeFields(string alias)
            : base(alias)
        {
        }

        /// <summary>
        /// Gets the name field
        /// </summary>
        public Field Name => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the type field
        /// </summary>
        public Field Type => GetField(MethodBase.GetCurrentMethod().Name);
    }
}