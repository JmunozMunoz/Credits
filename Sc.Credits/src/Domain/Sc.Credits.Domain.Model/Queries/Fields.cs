namespace Sc.Credits.Domain.Model.Queries
{
    /// <summary>
    /// The fields type.
    /// </summary>
    public class Fields
    {
        private readonly string _alias;

        /// <summary>
        /// Gets a field by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected Field GetField(string name) =>
            new Field(name, _alias);

        /// <summary>
        /// Creates new instance of <see cref="Fields"/>.
        /// </summary>
        /// <param name="alias"></param>
        protected Fields(string alias)
        {
            _alias = alias;
        }
    }
}