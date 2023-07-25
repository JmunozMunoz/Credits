using Sc.Credits.Domain.Model.Queries;
using System.Reflection;

namespace Sc.Credits.Domain.Model.Credits.Queries
{
    /// <summary>
    /// Source fields
    /// </summary>
    public class SourceFields
        : EntityFields
    {
        /// <summary>
        /// Creates a new instance of <see cref="SourceFields"/>
        /// </summary>
        protected SourceFields(string alias)
            : base(alias)
        {
        }

        /// <summary>
        /// Gets the name field
        /// </summary>
        public Field Name => GetField(MethodBase.GetCurrentMethod().Name);
    }
}