using Sc.Credits.Helpers.ObjectsUtils.ApplicationSettings;
using System;

namespace Sc.Credits.Helpers.ObjectsUtils
{
    /// <summary>
    /// Credinet app settings
    /// </summary>
    public class CredinetAppSettings
    {
        /// <summary>
        ///DataBase CrediNet Credits
        /// </summary>
        public string DataBaseCrediNetCredits { get; set; }

        /// <summary>
        /// Application Id
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// Application Secret
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        ///Storage Account
        /// </summary>
        public string StorageAccount { get; set; }

        /// <summary>
        ///Default Country
        /// </summary>
        public string DefaultCountry { get; set; }

        /// <summary>
        /// Default SubDomain
        /// </summary>
        public string DefaultSubDomain { get; set; }

        /// <summary>
        ///Events Topic Name
        /// </summary>
        public string EventsTopicName { get; set; }

        /// <summary>
        /// Service bus connection string request
        /// </summary>
        public string ServiceBusConnectionStringRequest { get; set; }

        /// <summary>
        /// Store Queue Name Subscription
        /// </summary>
        public string StoreQueueNameSubscription { get; set; }

        /// <summary>
        /// Credit Topic Name
        /// </summary>
        public string CreditTopicName { get; set; }

        /// <summary>
        /// Credit Token Validate Queue
        /// </summary>
        public string CreditTokenValidateQueue { get; set; }

        /// <summary>
        /// Credit Token Generate Queue
        /// </summary>
        public string CreditTokenGenerateQueue { get; set; }

        /// <summary>
        /// CreditMasterQueue
        /// </summary>
        public string CreditScCodeQueue { get; set; }

        /// <summary>
        /// Mail from
        /// </summary>
        public string MailFrom { get; set; }

        /// <summary>
        /// Mail from name
        /// </summary>
        public string MailFromName { get; set; }

        /// <summary>
        /// Gets or sets the mail message notification.
        /// </summary>
        /// <value>
        /// The mail message notification.
        /// </value>
        public string MailMessageNotification { get; set; }

        /// <summary>
        /// Mail messages queue
        /// </summary>
        public string MailMessagesQueue { get; set; }

        /// <summary>
        /// Sms queue
        /// </summary>
        public string SmsQueue { get; set; }

        /// <summary>
        /// Pdf blob container name
        /// </summary>
        public string PdfBlobContainerName { get; set; }

        /// <summary>
        /// Promissory note path
        /// </summary>
        public string PromissoryNotePath { get; set; }

        /// <summary>
        /// Culture info
        /// </summary>
        public string CultureInfo { get; set; }

        /// <summary>
        /// Credit token notification template id
        /// </summary>
        public string CreditTokenNotificationTemplateId { get; set; }

        /// <summary>
        /// Create credit notification template id
        /// </summary>
        public string CreateCreditNotificationTemplateId { get; set; }

        /// <summary>
        /// Credit token call request queue
        /// </summary>
        public string CreditTokenCallRequestQueue { get; set; }

        /// <summary>
        /// Create Credit Notification Queue
        /// </summary>
        public string CreateCreditNotificationQueue { get; set; }

        /// <summary>
        /// Active credits request payment gateway credits queue
        /// </summary>
        public string ActiveCreditsRequestPaymentGatewayCreditsQueue { get; set; }

        /// <summary>
        /// Pay credits request payment gateway credits queue
        /// </summary>
        public string PayCreditsRequestPaymentGatewayCreditsQueue { get; set; }

        /// <summary>
        /// Gets or sets the simulation records queue.
        /// </summary>
        /// <value>
        /// The simulation records queue.
        /// </value>
        public string SimulationRecordsQueue { get; set; }

        /// <summary>
        /// Company Name
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Amortization Schedule Url Template
        /// </summary>
        public string AmortizationScheduleUrlTemplate { get; set; }

        /// <summary>
        /// Credit Extra Values Queue
        /// </summary>
        public string CreditExtraValuesQueue { get; set; }

