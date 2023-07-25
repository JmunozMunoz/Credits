using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Sc.Credits.AppServices.Customers
{
    /// <summary>
    /// Program static class
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main method. AppServices' entry point
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Create web host builder
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("config/appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            return WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
                 .UseIISIntegration()
                 .UseConfiguration(config)
                 .UseSerilog((context, configuration) =>
                    configuration.ReadFrom.Configuration(config)
                        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName))
                 .UseStartup<Startup>();
        }
    }
}