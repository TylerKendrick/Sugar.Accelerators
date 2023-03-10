using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

/// <summary>
/// Contains extension methods for configuring an application using <see cref="IHostBuilder"/>.
/// </summary>
public static partial class HostBuilderExtensions
{
    private static IHostBuilder ConfigureApplicationConfiguration(this IHostBuilder hostBuilder)
    {
        string environmentName = 
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? Environments.Development;
        return hostBuilder
            .ConfigureAppConfiguration((context, builder) =>
            {
                // Add support for app secrets
                builder.AddEnvironmentVariables();
                builder.AddJsonFile("appsettings.json");
                builder.AddJsonFile($"appsettings.{environmentName}.json");
                builder.AddUserSecrets<Service>();
            });
    }

    private static IHostBuilder ConfigureApplicationHost(
        this IHostBuilder hostBuilder,
        Action<Microsoft.Extensions.Logging.ILoggingBuilder> configureLogging,
        Action<MediatRServiceConfiguration> configureMediatR,
        Action<IServiceCollection> configure,
        string resourcePath)
        => hostBuilder.ConfigureServices((context, services) =>
        {
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                configureLogging(builder);
            });
            // TODO: Add Automapper
            // TODO: Add Identity
            // TODO: Register Humanizer.
            services.AddMediatR(configureMediatR);
            services.AddLocalization(o => o.ResourcesPath = resourcePath);
            services.AddHealthChecks();

            // TODO: Add file providers
            //services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));

            // TODO: Add caching
            //services.AddMemoryCache();
            configure(services);
        });
}
