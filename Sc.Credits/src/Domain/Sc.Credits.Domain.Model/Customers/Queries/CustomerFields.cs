using Sc.Credits.Domain.Model.Queries;
using System.Reflection;

namespace Sc.Credits.Domain.Model.Customers.Queries
{
    /// <summary>
    /// Customer fields
    /// </summary>
    public class CustomerFields
        : EntityFields
    {
        /// <summary>
        /// Creates a new instance of <see cref="CustomerFields"/>
        /// </summary>
        protected CustomerFields(string alias)
            : base(alias)
        {
        }

        /// <summary>
        /// Gets the document's id field
        /// </summary>
        public Field IdDocument => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the document's type field
        /// </summary>
        public Field DocumentType => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the profile id field
        /// </summary>
        public Field ProfileId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the available credit limit field
        /// </summary>
        public Field AvailableCreditLimit => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the create date field
        /// </summary>
        public Field CreateDate => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the create time field
        /// </summary>
        public Field CreateTime => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the credit limit field
        /// </summary>
        public Field CreditLimit => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the email field
        /// </summary>
        public Field Email => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the full name field
        /// </summary>
        public Field FullName => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the first name field
        /// </summary>
        public Field FirstName => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the second name field
        /// </summary>
        public Field SecondName => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the first last name field
        /// </summary>
        public Field FirstLastName => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the second last name field
        /// </summary>
        public Field SecondLastName => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the mobile number field
        /// </summary>
        public Field Mobile => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the update date field
        /// </summary>
        public Field UpdateDate => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the update time field
        /// </summary>
        public Field UpdateTime => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the validated mail
        /// </summary>
        public Field ValidatedMail => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the send token mail
        /// </summary>
        public Field SendTokenMail => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the send token sms
        /// </summary>
        public Field SendTokenSms => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the status
        /// </summary>
        public Field Status => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the credit limit update date
        /// </summary>
        public Field CreditLimitUpdateDate => GetField(MethodBase.GetCurrentMethod().Name);
    }
}