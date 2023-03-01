using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Hygiene;
using FluentValidation;

/// <summary>
/// Contains extension methods for configuring an application using <see cref="IHostBuilder"/>.
/// </summary>
public static class HostBuilderExtensions
{
    private static IHostBuilder ConfigureApplicationDependencies(this IHostBuilder hostBuilder)
        => hostBuilder.ConfigureServices((context, services) =>
        {
            services.AddScoped<IAsyncPolicy<Person>>();
            services.AddScoped<ISanitizer<string>>();
            services.AddScoped<IValidator<string>>();
            services.AddSingleton<IService, Service>();
        });

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
        Action<ILoggingBuilder> configureLogging,
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

    /// <summary>
    /// Configures the application with the specified services and settings.
    /// </summary>
    /// <param name="hostBuilder">The <see cref="IHostBuilder"/> to configure.</param>
    /// <param name="configureLogging">An optional <see cref="Action{T}"/> delegate used to configure logging.</param>
    /// <param name="configureMediatR">An optional <see cref="Action{T}"/> delegate used to configure MediatR.</param>
    /// <param name="configure">An optional <see cref="Action{T}"/> delegate used to configure the service collection.</param>
    /// <param name="resourcePath">Provides a path to resources used for localization.</param>
    /// <returns>The configured <see cref="IHostBuilder"/>.</returns>
    /// <remarks>
    /// This method calls <see cref="ConfigureApplicationConfiguration"/> to configure the application's configuration settings,
    /// <see cref="ConfigureApplicationDependencies"/> to configure the application's service collection, and 
    /// <see cref="ConfigureApplicationHost"/> to configure the application's host.
    /// If no <paramref name="configure"/> delegate is provided, an empty action is used instead.
    /// </remarks>
    public static IHostBuilder ConfigureApplication(
        this IHostBuilder hostBuilder,
        Action<ILoggingBuilder>? configureLogging = default,
        Action<MediatRServiceConfiguration>? configureMediatR = default,
        Action<IServiceCollection>? configure = default,
        string? resourcePath = default)
        => hostBuilder
            .ConfigureApplicationConfiguration()
            .ConfigureApplicationDependencies()
            .ConfigureApplicationHost(
                configureLogging ?? (x => {}),
                configureMediatR ?? (x => {}),
                configure ?? (x => {}),
                resourcePath ?? "Resources");
}
