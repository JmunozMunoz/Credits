using Sc.Credits.Domain.Model.Credits.Queries;
using Sc.Credits.Domain.Model.Customers.Queries;
using Sc.Credits.Domain.Model.Locations.Queries;
using Sc.Credits.Domain.Model.Refinancings.Queries;
using Sc.Credits.Domain.Model.Sequences.Queries;
using Sc.Credits.Domain.Model.Stores.Queries;
using System.Reflection;

namespace Sc.Credits.Domain.Model.Queries
{
    /// <summary>
    /// Tables
    /// </summary>
    public class Tables
    {
        /// <summary>
        /// Catalog of tables
        /// </summary>
        public static readonly Tables Catalog = new Tables();

        /// <summary>
        /// Gets the assurance companies table
        /// </summary>
        public Table<AssuranceCompanyFields> AssuranceCompanies =>
            GetTable<AssuranceCompanyFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the auth methods table
        /// </summary>
        public Table<AuthMethodFields> AuthMethods =>
            GetTable<AuthMethodFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the business group table
        /// </summary>
        public Table<BusinessGroupFields> BusinessGroup =>
            GetTable<BusinessGroupFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the cities table
        /// </summary>
        public Table<CityFields> Cities =>
            GetTable<CityFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the collect types table
        /// </summary>
        public Table<CollectTypeFields> CollectTypes =>
            GetTable<CollectTypeFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the credits table
        /// </summary>
        public Table<CreditFields> Credits =>
            GetTable<CreditFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the credits master table
        /// </summary>
        public Table<CreditMasterFields> CreditsMaster =>
            GetTable<CreditMasterFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the customers table
        /// </summary>
        public Table<CustomerFields> Customers =>
            GetTable<CustomerFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the message error logs table
        /// </summary>
        public Table<EntityFields> MessageErrorLogs =>
            GetTable<EntityFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the parameters table
        /// </summary>
        public Table<EntityFields> Parameters =>
            GetTable<EntityFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the payment types table
        /// </summary>
        public Table<PaymentTypeFields> PaymentTypes =>
            GetTable<PaymentTypeFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the profiles table
        /// </summary>
        public Table<ProfileFields> Profiles =>
            GetTable<ProfileFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the refinancing applications table
        /// </summary>
        public Table<RefinancingApplicationFields> RefinancingApplications =>
            GetTable<RefinancingApplicationFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the refinancing log details table
        /// </summary>
        public Table<RefinancingLogDetailFields> RefinancingLogDetails =>
            GetTable<RefinancingLogDetailFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the refinancing logs table
        /// </summary>
        public Table<RefinancingLogFields> RefinancingLogs =>
            GetTable<RefinancingLogFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the request cancel credits table
        /// </summary>
        public Table<RequestCancelCreditFields> RequestCancelCredits =>
            GetTable<RequestCancelCreditFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the request cancel payments table
        /// </summary>
        public Table<RequestCancelPaymentFields> RequestCancelPayments =>
            GetTable<RequestCancelPaymentFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the request status table
        /// </summary>
        public Table<RequestStatusFields> RequestStatus =>
            GetTable<RequestStatusFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the sequences table
        /// </summary>
        public Table<SequenceFieds> Sequences =>
            GetTable<SequenceFieds>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the sources table
        /// </summary>
        public Table<SourceFields> Sources =>
            GetTable<SourceFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the states table
        /// </summary>
        public Table<StateFields> States =>
            GetTable<StateFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the status table
        /// </summary>
        public Table<StatusFields> Status =>
            GetTable<StatusFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the store identifications table
        /// </summary>
        public Table<StoreIdentificationFields> StoreIdentifications =>
            GetTable<StoreIdentificationFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the transaction references table
        /// </summary>
        public Table<TransactionReferenceFields> TransactionReferences =>
            GetTable<TransactionReferenceFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the stores table
        /// </summary>
        public Table<StoreFields> Stores =>
            GetTable<StoreFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the transaction types table
        /// </summary>
        public Table<TransactionTypeFields> TransactionTypes =>
            GetTable<TransactionTypeFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the stores categories table
        /// </summary>
        public Table<StoreCategoryFields> StoresCategories =>
            GetTable<StoreCategoryFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the unapproved credits table
        /// </summary>
        public Table<UnapprovedCreditFields> UnapprovedCredits =>
            GetTable<UnapprovedCreditFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the record status of sending token client.
        /// </summary>
        /// <value>
        /// The record status of sending token client.
        /// </value>
        public Table<CreditRequestAgentAnalysisFields> CreditRequestAgentAnalyses =>
            GetTable<CreditRequestAgentAnalysisFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets the status fraud detaill.
        /// </summary>
        /// <value>
        /// The status fraud detaill.
        /// </value>
        public Table<AgentAnalysisResultFields> AgentAnalysisResults =>
            GetTable<AgentAnalysisResultFields>(MethodBase.GetCurrentMethod().Name);

        /// <summary>
        /// Gets table by name
        /// </summary>
        public static Table<TFields> GetTable<TFields>(string name)
            where TFields : Fields =>
            new Table<TFields>(name);
    }
}