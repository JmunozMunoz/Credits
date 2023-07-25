using Sc.Credits.Domain.Model.Base;
using System;

namespace Sc.Credits.Domain.Model.Stores
{
    /// <summary>
    /// The business group entity.
    /// </summary>
    public class BusinessGroup
        : Entity<string>
    {
        /// <summary>
        /// Gets the name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Creates new instance of <see cref="BusinessGroup"/>.
        /// </summary>
        protected BusinessGroup()
        {
            //Need to reflection
        }


        /// <summary>
        /// New
        /// </summary>
        /// <returns></returns>
        public static BusinessGroup New() =>
            new BusinessGroup();


        /// <summary>
        /// Creates new instance of <see cref="BusinessGroup"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public BusinessGroup(string id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Sets the name.
        /// </summary>
        /// <param name="name"></param>
        public void SetName(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Determines if a specific name matches the current name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool MatchName(string name)
            => Name == name;
    }
}