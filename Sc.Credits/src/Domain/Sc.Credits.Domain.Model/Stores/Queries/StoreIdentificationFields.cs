using Sc.Credits.Domain.Model.Queries;
using System.Reflection;

namespace Sc.Credits.Domain.Model.Stores.Queries
{
    /// <summary>
    /// The store identification fields type.
    /// </summary>
    public class StoreIdentificationFields
        : Fields
    {
        /// <summary>
        /// Creates new instance of <see cref="StoreIdentificationFields"/>
        /// </summary>
        public StoreIdentificationFields(string alias)
            : base(alias)
        {
        }

        /// <summary>
        /// Gets the store's id field
        /// </summary>
        public Field StoreId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the code on legacy field
        /// </summary>
        public Field ScCode => GetField(MethodBase.GetCurrentMethod().Name);
    }
}