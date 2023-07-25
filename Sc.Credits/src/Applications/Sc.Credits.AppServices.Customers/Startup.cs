using credinet.comun.models.Credits;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using org.reactivecommons.api;
using org.reactivecommons.api.impl;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.Managment.Services.Customers;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Common.Gateway;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Customers.Gateway;
using Sc.Credits.Domain.Model.Refinancings.Gateway;
using Sc.Credits.Domain.UseCase.Customers;
using Sc.Credits.DrivenAdapters.AzureStorage.BlobStore;
using Sc.Credits.DrivenAdapters.ServiceBus.Common;
using Sc.Credits.DrivenAdapters.ServiceBus.Credits;
using Sc.Credits.DrivenAdapters.ServiceBus.Customers;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Repository;
using Sc.Credits.EntryPoints.ServicesBus.Credits;
using Sc.Credits.Helpers.Commons.Cache;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.Commons.Logging.Gateway;
using Sc.Credits.Helpers.Commons.Messaging;
using Sc.Credits.Helpers.Commons.Messaging.Gateway;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using SC.AdministradorLlaves;
using System;
using System.Globalization;

namespace Sc.Credits.AppServices.Customers
{
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// New startup
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="environment"></param>
        /// <param name="logger"></param>
        public Startup(IConfiguration configuration,
            IHostingEnvironment environment,
            ILogger<Startup> logger)
        {
            Configuration = configuration;
            EnvironmentHost = environment;
            Logger = logger;
        }

        /// <summary>
        /// Configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Environment host
        /// </summary>
        public IHostingEnvironment EnvironmentHost { get; }

        /// <summary>
        /// Logger
        /// </summary>
        private ILogger<Startup> Logger { get; }

        /// <summary>
        /// Configure services
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CredinetAppSettings>(Configuration);

            CredinetAppSettings appSettings = Configuration.Get<CredinetAppSettings>();

            string appId = appSettings.AppId;
            string appSecret = appSettings.AppSecret;
            string database = appSettings.DataBaseCrediNetCredits;

            string country = EnvironmentHelper.GetCountryOrDefault(appSettings.DefaultCountry);

            string databaseName = $"{database}_{country}";
            string creditsSqlConnection = GetKeyValue(appSettings.SqlConnectionString, appId, appSecret)
                .Replace(database, databaseName);

            services.AddSingleton<ICreditsConnectionFactory>(new CreditsSqlConnectionFactory(creditsSqlConnection));

            string messagingDatabase = appSettings.MessagingDatabaseName;

            string messagingDatabaseName = $"{messagingDatabase}_{country}";
            string messagingSqlConnection = GetKeyValue(appSettings.MessagingSqlConnectionString, appId, appSecret)
                .Replace(messagingDatabase, messagingDatabaseName);

            services.AddSingleton<IMessagingConnectionFactory>(new MessagingSqlConnectionFactory(messagingSqlConnection));

            services.AddSingleton(typeof(ISqlDelegatedHandlers<>), typeof(SqlDapperDelegatedHandlers<>));

            services.AddScoped<ICustomerUsesCase, CustomerUsesCase>();

            services.AddScoped<ICreditMasterRepository, CreditMasterRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IRequestCancelPaymentRepository, RequestCancelPaymentRepository>();
            services.AddScoped<IAppParametersRepository, AppParametersRepository>();
            services.AddScoped<IMessageLogRepository, MessageLogRepository>();
            services.AddScoped<ICreditRequestAgentAnalysisQueryRepository, CreditRequestAgentAnalysisQueryRepository>();

            services.AddScoped<ICreditCustomerService, CreditCustomerService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IAppParametersService, AppParametersService>();
            services.AddScoped<ICustomerEventsRepository, CustomerEventsAdapter>();
            services.AddScoped<ITemplatesService, TemplatesService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IStorageService, StorageService>();
            services.AddScoped<ICommons, Commons>();
            services.AddScoped<ICreditCommonsService, CreditCommonsService>();
            services.AddScoped<ICreditRequestAgentAnalysisService, CreditRequestAgentAnalysisService>();
            services.AddScoped<CreditCommons>();
            services.AddScoped<PaymentCommons>();
            services.AddScoped<CancelCommons>();
            services.AddScoped(typeof(ILoggerService<>), typeof(LoggerService<>));

            services.AddSingleton(AppMemoryCache.Current);

            string serviceBusConn = GetKeyValue(appSettings.ServiceBusConnectionStringRequest, appId, appSecret);

            AddAsyncGateway<CreditCustomerMigrationHistoryRequest>(services, serviceBusConn);
            AddAsyncGateway<CreditLimitResponse>(services, serviceBusConn);
            AddAsyncGateway<MailNotificationRequest>(services, serviceBusConn);
            AddAsyncGateway<SmsNotificationRequest>(services, serviceBusConn);
            AddAsyncGateway<CreateCreditResponse>(services, serviceBusConn);
            AddAsyncGateway<DrivenAdapters.ServiceBus.Model.CreditEvent>(services, serviceBusConn);
            AddAsyncGateway<dynamic>(services, serviceBusConn);

            services.AddScoped<IEventsRepository, EventsAdapter>();
            services.AddScoped<INotificationRepository, NotificationAdapter>();
            services.AddScoped<ICreditNotificationRepository, CreditNotificationAdapter>();
            services.AddScoped<IMessagingLogger, MessagingLogger>();

            string storageKey = appSettings.StorageAccount;
            string storageConn = GetKeyValue(storageKey, appId, appSecret);

            services.AddScoped<IBlobStoreRepository>(provider => new BlobStoreAdapter(storageConn));

            services.AddScoped<ICreditCustomerCommandSubscription, CreditCustomerCommandSubscription>();

            services.AddSingleton(typeof(ISettings<>), typeof(Settings<>));

            Logger.LogInformation("===============STARTUP=============");
            Logger.LogInformation($"Environment = {EnvironmentHost.EnvironmentName}");
            Logger.LogInformation($"ConnectionString = {creditsSqlConnection}");
            Logger.LogInformation($"Country = {country}");
            Logger.LogInformation($"ServiceBusConnection = {serviceBusConn}");
            Logger.LogInformation($"{appSettings}");
            Logger.LogInformation($"Date = {DateTime.UtcNow.ToLocalTime()}");
            Logger.LogInformation("=============END STARTUP==========");
        }

        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="creditCustomerCommandSubscription"></param>
        public void Configure(ICreditCustomerCommandSubscription creditCustomerCommandSubscription)
        {
            CredinetAppSettings appSettings = Configuration.Get<CredinetAppSettings>();

            CultureInfo cultureInfo = string.IsNullOrEmpty(appSettings.CultureInfo) ? CultureInfo.InvariantCulture : new CultureInfo(appSettings.CultureInfo);
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            creditCustomerCommandSubscription.SubscribeAsync();
        }

        /// <summary>
        /// Get key value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public string GetKeyValue(string key, string appId, string appSecret)
        {
            ScAdministradorLlaves admLlaves = new ScAdministradorLlaves(appId, appSecret);
            return admLlaves.ObtenerLlave(key).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Add gateway
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="services"></param>
        /// <param name="serviceBusConn"></param>
        private static void AddAsyncGateway<TEntity>(IServiceCollection services, string serviceBusConn) =>
            services.AddSingleton<IDirectAsyncGateway<TEntity>>(new DirectAsyncGatewayServiceBus<TEntity>(serviceBusConn));
    }
}