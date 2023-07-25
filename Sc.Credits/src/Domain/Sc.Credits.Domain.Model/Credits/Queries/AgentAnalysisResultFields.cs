using Sc.Credits.Domain.Model.Queries;
using System.Reflection;

namespace Sc.Credits.Domain.Model.Credits.Queries
{
    public class AgentAnalysisResultFields
        : EntityFields
    {
        public AgentAnalysisResultFields(string alias)
            : base(alias)
        {

        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public Field Name => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public Field Description => GetField(MethodBase.GetCurrentMethod().Name);

    }
}
