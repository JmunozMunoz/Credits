using Sc.Credits.Domain.Model.Queries;
using System.Reflection;

namespace Sc.Credits.Domain.Model.Credits.Queries
{
    /// <summary>
    /// The transaction fields entity
    /// </summary>
    public class TransactionReferenceFields
        : Fields
    {
        /// <summary>
        /// Creates a new instance of <see cref="TransactionReferenceFields"/>
        /// </summary>
        public TransactionReferenceFields(string alias)
            : base(alias)
        {
        }

        /// <summary>
        /// Gets the credit's id field
        /// </summary>
        public Field CreditId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the transaction's id field
        /// </summary>
        public Field TransactionId => GetField(MethodBase.GetCurrentMethod().Name);
    }
}