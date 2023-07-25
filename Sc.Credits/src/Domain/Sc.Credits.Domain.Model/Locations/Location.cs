using Sc.Credits.Domain.Model.Base;

namespace Sc.Credits.Domain.Model.Locations
{
    /// <summary>
    /// Location base
    /// </summary>
    public class Location
        : Entity<string>, IAggregateRoot
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// New location
        /// </summary>
        protected Location()
        {
            //Need to reflection
        }

        /// <summary>
        /// New location
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public Location(string id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Set name
        /// </summary>
        /// <param name="name"></param>
        public void SetName(string name)
        {
            Name = name;
        }
    }
}