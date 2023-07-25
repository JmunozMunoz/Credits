namespace Sc.Credits.Helper.Test.Model
{
    using Sc.Credits.Helpers.ObjectsUtils;
    using Sc.Credits.Helpers.ObjectsUtils.ApplicationSettings;

    /// <summary>
    /// Parameter Helper Test
    /// </summary>
    public static class CredinetAppSettingsHelperTest
    {
        /// <summary>
        /// Get Parameters
        /// </summary>
        /// <returns></returns>
        public static CredinetAppSettings GetCredinetAppSettings()
        {
            return new CredinetAppSettings
            {
                TemplatesCacheMinutes = 2,
                TemplatesBlobContainerName = "TemplatesBlobContainerName",
                AmortizationScheduleUrlTemplate = string.Empty,
                StoreAssurancePercentageDefault = 0.1M,
                CompanyName = "Sistecrédito",
                CreateCreditNotificationQueue = "create_credit_notification",
                CreateCreditNotificationTemplateId = "df832f91-ce8f-49d4-8cc5-c22994c73087",
                CreditCustomerMigrationHistoryQueue = string.Empty,
                StoreMonthLimitDefault = 9,
                CreditScCodeQueue = "credit_sccode_update",
                CreditTokenCallRequestQueue = "credit_token_call_request",
                CreditTokenGenerateQueue = "credit_token_generate",
                CreditTokenNotificationTemplateId = "a8f8c84c-185d-4e31-9ff0-0f3655c03bb3",
                CreditTokenValidateQueue = "credit_token_validate",
                CreditTopicName = "credits_events",
                CultureInfo = "es-CO",
                AppId = "8b231cf6-9e8e-423e-95fa-dbbfefc3eeb8",
                AppSecret = "l+vAkJBjD/Y54Ib3UTegeOchxcEy1XYIL8Rm13XuiTc=",
                DataBaseCrediNetCredits = "DBCredits",
                DefaultCountry = "co",
                DefaultSubDomain = "Credit",
                StoreCategoryIdDefault = 1,
                EnableEFLoggerFactory = "false",
                EventsTopicName = "credinet_events",
                MailFrom = "noreply@sistecredito.co",
                MailFromName = "Sistecrédito",
                MailMessagesQueue = "notifications_send_mail",
                ParametersCacheMilliseconds = 1,
                PaymentFutureSecondsLimit = "5",
                PayNotificationMailTemplate = "6238b3ec-09a5-4382-8237-d12739735f82",
                PdfBlobContainerName = "pdf",
                PromissoryNotePath = "promissorynotes",
                QueryCredits = "credits_query",
                ServiceBusConnectionStringRequest = string.Empty,
                SmsCurrencyCode = "%24",
                SmsQueue = "notifications_send_sms",
                SqlConnectionString = string.Empty,
                StorageAccount = string.Empty,
                StoreQueueNameSubscription = "store_credits_register",
                CreditExtraValuesQueue = "creditextravalues_legacy_credits",
                UrlSmsNotificationPay = "http://dev-url.credinet.co/bjf",
                RefinancingStoreId = "5cc243b284bc9f7cf4495c21",
                RefinancingSourcesAllowed = "5",
                StoreSendTokenMailDefault = true,
                StoreSendTokenSmsDefault = true,
                UserToRejectCancellationRequests = "AgenteAutomatico",
                UserIdToRejectCancellationRequests = "607ef8206ce5c499c0ed6297",
                Validation = new ValidationSettings() { MinimumCreditValue = 0, MinimumFrequency = 0, MinimumMonths = 0, SimulationMinimumCreditValue = 0 },
                GenericStore = "617adfc1ea275100014c022f",
                LimitGetCreditMasterId = 10,
                StoresWithoutPaymentNumber= "617adfc1ea275100014c022f"
            };
        }
    }
}