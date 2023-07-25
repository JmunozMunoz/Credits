using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Sc.Credits.EntryPoints.ReactiveWeb.Controllers.V1;
using System;

namespace Sc.Credits.Applications.AppServices.Infrastructure
{
    /// <summary>
    /// Controller manager
    /// </summary>
    public static class ControllerManager
    {
        private const string LOCAL = "Local";

        /// <summary>
        /// Subdomains
        /// </summary>
        public static class Subdomains
        {
            public const string ALL = "All";
            public const string CREDIT = "Credit";
            public const string PAYMENT = "Payment";
            public const string CANCEL = "Cancel";
            public const string REFINANCING = "Refinancing";
            public const string CreditSimulator = "CreditSimulator";
        }

        /// <summary>
        /// Configure controllers
        /// </summary>
        /// <param name="services"></param>
        /// <param name="hostingEnvironment"></param>
        /// <param name="subDomain"></param>
        public static void Configure(IServiceCollection services, IHostingEnvironment hostingEnvironment, string subDomain)
        {
            if (hostingEnvironment.IsEnvironment(LOCAL) || subDomain == Subdomains.ALL)
            {
                Register(services,
                    typeof(CreditController),
                    typeof(EntryPoints.ReactiveWeb.Controllers.V2.CreditController),
                    typeof(PaymentController),
                    typeof(CancelController),
                    typeof(RefinancingController),
                    typeof(CreditSimulatorController));
            }
            else
            {
                if (subDomain == Subdomains.CREDIT)
                {
                    Register(services,
                        typeof(CreditController),
                        typeof(EntryPoints.ReactiveWeb.Controllers.V2.CreditController));
                }

                if (subDomain == Subdomains.PAYMENT)
                {
                    Register(services, typeof(PaymentController));
                }

                if (subDomain == Subdomains.CANCEL)
                {
                    Register(services, typeof(CancelController));
                }

                if (subDomain == Subdomains.REFINANCING)
                {
                    Register(services, typeof(RefinancingController));
                }
                if (subDomain == Subdomains.CreditSimulator)
                {
                    Register(services, typeof(CreditSimulatorController));
                }
            }
        }

        /// <summary>
        /// Register controllers
        /// </summary>
        /// <param name="services"></param>
        /// <param name="types"></param>
        private static void Register(IServiceCollection services, params Type[] types)
        {
            foreach (var type in types)
            {
                services.AddTransient(type);
            }

            services.AddSingleton<IControllerActivator>(new CustomControllerActivator(services, types));
        }
    }
}