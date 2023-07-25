using Sc.Credits.Domain.Model.Queries;
using System.Reflection;

namespace Sc.Credits.Domain.Model.Stores.Queries
{
    /// <summary>
    /// Business group fields
    /// </summary>
    public class BusinessGroupFields
        : EntityFields
    {
        /// <summary>
        /// New business group fields
        /// </summary>
        public BusinessGroupFields(string alias)
            : base(alias)
        {
        }

        /// <summary>
        /// Gets the name fields
        /// </summary>
        public Field Name => GetField(MethodBase.GetCurrentMethod().Name);
    }
}