using Sc.Credits.Domain.Model.Base;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// Auth method entity
    /// </summary>
    public class AuthMethod : Entity<int>, IAggregateRoot
    {
        /// <summary>
        /// Gets the name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="AuthMethod"/>
        /// </summary>
        public AuthMethod()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="AuthMethod"/>
        /// </summary>
        /// <param name="name"></param>
        public AuthMethod(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Set Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public new AuthMethod SetId(int id)
        {
            base.SetId(id);
            return this;
        }
    }
}