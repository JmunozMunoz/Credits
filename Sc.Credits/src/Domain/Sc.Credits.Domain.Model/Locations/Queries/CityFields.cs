using Sc.Credits.Domain.Model.Queries;
using System.Reflection;

namespace Sc.Credits.Domain.Model.Locations.Queries
{
    /// <summary>
    /// City fields
    /// </summary>
    public class CityFields
        : LocationFields
    {
        /// <summary>
        /// Creates a new instance of <see cref="CityFields"/>
        /// </summary>
        public CityFields(string alias)
            : base(alias)
        {
        }

        /// <summary>
        /// Gets the state id field
        /// </summary>
        public Field StateId => GetField(MethodBase.GetCurrentMethod().Name);
    }
}