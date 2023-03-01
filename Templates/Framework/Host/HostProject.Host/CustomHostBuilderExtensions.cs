using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;

public static class CustomHostBuilderExtensions
{
    public static IHostBuilder ConfigureApplication(this IHostBuilder hostBuilder, Action<IApplicationBuilder> configure)
    {
        return hostBuilder
            .ConfigureAppConfiguration((context, configBuilder) =>
            {
                // Add support for app secrets
                configBuilder.AddUserSecrets<Program>();
            })
            .ConfigureLogging(logging =>
            {
                // Configure logging
                logging.ClearProviders();
                logging.AddConsole();
            })
            .ConfigureServices((context, services) =>
            {
                // Add caching
                services.AddMemoryCache();

                // Add localization
                services.AddLocalization(options =>
                {
                    options.ResourcesPath = "Resources";
                });

                // Add health checks
                services.AddHealthChecks();

                // Add file providers
                services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));
                
                // Configure the application
                var appBuilder = new ApplicationBuilder(services.BuildServiceProvider());
                configure(appBuilder);
            });
    }
}
