using System.Reflection;

namespace Sc.Credits.Domain.Model.Queries
{
    /// <summary>
    /// The entity fields type.
    /// </summary>
    public class EntityFields
        : Fields
    {
        /// <summary>
        /// Gets the Id field.
        /// </summary>
        public Field Id => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Creates new instance of <see cref="EntityFields"/>.
        /// </summary>
        /// <param name="alias"></param>
        protected EntityFields(string alias)
            : base(alias)
        {
        }
    }
}