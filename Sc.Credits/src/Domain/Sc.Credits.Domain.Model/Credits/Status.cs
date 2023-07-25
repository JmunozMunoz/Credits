using Sc.Credits.Domain.Model.Base;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// Status
    /// </summary>
    public class Status
         : Entity<int>
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// New status
        /// </summary>
        public Status()
        {
        }

        /// <summary>
        /// New status
        /// </summary>
        /// <param name="name"></param>
        public Status(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Set id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public new Status SetId(int id)
        {
            base.SetId(id);
            return this;
        }
    }
}