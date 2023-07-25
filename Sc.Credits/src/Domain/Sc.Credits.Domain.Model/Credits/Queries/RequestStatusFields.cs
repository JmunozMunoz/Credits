using Sc.Credits.Domain.Model.Queries;
using System.Reflection;

namespace Sc.Credits.Domain.Model.Credits.Queries
{
    /// <summary>
    /// Request status fields
    /// </summary>
    public class RequestStatusFields
        : EntityFields
    {
        /// <summary>
        /// Creates a new instance of <see cref="RequestStatusFields"/>
        /// </summary>
        /// <param name="alias"></param>
        public RequestStatusFields(string alias)
            : base(alias)
        {
        }

        /// <summary>
        /// Gets the name field
        /// </summary>
        public Field Name => GetField(MethodBase.GetCurrentMethod().Name);
    }
}