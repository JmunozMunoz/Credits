using Sc.Credits.Domain.Model.Queries;
using System.Reflection;

namespace Sc.Credits.Domain.Model.Credits.Queries
{
    /// <summary>
    /// Request cancel payment fields
    /// </summary>
    public class RequestCancelPaymentFields
        : EntityFields
    {
        /// <summary>
        /// Creates a new instance of <see cref="RequestCancelPaymentFields"/>
        /// </summary>
        protected RequestCancelPaymentFields(string alias)
            : base(alias)
        {
        }

        /// <summary>
        /// Gets the credit cancel id field
        /// </summary>
        public Field CreditCancelId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the credit id field
        /// </summary>
        public Field CreditId => GetField(MethodBase.GetCurrentMethod().Name);

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
        /// Gets the process username field
        /// </summary>
        public Field ProcessUserName => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the user id field
        /// </summary>
        public Field UserId => GetField(MethodBase.GetCurrentMethod().Name);
    }
}