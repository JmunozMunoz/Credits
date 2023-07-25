using Sc.Credits.Domain.Model.Queries;
using System.Reflection;

namespace Sc.Credits.Domain.Model.Sequences.Queries
{
    /// <summary>
    /// Sequence fields
    /// </summary>
    public class SequenceFieds
        : EntityFields
    {
        /// <summary>
        /// New sequence fields
        /// </summary>
        protected SequenceFieds(string alias)
            : base(alias)
        {
        }

        /// <summary>
        /// Gets the last number field
        /// </summary>
        public Field LastNumber => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the store id field
        /// </summary>
        public Field StoreId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the type field
        /// </summary>
        public Field Type => GetField(MethodBase.GetCurrentMethod().Name);
    }
}