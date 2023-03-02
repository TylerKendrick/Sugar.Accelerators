using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Hygiene;
using FluentValidation;

/// <summary>
/// Contains extension methods for configuring an application using <see cref="IHostBuilder"/>.
/// </summary>
public static partial class HostBuilderExtensions
{
    private static IHostBuilder ConfigureApplicationDependencies(
        this IHostBuilder hostBuilder,
        Func<IServiceProvider, IAsyncPolicy<Person>> policyFactory,
        Func<IServiceProvider, ISanitizer<string>> sanitizerFactory,
        Func<IServiceProvider, IValidator<string>> validatorFactory)
        => hostBuilder.ConfigureServices((context, services) =>
        {
            services.AddScoped<IAsyncPolicy<Person>>(policyFactory);
            services.AddScoped<ISanitizer<string>>(sanitizerFactory);
            services.AddScoped<IValidator<string>>(validatorFactory);
            services.AddSingleton<IService, Service>();
        });
}