        /// <summary>
        /// Pay Notification Mail Template
        /// </summary>
        public string PayNotificationMailTemplate { get; set; }

        /// <summary>
        /// Pay Notification Mail Template
        /// </summary>
        public string MultiplePaymentsNotificationMailTemplate { get; set; }

        /// <summary>
        /// Url SMS Notification Pay
        /// </summary>
        public string UrlSmsNotificationPay { get; set; }

        /// <summary>
        /// SqlConnectionString
        /// </summary>
        public string SqlConnectionString { get; set; }

        /// <summary>
        /// Enable entity framework logger factory
        /// </summary>
        public string EnableEFLoggerFactory { get; set; }

        /// <summary>
        /// Sms Currency Code
        /// </summary>
        public string SmsCurrencyCode { get; set; }

        /// <summary>
        /// Query Credits
        /// </summary>
        public string QueryCredits { get; set; }

        /// <summary>
        /// Store month limit default
        /// </summary>
        public int StoreMonthLimitDefault { get; set; }

        /// <summary>
        /// Store assurance percentage default
        /// </summary>
        public decimal StoreAssurancePercentageDefault { get; set; }

        /// <summary>
        /// Store minimum fee default
        /// </summary>
        public decimal StoreMinimumFeeDefault { get; set; }

        /// <summary>
        /// Gets or sets the store category id default value
        /// </summary>
        public int StoreCategoryIdDefault { get; set; }

        /// <summary>
        /// Store mandatory down payment default
        /// </summary>
        public bool StoreMandatoryDownPaymentDefault { get; set; }

        /// <summary>
        /// Store assurance company id default
        /// </summary>
        public int StoreAssuranceCompanyIdDefault { get; set; }

        /// <summary>
        /// Store down payment percentage default
        /// </summary>
        public decimal StoreDownPaymentPercentageDefault { get; set; }

        /// <summary>
        /// Payment future seconds limit
        /// </summary>
        public string PaymentFutureSecondsLimit { get; set; }

        /// <summary>
        /// Templates blob container name
        /// </summary>
        public string TemplatesBlobContainerName { get; set; }

        /// <summary>
        /// Templates cache minutes
        /// </summary>
        public int TemplatesCacheMinutes { get; set; }

        /// <summary>
        /// Credit customer migration history queue
        /// </summary>
        public string CreditCustomerMigrationHistoryQueue { get; set; }

        /// <summary>
        /// Parameters cache minutes
        /// </summary>
        public double ParametersCacheMilliseconds { get; set; }

        /// <summary>
        /// Return token value
        /// </summary>
        public bool ReturnTokenValue { get; set; }

        /// <summary>
        /// PaidCreditCertificatePrintTemplate
        /// </summary>
        public string PaidCreditCertificatePrintTemplate { get; set; }

        /// <summary>
        /// Credit Payment Print Template
        /// </summary>
        public string CreditPaymentPrintTemplate { get; set; }

        /// <summary>
        /// Topic credit limit update
        /// </summary>
        public string TopicCreditLimitUpdate { get; set; }

        /// <summary>
        /// Credit limit update subscription
        /// </summary>
        public string CreditLimitUpdateSuscription { get; set; }

        /// <summary>
        /// Topic max concurrent calls
        /// </summary>
        public int TopicMaxConcurrentCalls { get; set; }

        /// <summary>
        /// Queue max concurrent calls
        /// </summary>
        public int QueueMaxConcurrentCalls { get; set; }

        /// <summary>
        /// Response reply max concurrent sessions
        /// </summary>
        public int ResponseReplyMaxConcurrentSessions { get; set; }

        /// <summary>
        /// Domain name
        /// </summary>
        public string DomainName { get; set; }

        /// <summary>
        /// End Point Autentic
        /// </summary>
        public string EndPointAutentic { get; set; }

        /// <summary>
        /// End Point Oauth Autentic
        /// </summary>
        public string EndPointOauthAutentic { get; set; }

        /// <summary>
        /// Audience Autentic
        /// </summary>
        public string AudienceAutentic { get; set; }

