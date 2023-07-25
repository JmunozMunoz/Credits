using Coravel;
using credinet.comun.api;
using credinet.comun.models.Credits;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using org.reactivecommons.api;
using org.reactivecommons.api.impl;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.Managment.Services.Customers;
using Sc.Credits.Domain.Managment.Services.Locations;
using Sc.Credits.Domain.Managment.Services.Stores;
using Sc.Credits.Domain.Model.Call.Gateway;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Common.Gateway;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Customers.Gateway;
using Sc.Credits.Domain.Model.Google.Recaptcha;
using Sc.Credits.Domain.Model.Locations.Gateway;
using Sc.Credits.Domain.Model.Refinancings.Gateway;
using Sc.Credits.Domain.Model.Sequences.Gateway;
using Sc.Credits.Domain.Model.Stores.Gateway;
using Sc.Credits.Domain.UseCase.Credits;
using Sc.Credits.Domain.UseCase.Customers;
using Sc.Credits.Domain.UseCase.Stores;
using Sc.Credits.DrivenAdapters.AzureStorage.BlobStore;
using Sc.Credits.DrivenAdapters.ServiceBus.Common;
using Sc.Credits.DrivenAdapters.ServiceBus.Credits;
using Sc.Credits.DrivenAdapters.ServiceBus.Customers;
using Sc.Credits.DrivenAdapters.SqlServer;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Repository;
using Sc.Credits.EntryPoints.Jobs;
using Sc.Credits.Helpers.Commons.Cache;
using Sc.Credits.Helpers.Commons.Logging;
using Sc.Credits.Helpers.Commons.Logging.Gateway;
using Sc.Credits.Helpers.Commons.Messaging;
using Sc.Credits.Helpers.Commons.Messaging.Gateway;
using Sc.Credits.Helpers.Commons.Settings;
using Sc.Credits.Helpers.ObjectsUtils;
using SC.AdministradorLlaves;
using SC.Credits.DrivenAdapter.Http.ReCaptchaValidators;
using SC.Credits.DrivenAdapter.Http.UdpCall;
using System;
using System.Globalization;
using System.Reflection;

namespace Sc.Credits.AppServices.Jobs
{
    /// <summary>
    /// Startup class
    /// </summary>
    public class Startup
    {
        private ILogger<Startup> Logger { get; }

        /// <summary>
        /// Gets the environment host
        /// </summary>
        public IHostingEnvironment EnvironmentHost { get; }

        /// <summary>
        /// Gets the configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="environment"></param>
        /// <param name="logger"></param>
        public Startup(IConfiguration configuration, IHostingEnvironment environment, ILogger<Startup> logger)
        {
            Configuration = configuration;
            EnvironmentHost = environment;
            Logger = logger;
        }

        /// <summary>
        /// Configure Services
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore().SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddJsonFormatters();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddRespuestaApiFactory();
            services.AddVersionedApiExplorer();

            services.Configure<CredinetAppSettings>(Configuration);

            CredinetAppSettings appSettings = Configuration.Get<CredinetAppSettings>();

            string appId = appSettings.AppId;
            string appSecret = appSettings.AppSecret;
            string database = appSettings.DataBaseCrediNetCredits;

            string country = EnvironmentHelper.GetCountryOrDefault(appSettings.DefaultCountry);

            string databaseName = $"{database}_{country}";
            string creditsSqlConnection = GetKeyValue(appSettings.SqlConnectionString, appId, appSecret).Replace(database, databaseName);

            services.AddEntityFrameworkSqlServer()
                   .AddDbContext<CreditsContext>(options =>
                   {
                       options.UseSqlServer(creditsSqlConnection,
                           sqlServerOptionsAction: sqlOptions =>
                           {
                               sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                               sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                               sqlOptions.MigrationsHistoryTable("Migrations", "Configuration");
                           });
                   },
                   ServiceLifetime.Scoped
            );

            services.AddSingleton<ICreditsConnectionFactory>(new CreditsSqlConnectionFactory(creditsSqlConnection));

            string messagingDatabase = appSettings.MessagingDatabaseName;

            string messagingDatabaseName = $"{messagingDatabase}_{country}";
            string messagingSqlConnection = GetKeyValue(appSettings.MessagingSqlConnectionString, appId, appSecret).Replace(messagingDatabase, messagingDatabaseName);

            services.AddSingleton<IMessagingConnectionFactory>(new MessagingSqlConnectionFactory(messagingSqlConnection));

            services.AddSingleton(typeof(ISqlDelegatedHandlers<>), typeof(SqlDapperDelegatedHandlers<>));

