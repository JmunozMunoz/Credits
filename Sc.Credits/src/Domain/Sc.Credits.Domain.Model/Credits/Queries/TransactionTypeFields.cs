using Sc.Credits.Domain.Model.Queries;
using System.Reflection;

namespace Sc.Credits.Domain.Model.Credits.Queries
{
    /// <summary>
    /// The transaction type fields entity
    /// </summary>
    public class TransactionTypeFields
        : EntityFields
    {
        /// <summary>
        /// Creates a new instance of <see cref="TransactionTypeFields"/>
        /// </summary>
        protected TransactionTypeFields(string alias)
            : base(alias)
        {
        }

        /// <summary>
        /// Gets the name field
        /// </summary>
        public Field Name => GetField(MethodBase.GetCurrentMethod().Name);
    }
}