        /// <summary>
        /// Grant Type Autentic
        /// </summary>
        public string GrantTypeAutentic { get; set; }

        /// <summary>
        /// Client Id Autentic
        /// </summary>
        public string ClientIdAutentic { get; set; }

        /// <summary>
        /// Client Secret Autentc
        /// </summary>
        public string ClientSecretAutentc { get; set; }

        /// <summary>
        /// Validate token on create
        /// </summary>
        public bool ValidateTokenOnCreate { get; set; }

        /// <summary>
        /// Pay credits events credits topic
        /// </summary>
        public string PayCreditsEventsCreditsTopic { get; set; }

        /// <summary>
        /// Active credits events credits topic
        /// </summary>
        public string ActiveCreditsEventsCreditsTopic { get; set; }

        /// <summary>
        /// Customer update customer events topic
        /// </summary>
        public string CustomerUpdateCustomerEventsTopic { get; set; }

        /// <summary>
        /// Credits update customer events subscription
        /// </summary>
        public string CreditsUpdateCustomerEventsSubscription { get; set; }

        /// <summary>
        /// Customer update migration topic
        /// </summary>
        public string CustomerUpdateMigrationTopic { get; set; }

        /// <summary>
        /// Credits update migration subscription
        /// </summary>
        public string CreditsUpdateMigrationSubscription { get; set; }

        /// <summary>
        /// Customer update study events
        /// </summary>
        public string CustomerUpdateStudyEventsTopic { get; set; }

        /// <summary>
        /// Credits update study events subscription
        /// </summary>
        public string CreditsUpdateStudyEventsSubscription { get; set; }

        /// <summary>
        /// Customer status update legacy topic
        /// </summary>
        public string CustomerStatusUpdateLegacyTopic { get; set; }

        /// <summary>
        /// Credits status update legacy subscription
        /// </summary>
        public string CreditsStatusUpdateLegacySubscription { get; set; }

        /// <summary>
        /// Cutomer update mail mobile topic
        /// </summary>
        public string CustomerUpdateMailMobileTopic { get; set; }

        /// <summary>
        /// Credits mail mobile subscription
        /// </summary>
        public string CreditsMailMobileSubscription { get; set; }

        /// <summary>
        /// Store allow promissory note signature default
        /// </summary>
        public bool StoreAllowPromissoryNoteSignatureDefault { get; set; }

        /// <summary>
        /// Refinancing store id
        /// </summary>
        public string RefinancingStoreId { get; set; }

        /// <summary>
        /// Refinancing fees allowed
        /// </summary>
        public string RefinancingFeesAllowed { get; set; }

        /// <summary>
        /// Refinancing sources allowed
        /// </summary>
        public string RefinancingSourcesAllowed { get; set; }

        /// <summary>
        /// Active credits included store ids
        /// </summary>
        public string ActiveCreditsIncludedStoreIds { get; set; }

        /// <summary>
        /// Messging sql connection string
        /// </summary>
        public string MessagingSqlConnectionString { get; set; }

        /// <summary>
        /// Messaging database name
        /// </summary>
        public string MessagingDatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the store send token mail default value
        /// </summary>
        public bool StoreSendTokenMailDefault { get; set; }

        /// <summary>
        /// Gets or sets the store send token sms default value
        /// </summary>
        public bool StoreSendTokenSmsDefault { get; set; }

        /// <summary>
        /// Gets or sets the user to reject cancellation requests.
        /// </summary>

        public string UserToRejectCancellationRequests { get; set; }

        /// <summary>
        /// Gets or sets the user id to reject cancellation requests.
        /// </summary>
        public string UserIdToRejectCancellationRequests { get; set; }

        /// <summary>
        /// Gets or sets the reject cancellation request cron.
        /// </summary>
        public string RejectCancellationRequestCron { get; set; }

        /// <summary>
        /// Gets or sets the partial cancelation store id.
        /// </summary>
        public string PartialCancelationStoreId { get; set; }

        /// <summary>
        /// Gets or sets the generic store.
        /// </summary>
        /// <value>
        /// The generic store.
        /// </value>
        public string GenericStore { get; set; }

