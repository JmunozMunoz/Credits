using Sc.Credits.Domain.Model.Queries;
using System.Reflection;

namespace Sc.Credits.Domain.Model.Credits.Queries
{
    /// <summary>
    /// Auth method fields
    /// </summary>
    public class AuthMethodFields
        : EntityFields
    {
        /// <summary>
        /// Creates a new instance of <see cref="AuthMethodFields"/>
        /// </summary>
        protected AuthMethodFields(string alias)
            : base(alias)
        {
        }

        /// <summary>
        /// Gets the name field
        /// </summary>
        public Field Name => GetField(MethodBase.GetCurrentMethod().Name);
    }
}