            services.AddScoped<ICreditMasterRepository, CreditMasterRepository>();
            services.AddScoped<IAppParametersRepository, AppParametersRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IStoreRepository, StoreRepository>();
            services.AddScoped<IRequestCancelCreditRepository, RequestCancelCreditRepository>();
            services.AddScoped<IRequestCancelPaymentRepository, RequestCancelPaymentRepository>();
            services.AddScoped<IUnapprovedCreditRepository, UnapprovedCreditRepository> ();
            services.AddScoped<IStateRepository, StateRepository>();
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<IMessageLogRepository, MessageLogRepository>();
            services.AddScoped<ICreditRequestAgentAnalysisQueryRepository, CreditRequestAgentAnalysisQueryRepository>();
            services.AddSingleton<ISequenceRepository, SequenceRepository>();
            services.AddSingleton<ICreditRequestAgentAnalysisRepository, CreditRequestAgentAnalysisRepository>();
            services.AddScoped<IRefinancingLogRepository, RefinancingLogRepository>();

            services.AddScoped<ICreditUsesCase, CreditUsesCase>();
            services.AddScoped<ICreditOperationsUseCase, CreditOperationsUseCase>();
            services.AddScoped<ICreditPaymentUsesCase, CreditPaymentUsesCase>();
            services.AddScoped<ICustomerUsesCase, CustomerUsesCase>();
            services.AddScoped<IStoreUseCase, StoreUseCase>();
            services.AddScoped<ICancelUseCase, CancelUseCase>();

            services.AddScoped<ICreditPaymentService, CreditPaymentService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IPartialCancelationCreditService, PartialCancelationCreditService>();
            services.AddScoped<ITotalCancelationCreditService, TotalCancelationCreditService>();
            services.AddScoped<ICancelCreditContextService, CancelCreditContextService>();
            services.AddScoped<ICancelCreditService, CancelCreditService>();
            services.AddScoped<ICancelPaymentService, CancelPaymentService>();
            services.AddScoped<IAppParametersService, AppParametersService>();
            services.AddScoped<ILocationService, LocationService>();
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

            services.AddScoped<IMessagingLogger, MessagingLogger>();

            string serviceBusConn = GetKeyValue(appSettings.ServiceBusConnectionStringRequest, appId, appSecret);

            AddAsyncGateway<CreditLimitResponse>(services, serviceBusConn);
            AddAsyncGateway<MailNotificationRequest>(services, serviceBusConn);
            AddAsyncGateway<SmsNotificationRequest>(services, serviceBusConn);
            AddAsyncGateway<CreateCreditResponse>(services, serviceBusConn);
            AddAsyncGateway<DrivenAdapters.ServiceBus.Model.CreditEvent>(services, serviceBusConn);
            AddAsyncGateway<dynamic>(services, serviceBusConn);

            services.AddScoped<IEventsRepository, EventsAdapter>();
            services.AddScoped<ITokenRepository, TokenAdapter>();
            services.AddScoped<INotificationRepository, NotificationAdapter>();
            services.AddScoped<ICreditNotificationRepository, CreditNotificationAdapter>();
            services.AddScoped<ICreditPaymentEventsRepository, CreditPaymentEventsAdapter>();
            services.AddScoped<ICustomerEventsRepository, CustomerEventsAdapter>();

            services.AddHttpClient<IReCaptchaGateway, ReCaptchaHttpAdapter>();
            services.AddHttpClient<IUdpCallHttpRepository, UdpCallHttpAdapter>();

            string storageKey = appSettings.StorageAccount;
            string storageConn = GetKeyValue(storageKey, appId, appSecret);

            services.AddScoped<IBlobStoreRepository>(provider => new BlobStoreAdapter(storageConn));
         
            services.AddSingleton(typeof(ISettings<>), typeof(Settings<>));
            services.AddSingleton(AppMemoryCache.Current);

            services.HabilitarVesionamiento();

            services.AddTransient<RejectCancellationRequestJob>();
            services.AddScheduler();

            Logger.LogInformation("===============STARTUP=============");
            Logger.LogInformation($"Environment = {EnvironmentHost.EnvironmentName}");
            Logger.LogInformation($"ConnectionString = {creditsSqlConnection}");
            Logger.LogInformation($"MessagingConnectionString = {messagingSqlConnection}");
            Logger.LogInformation($"Country = {country}");
            Logger.LogInformation($"ServiceBusConnection = {serviceBusConn}");
            Logger.LogInformation($"{appSettings}");
            Logger.LogInformation($"Date = {DateTime.UtcNow.ToLocalTime()}");
            Logger.LogInformation("=============END STARTUP==========");
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
        /// Configure
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
         public void Configure(IApplicationBuilder app,
            IHostingEnvironment env)
        {
            CredinetAppSettings appSettings = Configuration.Get<CredinetAppSettings>();

            CultureInfo cultureInfo = string.IsNullOrEmpty(appSettings.CultureInfo) ? CultureInfo.InvariantCulture : new CultureInfo(appSettings.CultureInfo);
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
         
            app.UseStaticFiles();

            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

            IServiceProvider provider = app.ApplicationServices;

            provider.UseScheduler(scheduler =>
            {
                scheduler
                    .Schedule<RejectCancellationRequestJob>()
                .Cron(appSettings.RejectCancellationRequestCron);
            });
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