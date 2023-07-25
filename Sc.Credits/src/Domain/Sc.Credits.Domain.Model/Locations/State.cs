namespace Sc.Credits.Domain.Model.Locations
{
    /// <summary>
    /// State
    /// </summary>
    public class State : Location
    {
        /// <summary>
        /// Creates a new instance of <see cref="State"/>
        /// </summary>
        protected State()
        {
            //Need to reflection
        }

        /// <summary>
        /// Creates a new instance of <see cref="State"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public State(string id, string name)
            : base(id, name)
        {
        }
    }
}