using Sc.Credits.Domain.Model.Base;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The source entity
    /// </summary>
    public class Source : Entity<int>
    {
        /// <summary>
        /// Gets the name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="Source"/>
        /// </summary>
        public Source()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="Source"/>
        /// </summary>
        /// <param name="name"></param>
        public Source(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Set id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public new Source SetId(int id)
        {
            base.SetId(id);
            return this;
        }
    }
}