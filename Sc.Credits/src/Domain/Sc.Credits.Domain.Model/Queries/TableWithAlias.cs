namespace Sc.Credits.Domain.Model.Queries
{
    /// <summary>
    /// Table with alias
    /// </summary>
    public class TableWithAlias<TFields>
        : Table<TFields>
        where TFields : Fields
    {
        /// <summary>
        /// Alias
        /// </summary>
        public string Alias { get; }

        /// <summary>
        /// Name as
        /// </summary>
        public string NameAs { get; }

        /// <summary>
        /// Creates a new instance of <see cref="TableWithAlias{TFields}"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="suffix"></param>
        public TableWithAlias(string name, string suffix)
            : base(name, loadFields: false)
        {
            Alias = $"{name}_{suffix}";
            NameAs = $"{name} AS {Alias}";
            LoadFields(Alias);
        }
    }
}