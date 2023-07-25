using credinet.comun.api;
using credinet.comun.api.Swagger.Extensions;
using credinet.comun.models.Credits;
using credinet.comun.models.Study;
using credinet.exception.middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using org.reactivecommons.api;
using org.reactivecommons.api.impl;
using Polly;
using Polly.Extensions.Http;
using Sc.Credits.Applications.AppServices.Infrastructure;
using Sc.Credits.Domain.Managment.Services.Common;
using Sc.Credits.Domain.Managment.Services.Credits;
using Sc.Credits.Domain.Managment.Services.Customers;
using Sc.Credits.Domain.Managment.Services.Locations;
using Sc.Credits.Domain.Managment.Services.Refinancings;
using Sc.Credits.Domain.Managment.Services.Simulator;
using Sc.Credits.Domain.Managment.Services.Stores;
using Sc.Credits.Domain.Model.Call.Gateway;
using Sc.Credits.Domain.Model.Common;
using Sc.Credits.Domain.Model.Common.Gateway;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Domain.Model.Credits.Gateway;
using Sc.Credits.Domain.Model.Customers;
using Sc.Credits.Domain.Model.Customers.Gateway;
using Sc.Credits.Domain.Model.Google.Recaptcha;
using Sc.Credits.Domain.Model.Locations.Gateway;
using Sc.Credits.Domain.Model.Refinancings.Gateway;
using Sc.Credits.Domain.Model.ReportTemplates.Gateway;
using Sc.Credits.Domain.Model.Sequences.Gateway;
using Sc.Credits.Domain.Model.Stores;
using Sc.Credits.Domain.Model.Stores.Gateway;
using Sc.Credits.Domain.UseCase.Credits;
using Sc.Credits.Domain.UseCase.Customers;
using Sc.Credits.Domain.UseCase.Refinancings;
using Sc.Credits.Domain.UseCase.Stores;
using Sc.Credits.DrivenAdapters.Autentic;
using Sc.Credits.DrivenAdapters.AzureStorage.BlobStore;
using Sc.Credits.DrivenAdapters.ServiceBus.Common;
using Sc.Credits.DrivenAdapters.ServiceBus.Credits;
using Sc.Credits.DrivenAdapters.ServiceBus.Customers;
using Sc.Credits.DrivenAdapters.SqlServer;
using Sc.Credits.DrivenAdapters.SqlServer.Connection;
using Sc.Credits.DrivenAdapters.SqlServer.Queries;
using Sc.Credits.DrivenAdapters.SqlServer.Repository;
using Sc.Credits.EntryPoints.ServicesBus.Credits;
using Sc.Credits.EntryPoints.ServicesBus.Customer;
using Sc.Credits.EntryPoints.ServicesBus.Model;
using Sc.Credits.EntryPoints.ServicesBus.Store;
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
using SC.Customer.DrivenAdapter.Http.ReportTemplates;
using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Wkhtmltopdf.NetCore;

