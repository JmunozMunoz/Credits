using Sc.Credits.Domain.Model.Base;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// AgentAnalysisResult
    /// </summary>
    /// <seealso cref="IAggregateRoot" />
    public class AgentAnalysisResult :
        Entity<int>, IAggregateRoot
    {
        /// <summary>
        /// Gets the name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the description
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="AgentAnalysisResult"/>
        /// </summary>
        public AgentAnalysisResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentAnalysisResult"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        public AgentAnalysisResult(string name, string description)
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Set the id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public new AgentAnalysisResult SetId(int id)
        {
            base.SetId(id);
            return this;
        }
    }
}