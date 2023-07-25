namespace Sc.Credits.Domain.Model.Queries
{
    /// <summary>
    /// Field
    /// </summary>
    public class Field
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Name with alias
        /// </summary>
        public string NameWithAlias => $"{_alias}.{Name}";

        private readonly string _alias;

        /// <summary>
        /// New field
        /// </summary>
        /// <param name="name"></param>
        /// <param name="alias"></param>
        public Field(string name, string alias)
        {
            Name = name.Replace("get_", string.Empty);
            _alias = alias;
        }
    }
}