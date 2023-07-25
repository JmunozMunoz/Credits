using Sc.Credits.Domain.Model.Queries;
using System.Reflection;

namespace Sc.Credits.Domain.Model.Credits.Queries
{
    /// <summary>
    /// Status fields
    /// </summary>
    public class StatusFields
        : EntityFields
    {
        /// <summary>
        /// Creates a new instance of <see cref="StatusFields"/>
        /// </summary>
        protected StatusFields(string alias)
            : base(alias)
        {
        }

        /// <summary>
        /// Gets the name field
        /// </summary>
        public Field Name => GetField(MethodBase.GetCurrentMethod().Name);
    }
}