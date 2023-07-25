using Sc.Credits.Domain.Model.Base;

namespace Sc.Credits.Domain.Model.Credits
{
    /// <summary>
    /// The request status entity
    /// </summary>
    public class RequestStatus
        : Entity<int>
    {
        /// <summary>
        /// Gets the name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="RequestStatus"/>
        /// </summary>
        protected RequestStatus()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="RequestStatus"/>
        /// </summary>
        /// <param name="name"></param>
        public RequestStatus(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Set Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public new RequestStatus SetId(int id)
        {
            base.SetId(id);
            return this;
        }

        /// <summary>
        /// New
        /// </summary>
        /// <returns></returns>
        public static RequestStatus New() => new RequestStatus();
    }
}