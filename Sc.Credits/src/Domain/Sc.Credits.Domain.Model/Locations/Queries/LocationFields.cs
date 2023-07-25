using Sc.Credits.Domain.Model.Queries;
using System.Reflection;

namespace Sc.Credits.Domain.Model.Locations.Queries
{
    /// <summary>
    /// Location fields
    /// </summary>
    public class LocationFields
        : EntityFields
    {
        /// <summary>
        /// Creates a new instance of <see cref="LocationFields"/>
        /// </summary>
        public LocationFields(string alias)
            : base(alias)
        {
        }

        /// <summary>
        /// Gets the name field
        /// </summary>
        public Field Name => GetField(MethodBase.GetCurrentMethod().Name);
    }
}