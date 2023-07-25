using Sc.Credits.Domain.Model.Queries;
using System.Reflection;

namespace Sc.Credits.Domain.Model.Stores.Queries
{
    /// <summary>
    /// Collect type fields
    /// </summary>
    public class CollectTypeFields
        : EntityFields
    {
        /// <summary>
        /// New collect type fields
        /// </summary>
        public CollectTypeFields(string alias)
            : base(alias)
        {
        }

        /// <summary>
        /// Gets the name fields
        /// </summary>
        public Field Name => GetField(MethodBase.GetCurrentMethod().Name);
    }
}