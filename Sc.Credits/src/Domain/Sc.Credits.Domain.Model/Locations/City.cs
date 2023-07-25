using Sc.Credits.Helpers.Commons.Dapper.Annotations;

namespace Sc.Credits.Domain.Model.Locations
{
    /// <summary>
    /// City
    /// </summary>
    public class City : Location
    {
        /// <summary>
        /// State id
        /// </summary>
        public string StateId { get; set; }

        /// <summary>
        /// State
        /// </summary>
        [Write(false)]
        public State State { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="City"/>
        /// </summary>
        protected City()
        {
            //Need to reflection
        }

        /// <summary>
        /// Creates a new instance of <see cref="City"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="stateId"></param>
        public City(string id, string name, string stateId)
            : base(id, name)
        {
            StateId = stateId;
        }
    }
}