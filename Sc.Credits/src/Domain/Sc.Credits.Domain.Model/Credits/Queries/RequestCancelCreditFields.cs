using Sc.Credits.Domain.Model.Queries;
using System.Reflection;

namespace Sc.Credits.Domain.Model.Credits.Queries
{
    /// <summary>
    /// Request cancel credit fields
    /// </summary>
    public class RequestCancelCreditFields
        : EntityFields
    {
        /// <summary>
        /// Creates a new instance of <see cref="RequestCancelCreditFields"/>
        /// </summary>
        protected RequestCancelCreditFields(string alias)
            : base(alias)
        {
        }

        /// <summary>
        /// Gets the credit master id field
        /// </summary>
        public Field CreditMasterId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the date field
        /// </summary>
        public Field Date => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the process date field
        /// </summary>
        public Field ProcessDate => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the process time field
        /// </summary>
        public Field ProcessTime => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the reason field
        /// </summary>
        public Field Reason => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the request status id field
        /// </summary>
        public Field RequestStatusId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the store's id field
        /// </summary>
        public Field StoreId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the time field
        /// </summary>
        public Field Time => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the username field
        /// </summary>
        public Field UserName => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the process user id field
        /// </summary>
        public Field ProcessUserId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the process user name field
        /// </summary>
        public Field ProcessUserName => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the user id field
        /// </summary>
        public Field UserId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        ///
        /// </summary>
        public Field ValueCancel => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        ///
        /// </summary>
        public Field CancellationType => GetField(MethodBase.GetCurrentMethod().Name);
    }
}