        /// <summary>
        /// Gets or sets the report templates endpoint.
        /// </summary>
        /// <value>
        /// The generic store.
        /// </value>
        public string ReportTemplatesEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the report templates api key.
        /// </summary>
        /// <value>
        /// The generic store.
        /// </value>
        public string ReportTemplatesApiKey { get; set; }

        /// <summary>
        /// Gets or sets the report templates generation path.
        /// </summary>
        /// <value>
        /// The generic store.
        /// </value>
        public string ReportTemplatesGenerationPath { get; set; }

        /// <summary>
        /// Gets or sets the validation.
        /// </summary>
        /// <value>
        /// The validation.
        /// </value>
        public ValidationSettings Validation { get; set; }

        /// <summary>
        /// Gets or sets the company number.
        /// </summary>
        public string CompanyNumber { get; set; }

        /// <summary>
        /// Gets or sets the company city.
        /// </summary>
        public string CompanyCity { get; set; }

        /// <summary>
        /// Gets or sets the account statement certificate name.
        /// </summary>
        public string CertificateAccountStatementName { get; set; }

        /// <summary>
        /// Gets or sets the certificate account statement lender name.
        /// </summary>
        public string CertificateAccountStatementLenderName { get; set; }

        /// <summary>
        /// Gets or sets the certificate account statement report name.
        /// </summary>
        public string CertificateAccountStatementReportName { get; set; }

        /// <summary>
        /// Gets or sets the check customer history
        /// </summary>
        public bool CheckCustomerHistory { get; set; }

        /// <summary>
        /// Gets or sets the secret key v3 re captcha.
        /// </summary>
        /// <value>
        /// The secret key v3 re captcha.
        /// </value>
        public string SecretKeyV3ReCaptcha { get; set; }

        /// <summary>
        /// Gets or sets the google base URL.
        /// </summary>
        /// <value>
        /// The google base URL.
        /// </value>
        public string GoogleBaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the recaptcha validation token endpoint.
        /// </summary>
        /// <value>
        /// The recaptcha validation token endpoint.
        /// </value>
        public string RecaptchaValidationTokenEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the minimum recaptcha score value.
        /// </summary>
        /// <value>
        /// The minimum recaptcha score value.
        /// </value>
        public decimal MinimumRecaptchaScoreValue { get; set; }

        /// <summary>
        /// Gets or sets the minimum recaptcha score value.
        /// </summary>
        public string CustomerRiskLevelCalculationQueue { get; set; }

        /// <summary>
        /// Gets or sets the risky credit request template's id
        /// </summary>
        public string RiskyCreditRequestTemplateId { get; set; }

        /// <summary>
        /// Gets or sets the risky credit notification recipient
        /// </summary>
        public string RiskyCreditNotificationRecipient { get; set; }

        /// <summary>
        /// Time period to verify credit creation
        /// </summary>
        public int TimePeriodVerifyCreationMinutes { get; set; }

        /// <summary>
        /// Promissory note template name
        /// </summary>
        public string PromissoryNoteTemplateName { get; set; }

        /// <summary>
        /// Gets or sets the customer call notification template.
        /// </summary>
        /// <value>
        /// The customer call notification template.
        /// </value>
        public string CustomerCallNotificationTemplate { get; set; }

        /// <summary>
        /// Gets or sets the customer reject notification template.
        /// </summary>
        /// <value>
        /// The customer reject notification template.
        /// </value>
        public string CustomerRejectNotificationTemplate { get; set; }

        /// <summary>
        /// Gets or sets the customer reject call notification template.
        /// </summary>
        /// <value>
        /// The customer reject call notification template.
        /// </value>
        public string CustomerRejectCallNotificationTemplate { get; set; }

        /// <summary>
        /// Gets or sets the name of the mail notification request.
        /// </summary>
        /// <value>
        /// The name of the mail notification request.
        /// </value>
        public string MailNotificationRequestName { get; set; }

