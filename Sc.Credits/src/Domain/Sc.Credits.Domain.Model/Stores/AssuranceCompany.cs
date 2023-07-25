using Sc.Credits.Domain.Model.Base;

namespace Sc.Credits.Domain.Model.Stores
{
    /// <summary>
    /// Assurance company
    /// </summary>
    public class AssuranceCompany : Entity<long>
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Createas a new instance of <see cref="AssuranceCompany"/>
        /// </summary>
        public AssuranceCompany()
        {
        }

        /// <summary>
        /// Createas a new instance of <see cref="AssuranceCompany"/>
        /// </summary>
        /// <param name="name"></param>
        public AssuranceCompany(string name)
        {
            Name = name;
        }
    }
}