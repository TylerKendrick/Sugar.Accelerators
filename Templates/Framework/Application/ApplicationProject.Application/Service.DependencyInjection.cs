using ApplicationProject.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

/// <summary>
/// Provides extension methods to configure the service.
/// </summary>
public static class ServiceDependencyInjectionExtensions
{
    /// <summary>
    /// Configures the dependencies for the service.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureService(
        this IServiceCollection services)
    {
        services.AddTransient<Service>();
        services.AddSingleton<IService>(provider =>
        {
            T Get<T>() where T : class => provider.GetRequiredService<T>();
            var service = Get<Service>();
            var logger = Get<ILogger<IService>>();
            var policyProvider = Get<IPolicyProvider>();
            var sanitizerProvider = Get<ISanitizerProvider>();
            var validationProvider = Get<IValidatorProvider>();
            return new LoggingServiceDecorator(
                new PolicyServiceDecorator(
                    new SanitizerServiceDecorator(
                        new ValidationServiceDecorator(
                            service, validationProvider
                        ), sanitizerProvider),
                        policyProvider),
                    logger);
        });
        return services;
    }
}