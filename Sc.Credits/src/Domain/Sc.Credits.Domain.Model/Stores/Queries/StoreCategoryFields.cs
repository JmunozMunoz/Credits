using Sc.Credits.Domain.Model.Queries;
using System.Reflection;

namespace Sc.Credits.Domain.Model.Stores.Queries
{
    /// <summary>
    /// Store category fields
    /// </summary>
    public class StoreCategoryFields
        : EntityFields
    {
        /// <summary>
        /// New store category fields
        /// </summary>
        public StoreCategoryFields(string alias)
            : base(alias)
        {
        }

        /// <summary>
        /// Gets the name field
        /// </summary>
        public Field Name => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the regular fees number field
        /// </summary>
        public Field RegularFeesNumber => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the maximum fees number field
        /// </summary>
        public Field MaximumFeesNumber => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the minimum fee value field
        /// </summary>
        public Field MinimumFeeValue => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the maximum credit value field
        /// </summary>
        public Field MaximumCreditValue => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the fee cutoff value field
        /// </summary>
        public Field FeeCutoffValue => GetField(MethodBase.GetCurrentMethod().Name);
    }
}