        /// <summary>
        /// Gets or sets the name of the SMS notification request.
        /// </summary>
        /// <value>
        /// The name of the SMS notification request.
        /// </value>
        public string SmsNotificationRequestName { get; set; }

        /// <summary>
        /// Gets or sets the customer risk level number.
        /// </summary>
        /// <value>
        /// The customer risk level number.
        /// </value>
        public string CustomerRiskLevelNumber { get; set; }

        /// <summary>
        /// Id document of simulated client
        /// </summary>
        public string SimulatedClientIdDocument { get; set; }

        /// <summary>
        /// Gets or sets the URL site fraud.
        /// </summary>
        /// <value>
        /// The URL site fraud.
        /// </value>
        public string UrlSiteFraud { get; set; }

        /// <summary>
        /// Gets or sets the mail footer template.
        /// </summary>
        /// <value>
        /// The mail footer template.
        /// </value>
        public string MailFooterTemplate { get; set; }

        /// <summary>
        /// Gets or sets the refinancing date
        /// </summary>
        public DateTime RefinancingDate { get; set; }

        /// <summary>
        /// Gets limit get creditmasterId.
        /// </summary>
        /// <value>
        /// The mail footer template.
        /// </value>
        public int LimitGetCreditMasterId { get; set; }

        /// <summary>
        /// Gets or sets the customer status.
        /// </summary>
        /// <value>
        /// The customer status.
        /// </value>
        public string CustomerStatus { get; set; }

        /// <summary>
        /// Gets or sets the UDP call URL.
        /// </summary>
        /// <value>
        /// The UDP call URL.
        /// </value>
        public string UdpCallUrlEndPoint { get; set; }

        /// <summary>
        /// Gets or sets the UDP call URL rute.
        /// </summary>
        /// <value>
        /// The UDP call URL rute.
        /// </value>
        public string UdpCallUrlRute { get; set; }

        /// <summary>
        /// Gets or sets the UDP response call URL.
        /// </summary>
        /// <value>
        /// The UDP response call URL.
        /// </value>
        public string UdpResponseCallUrl { get; set; }

        /// <summary>
        /// Pending AgentAnalysis Result Id
        /// </summary>
        public int PendingAgentAnalysisResultId { get; set; }

        /// <summary>
        ///  Gets or sets Store Risk Level
        /// </summary>
        public bool StoreHasRiskCalculationDefault { get; set; }

        /// <summary>
        ///  Gets or sets template cancellation
        /// </summary>
        public string TemplateCancellation { get; set; }

        /// <summary>
        ///  Gets or sets stores without payment number
        /// </summary>
        public string StoresWithoutPaymentNumber { get; set; }

        /// <summary>
        /// Gets or sets the discounted days.
        /// </summary>
        /// <value>
        /// The discounted days.
        /// </value>
        public int DiscountedDays { get; set; }

        /// <summary>
        /// Gets or sets the pending status.
        /// </summary>
        /// <value>
        /// The pending status.
        /// </value>
        public int PendingStatus { get; set; }

        /// <summary>
        /// Gets or sets the reject due to expiration status.
        /// </summary>
        /// <value>
        /// The reject due to expiration status.
        /// </value>
        public int RejectDueToExpirationStatus { get; set; }

        /// <summary>
        /// Gets or sets the max retry attempts of the send mail notification request
        /// </summary>
        /// <value>
        /// The reject due to expiration status.
        /// </value>
        public int MaxRetryAttemptsMailNotificationRequest { get; set; }

        /// <summary>
        /// Gets or sets the max retry attempts of the send sms notification request
        /// </summary>
        /// <value>
        /// The reject due to expiration status.
        /// </value>
        public int MaxRetryAttemptsSmsNotificationRequest { get; set; }

        /// <summary>
        /// Gets or sets the max retry attempts of the send the create credit notification request
        /// </summary>
        /// <value>
        /// The reject due to expiration status.
        /// </value>
        public int MaxRetryAttemptsCreateCreditNotificationRequest { get; set; }

        /// <summary>
        /// To string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return StringHelper.ToDebugString(this);
        }
    }
}