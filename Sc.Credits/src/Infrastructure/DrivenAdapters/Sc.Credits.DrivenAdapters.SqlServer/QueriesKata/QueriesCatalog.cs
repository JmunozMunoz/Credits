using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Credits;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Customers;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Locations;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Parameters;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Refinancings;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Sequences;
using Sc.Credits.DrivenAdapters.SqlServer.QueriesKata.Stores;

namespace Sc.Credits.DrivenAdapters.SqlServer.QueriesKata
{
    /// <summary>
    /// Queries catalog
    /// </summary>
    internal static class QueriesCatalog
    {
        /// <summary>
        /// Credit master
        /// </summary>
        public static readonly CreditMasterQueries CreditMaster = new CreditMasterQueries();

        /// <summary>
        /// Credit
        /// </summary>
        public static readonly CreditQueries Credit = new CreditQueries();

        /// <summary>
        /// Payment type
        /// </summary>
        public static readonly PaymentTypeQueries PaymentType = new PaymentTypeQueries();

        /// <summary>
        /// Customer
        /// </summary>
        public static readonly CustomerQueries Customer = new CustomerQueries();

        /// <summary>
        /// Profile
        /// </summary>
        public static readonly ProfileQueries Profile = new ProfileQueries();

        /// <summary>
        /// Assurance company
        /// </summary>
        public static readonly AssuranceCompanyQueries AssuranceCompany = new AssuranceCompanyQueries();

        /// <summary>
        /// Collect type
        /// </summary>
        public static readonly CollectTypeQueries CollectType = new CollectTypeQueries();

        /// <summary>
        /// Store
        /// </summary>
        public static readonly StoreQueries Store = new StoreQueries();

        /// <summary>
        /// Parameter
        /// </summary>
        public static readonly ParameterQueries Parameter = new ParameterQueries();

        /// <summary>
        /// Transaction type
        /// </summary>
        public static readonly TransactionTypeQueries TransactionType = new TransactionTypeQueries();

        /// <summary>
        /// Status
        /// </summary>
        public static readonly StatusQueries Status = new StatusQueries();

        /// <summary>
        /// Source
        /// </summary>
        public static readonly SourceQueries Source = new SourceQueries();

        /// <summary>
        /// City
        /// </summary>
        public static readonly CityQueries City = new CityQueries();

        /// <summary>
        /// State
        /// </summary>
        public static readonly StateQueries State = new StateQueries();

        /// <summary>
        /// Sequence
        /// </summary>
        public static readonly SequenceQueries Sequence = new SequenceQueries();

        /// <summary>
        /// Store identification
        /// </summary>
        public static readonly StoreIdentificationQueries StoreIdentification = new StoreIdentificationQueries();

        /// <summary>
        /// Business group
        /// </summary>
        public static readonly BusinessGroupQueries BusinessGroup = new BusinessGroupQueries();

        /// <summary>
        /// Auth method
        /// </summary>
        public static readonly AuthMethodQueries AuthMethod = new AuthMethodQueries();

        /// <summary>
        /// Request cancel payment queries
        /// </summary>
        public static readonly RequestCancelPaymentQueries RequestCancelPayment = new RequestCancelPaymentQueries();

        /// <summary>
        /// Request cancel credit queries
        /// </summary>
        public static readonly RequestCancelCreditQueries RequestCancelCredit = new RequestCancelCreditQueries();

        /// <summary>
        /// Request status
        /// </summary>
        public static readonly RequestStatusQueries RequestStatus = new RequestStatusQueries();

        /// <summary>
        /// Refinancing application
        /// </summary>
        public static readonly RefinancingApplicationQueries RefinancingApplication = new RefinancingApplicationQueries();

        /// <summary>
        /// Refinancing log detail
        /// </summary>
        public static readonly RefinancingLogDetailQueries RefinancingLogDetail = new RefinancingLogDetailQueries();

        /// <summary>
        /// Refinancing log 
        /// </summary>
        public static readonly RefinancingLogQueries RefinancingLog = new RefinancingLogQueries();

        /// <summary>
        /// Stores categories queries
        /// </summary>
        public static readonly StoreCategoryQueries StoreCategory = new StoreCategoryQueries();

        /// <summary>
        /// Unapproved credit
        /// </summary>
        public static readonly UnapprovedCreditQueries UnapprovedCredit = new UnapprovedCreditQueries();

        /// <summary>
        /// The record status of sending token client queries
        /// </summary>
        public static readonly CreditRequestAgentAnalysisQueries CreditRequestAgentAnalysisQueries = new CreditRequestAgentAnalysisQueries();

        /// <summary>
        /// The status fraud detaill queries
        /// </summary>
        public static readonly AgentAnalysisResultQueries AgentAnalysisResultQueries = new AgentAnalysisResultQueries();
    }
}