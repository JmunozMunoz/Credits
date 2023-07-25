using Sc.Credits.Domain.Model.Queries;
using System.Reflection;

namespace Sc.Credits.Domain.Model.Credits.Queries
{
    /// <summary>
    /// Credit master fields
    /// </summary>
    public class CreditMasterFields
        : EntityFields
    {
        /// <summary>
        /// Creates a new instance of <see cref="CreditMasterFields"/>
        /// </summary>
        protected CreditMasterFields(string alias)
            : base(alias)
        {
        }

        /// <summary>
        /// Gets the create date field
        /// </summary>
        public Field CreateDate => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the create time field
        /// </summary>
        public Field CreateTime => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the last date field
        /// </summary>
        public Field LastDate => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the last time field
        /// </summary>
        public Field LastTime => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the last id field
        /// </summary>
        public Field LastId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the customer's id field
        /// </summary>
        public Field CustomerId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the status' id field
        /// </summary>
        public Field StatusId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the store's id field
        /// </summary>
        public Field StoreId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the certified's id field
        /// </summary>
        public Field CertifiedId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the certifying authority field
        /// </summary>
        public Field CertifyingAuthority => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the credit date field
        /// </summary>
        public Field CreditDate => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the credit number field
        /// </summary>
        public Field CreditNumber => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the credit time field
        /// </summary>
        public Field CreditTime => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the invoice field
        /// </summary>
        public Field Invoice => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the products field
        /// </summary>
        public Field Products => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the reason field
        /// </summary>
        public Field Reason => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the code on legacy system field
        /// </summary>
        public Field ScCode => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the seller field
        /// </summary>
        public Field Seller => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the effective annual rate field
        /// </summary>
        public Field EffectiveAnnualRate => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the token field
        /// </summary>
        public Field Token => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the promissory note file name field
        /// </summary>
        public Field PromissoryNoteFileName => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the token field
        /// </summary>
        public Field RiskLevel => GetField(MethodBase.GetCurrentMethod().Name);
    }
}