namespace Sc.Credits.AppServices
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
            string subDomain = EnvironmentHelper.GetSubDomainOrDefault(appSettings.DefaultSubDomain);

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
            services.AddScoped<IUnapprovedCreditRepository, UnapprovedCreditRepository>();
            services.AddScoped<IStateRepository, StateRepository>();
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<IRefinancingRepository, RefinancingRepository>();
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
            services.AddScoped<IRefinancingUsesCase, RefinancingUsesCase>();

            services.AddScoped<ICreditService, CreditService>();
            services.AddScoped<ISimulatorService, SimulatorService>();
            services.AddScoped<ICreditPaymentService, CreditPaymentService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<ITemplatesService, TemplatesService>();
            services.AddScoped<IPartialCancelationCreditService, PartialCancelationCreditService>();
            services.AddScoped<ITotalCancelationCreditService, TotalCancelationCreditService>();
            services.AddScoped<ICancelCreditContextService, CancelCreditContextService>();
            services.AddScoped<ICancelCreditService, CancelCreditService>();
            services.AddScoped<ICancelPaymentService, CancelPaymentService>();
            services.AddScoped<IAppParametersService, AppParametersService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IRefinancingService, RefinancingService>();
            services.AddScoped<ISignatureService, SignatureService>();
            services.AddScoped<ITemplatesService, TemplatesService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IStorageService, StorageService>();
            services.AddScoped<ICommons, Commons>();
            services.AddScoped<ICreditCommonsService, CreditCommonsService>();
            services.AddScoped<ISimulatorCommonService, SimulatorCommonService>();
            services.AddScoped<ICreditRequestAgentAnalysisService, CreditRequestAgentAnalysisService>();
            services.AddScoped<CreditCommons>();
            services.AddScoped<SimulatorCommons>();
            services.AddScoped<PaymentCommons>();
            services.AddScoped<CancelCommons>();
            services.AddScoped(typeof(ILoggerService<>), typeof(LoggerService<>));

            services.AddScoped<IMessagingLogger, MessagingLogger>();

            string serviceBusConn = GetKeyValue(appSettings.ServiceBusConnectionStringRequest, appId, appSecret);

            AddAsyncGateway<StoreRequest>(services, serviceBusConn);
            AddAsyncGateway<CreditScCodeRequest>(services, serviceBusConn);
            AddAsyncGateway<CustomerRequest>(services, serviceBusConn);
            AddAsyncGateway<CreateCreditResponse>(services, serviceBusConn);
            AddAsyncGateway<ActiveCreditsRequest>(services, serviceBusConn);
            AddAsyncGateway<PayCreditsRequest>(services, serviceBusConn);
            AddAsyncGateway<ChargesUpdatedPaymentPlanValueRequest>(services, serviceBusConn);
            AddAsyncGateway<CalculatedQuery>(services, serviceBusConn);
            AddAsyncGateway<CreditLimitResponse>(services, serviceBusConn);
            AddAsyncGateway<StudyResponse>(services, serviceBusConn);
            AddAsyncGateway<MailNotificationRequest>(services, serviceBusConn);
            AddAsyncGateway<SmsNotificationRequest>(services, serviceBusConn);
            AddAsyncGateway<CreateCreditResponse>(services, serviceBusConn);
            AddAsyncGateway<DrivenAdapters.ServiceBus.Model.CreditEvent>(services, serviceBusConn);
            AddAsyncGateway<dynamic>(services, serviceBusConn);

            services.AddHttpClient<IReportTemplatesGateway, ReportTemplatesHttpAdapter>();
            services.AddHttpClient<IReCaptchaGateway, ReCaptchaHttpAdapter>();
            services.AddHttpClient<IUdpCallHttpRepository, UdpCallHttpAdapter>();

            services.AddScoped<IEventsRepository, EventsAdapter>();
            services.AddScoped<ITokenRepository, TokenAdapter>();
            services.AddScoped<IRiskLevelRepository, RiskLevelAdapter>();
            services.AddScoped<ISimulationRecordsRepository, SimulationRecordsAdapter>();
            services.AddScoped<INotificationRepository, NotificationAdapter>();
            services.AddScoped<ICreditNotificationRepository, CreditNotificationAdapter>();
            services.AddScoped<ICreditPaymentEventsRepository, CreditPaymentEventsAdapter>();
            services.AddScoped<ICustomerEventsRepository, CustomerEventsAdapter>();
            services.AddHttpClient<IAutenticRepository, AutenticAdapter>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(3))
                .AddPolicyHandler(HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(3, retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

            services.AddScoped<ICustomerUpdateEventSubscription, CustomerUpdateEventSubscription>();
            services.AddScoped<ICustomerStatusUpdateEventSubscription, CustomerStatusUpdateEventSubscription>();
            services.AddScoped<ICustomerUpdateMailMobileEventSubscription, CustomerUpdateMailMobileEventSubscription>();
            services.AddScoped<ICustomerUpdateMigrationEventSubscription, CustomerUpdateMigrationEventSubscription>();
            services.AddScoped<ICustomerUpdateStudyEventSubscription, CustomerUpdateStudyEventSubscription>();
            services.AddScoped<ICustomerCreditLimitUpdateEventSubscription, CustomerCreditLimitUpdateEventSubscription>();
            services.AddScoped<IStoreCommandSubscription, StoreCommandSubscription>();
            services.AddScoped<ICreditCommandSubscription, CreditCommandSubscription>();
            services.AddScoped<IActiveCreditsCommandSubscription, ActiveCreditsCommandSubscription>();
            services.AddScoped<IPayCreditsCommandSubscription, PayCreditsCommandSubscription>();
            services.AddScoped<ICreditCreateCommandSubscription, CreditCreateCommandSubscription>();
            services.AddScoped<ICreditQueryCommandSubscription, CreditQueryCommandSubscription>();

            string storageKey = appSettings.StorageAccount;
            string storageConn = GetKeyValue(storageKey, appId, appSecret);

            services.AddScoped<IBlobStoreRepository>(provider => new BlobStoreAdapter(storageConn));

            services.AddTransient<CustomerSubscriptions>();
            services.AddTransient<CreditsSubscriptions>();
            services.AddTransient<CreditsPaymentsSubscription>();

            services.AddSingleton(typeof(ISettings<>), typeof(Settings<>));
            services.AddSingleton(AppMemoryCache.Current);

            services.AddWkhtmltopdf("Infrastructure/HtmlToPdf");

            ControllerManager.Configure(services, EnvironmentHost, subDomain);

            services.ConfigurarSwaggerConVersiones(EnvironmentHost,
                PlatformServices.Default.Application.ApplicationBasePath,
                new string[]
                {
                    "Sc.Credits.EntryPoints.ReactiveWeb.xml"
                });

            services.HabilitarVesionamiento();

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
        /// <param name="apiVersionDescriptionProvider"></param>
        /// <param name="storeSubscription"></param>
        /// <param name="customerSubscriptions"></param>
        /// <param name="creditsSubscriptions"></param>
        /// <param name="creditsPaymentsSubscriptions"></param>
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            IApiVersionDescriptionProvider apiVersionDescriptionProvider,
            IStoreCommandSubscription storeSubscription,
            CustomerSubscriptions customerSubscriptions,
            CreditsSubscriptions creditsSubscriptions,
            CreditsPaymentsSubscription creditsPaymentsSubscriptions)
        {
            CredinetAppSettings appSettings = Configuration.Get<CredinetAppSettings>();

            CultureInfo cultureInfo = string.IsNullOrEmpty(appSettings.CultureInfo) ? CultureInfo.InvariantCulture : new CultureInfo(appSettings.CultureInfo);
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            Task.WaitAll(customerSubscriptions.SubscribeAllAsync(),
                creditsSubscriptions.SubscribeAllAsync(),
                creditsPaymentsSubscriptions.SubscribeAllAsync(),
                storeSubscription.SubscribeAsync());

            app.UseStaticFiles();

            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger((c) =>
                {
                    c.PreSerializeFilters.Add((swaggerDoc, httpRequest) => { swaggerDoc.Host = httpRequest.Host.Value; });
                });
                app.UseSwaggerUI(
                options =>
                {
                    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"../swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant());
                    }
                    options.InjectStylesheet($"../swagger.{env.EnvironmentName}.css");
                });
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.ConfigureExceptionHandler();
            app.UseHttpsRedirection();
            app.UseAmbienteHeaderMiddleware();
            app.UseOrigenHeaderMiddleware();
            app.UseMvc();
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