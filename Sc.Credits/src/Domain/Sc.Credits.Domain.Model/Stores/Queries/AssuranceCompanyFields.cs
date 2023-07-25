using Sc.Credits.Domain.Model.Queries;
using System.Reflection;

namespace Sc.Credits.Domain.Model.Stores.Queries
{
    /// <summary>
    /// Assurance company fields
    /// </summary>
    public class AssuranceCompanyFields
        : EntityFields
    {
        /// <summary>
        /// New assurance company fields
        /// </summary>
        public AssuranceCompanyFields(string alias)
            : base(alias)
        {
        }

        /// <summary>
        /// Gets the name field
        /// </summary>
        public Field Name => GetField(MethodBase.GetCurrentMethod().Name);
    }
}