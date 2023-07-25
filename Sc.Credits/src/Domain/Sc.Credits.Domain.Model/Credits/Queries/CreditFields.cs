using Sc.Credits.Domain.Model.Queries;
using System.Reflection;

namespace Sc.Credits.Domain.Model.Credits.Queries
{
    /// <summary>
    /// Credit fields
    /// </summary>
    public class CreditFields
        : EntityFields
    {
        /// <summary>
        /// Creates a new instance of <see cref="CreditFields"/>
        /// </summary>
        protected CreditFields(string alias)
            : base(alias)
        {
        }

        /// <summary>
        /// Gets the arrears value paid field
        /// </summary>
        public Field ArrearsValuePaid => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the assurance value paid field
        /// </summary>
        public Field AssuranceValuePaid => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the bank account field
        /// </summary>
        public Field BankAccount => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the calculation date field
        /// </summary>
        public Field CalculationDate => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the charge value paid field
        /// </summary>
        public Field ChargeValuePaid => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the credit value paid field
        /// </summary>
        public Field CreditValuePaid => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the due date field
        /// </summary>
        public Field DueDate => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the interest value paid field
        /// </summary>
        public Field InterestValuePaid => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the last payment date field
        /// </summary>
        public Field LastPaymentDate => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the last payment time field
        /// </summary>
        public Field LastPaymentTime => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the payment type's id field
        /// </summary>
        public Field PaymentTypeId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the payment type's name field
        /// </summary>
        public Field PaymentTypeName => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the total value paid field
        /// </summary>
        public Field TotalValuePaid => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the transaction type id field
        /// </summary>
        public Field TransactionTypeId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the transaction type's name field
        /// </summary>
        public Field TransactionTypeName => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the store's id field
        /// </summary>
        public Field StoreId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the customer's id field
        /// </summary>
        public Field CustomerId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the status' id field
        /// </summary>
        public Field StatusId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the source's id field
        /// </summary>
        public Field SourceId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the authorization method's id field
        /// </summary>
        public Field AuthMethodId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the alternate payment field
        /// </summary>
        public Field AlternatePayment => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the arrears charge field
        /// </summary>
        public Field ArrearsCharge => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the arrears days field
        /// </summary>
        public Field ArrearsDays => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the assurance fee field
        /// </summary>
        public Field AssuranceFee => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the assurance percentage field
        /// </summary>
        public Field AssurancePercentage => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the assurance value field
        /// </summary>
        public Field AssuranceValue => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the auth method name field
        /// </summary>
        public Field AuthMethodName => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the balance field
        /// </summary>
        public Field Balance => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the charge value field
        /// </summary>
        public Field ChargeValue => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the create date field
        /// </summary>
        public Field CreateDate => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the create time field
        /// </summary>
        public Field CreateTime => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the credit master's id field
        /// </summary>
        public Field CreditMasterId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the credit number field
        /// </summary>
        public Field CreditNumber => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the credit value field
        /// </summary>
        public Field CreditValue => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the fee value field
        /// </summary>
        public Field FeeValue => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the fees field
        /// </summary>
        public Field Fees => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the frequency field
        /// </summary>
        public Field Frequency => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the interest rate field
        /// </summary>
        public Field InterestRate => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the last fee field
        /// </summary>
        public Field LastFee => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the location field
        /// </summary>
        public Field Location => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the source name field
        /// </summary>
        public Field SourceName => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the status name field
        /// </summary>
        public Field StatusName => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the transaction date field
        /// </summary>
        public Field TransactionDate => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the transaction time field
        /// </summary>
        public Field TransactionTime => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the updated payment plan value field
        /// </summary>
        public Field UpdatedPaymentPlanValue => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the user's id field
        /// </summary>
        public Field UserId => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the assurance balance field
        /// </summary>
        public Field AssuranceBalance => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the assurance total value field
        /// </summary>
        public Field AssuranceTotalValue => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the down payment field
        /// </summary>
        public Field DownPayment => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the payment number field
        /// </summary>
        public Field PaymentNumber => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the username field
        /// </summary>
        public Field UserName => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the assurance total fee value field
        /// </summary>
        public Field AssuranceTotalFeeValue => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the total down payment field
        /// </summary>
        public Field TotalDownPayment => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the has arrears charge field
        /// </summary>
        public Field HasArrearsCharge => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the active fee value paid field
        /// </summary>
        public Field ActiveFeeValuePaid => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the previous arrears field
        /// </summary>
        public Field PreviousArrears => GetField(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the previous interest field
        /// </summary>
        public Field PreviousInterest => GetField(MethodBase.GetCurrentMethod().Name);
    }
}