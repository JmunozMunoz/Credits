using Sc.Credits.Helpers.Commons.Extensions;

namespace Sc.Credits.Domain.Model.Queries
{
    /// <summary>
    /// Table
    /// </summary>
    public class Table<TFields>
        where TFields : Fields
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Fields
        /// </summary>
        public TFields Fields { get; private set; }

        /// <summary>
        /// New table
        /// </summary>
        /// <param name="name"></param>
        /// <param name="loadFields"></param>
        public Table(string name, bool loadFields = true)
        {
            Name = name.Replace("get_", string.Empty);
            if (loadFields)
            {
                LoadFields(Name);
            }
        }

        /// <summary>
        /// Load fields
        /// </summary>
        /// <param name="alias"></param>
        protected void LoadFields(string alias)
        {
            Fields = TypeInstance.New<TFields>(alias);
        }

        /// <summary>
        /// With alias
        /// </summary>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public TableWithAlias<TFields> WithAlias(string suffix)
        {
            return new TableWithAlias<TFields>(Name, suffix);
        }
    }
}