using Sc.Credits.Domain.Model.Queries;
using System.Reflection;

namespace Sc.Credits.Domain.Model.Refinancings.Queries
{
    /// <summary>
    /// The refinancing application fields
    /// </summary>
    public class RefinancingApplicationFields
        : EntityFields
    {
        /// <summary>
        /// Creates a new instance of <see cref="RefinancingApplicationFields"/>
        /// </summary>
        /// <param name="alias"></param>
        public RefinancingApplicationFields(string alias)
            : base(alias)
        {
        }

        /// <summary>
        /// Gets the arrears days from field
        /// </summary>
        public Field ArrearsDaysFrom => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the arrears days to field
        /// </summary>
        public Field ArrearsDaysTo => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the creation date from field
        /// </summary>
        public Field CreationDateFrom => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the creation date to field
        /// </summary>
        public Field CreationDateTo => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the name field
        /// </summary>
        public Field Name => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the value from field
        /// </summary>
        public Field ValueFrom => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the value to field
        /// </summary>
        public Field ValueTo => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the allow refinancing credits field
        /// </summary>
        public Field AllowRefinancingCredits => GetField(MethodBase.GetCurrentMethod().Name);
    }
}