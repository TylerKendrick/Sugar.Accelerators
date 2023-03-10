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
public static partial class HostBuilderExtensions
{
    /// <summary>
    /// Configures the application with the specified services and settings.
    /// </summary>
    /// <param name="hostBuilder">The <see cref="IHostBuilder"/> to configure.</param>
    /// <param name="policyFactory">Provides an instance of a policy.</param>
    /// <param name="sanitizerFactory">Provides an instance of a sanitizer.</param>
    /// <param name="validatorFactory">Provides an instance of a validator.</param>
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
        Func<IServiceProvider, IAsyncPolicy<Person>> policyFactory,
        Func<IServiceProvider, ISanitizer<string>> sanitizerFactory,
        Func<IServiceProvider, IValidator<string>> validatorFactory,
        Action<Microsoft.Extensions.Logging.ILoggingBuilder>? configureLogging = default,
        Action<MediatRServiceConfiguration>? configureMediatR = default,
        Action<IServiceCollection>? configure = default,
        string? resourcePath = default)
        => hostBuilder
            .ConfigureApplicationConfiguration()
            .ConfigureApplicationDependencies(
                policyFactory,
                sanitizerFactory,
                validatorFactory)
            .ConfigureApplicationHost(
                configureLogging ?? (x => {}),
                configureMediatR ?? (x => {}),
                configure ?? (x => {}),
                resourcePath ?? "Resources");
}
