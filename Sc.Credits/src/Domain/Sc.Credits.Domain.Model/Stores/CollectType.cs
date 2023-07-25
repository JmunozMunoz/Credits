using Sc.Credits.Domain.Model.Base;

namespace Sc.Credits.Domain.Model.Stores
{
    /// <summary>
    /// Collect type entity
    /// </summary>
    public class CollectType : Entity<int>
    {
        /// <summary>
        /// Gets the name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="CollectType"/>
        /// </summary>
        /// <param name="name"></param>
        public CollectType(string name)
        {
            Name = name;
        }
    }
}