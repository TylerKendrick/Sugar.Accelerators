using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Represents a builder used to configure service policies.
/// </summary>
public class ServicePolicyBuilder : IServicePolicyBuilder, IBuilder
{
    /// <summary>
    /// Adds a retry policy to the service configuration.
    /// </summary>
    public void AddRetryPolicy()
    {
        // Add retry policy implementation here
    }

    /// <summary>
    /// Adds a circuit breaker policy to the service configuration.
    /// </summary>
    public void AddCircuitBreakerPolicy()
    {
        // Add circuit breaker policy implementation here
    }

    /// <summary>
    /// Applies the configured service policies to the specified service collection.
    /// </summary>
    /// <param name="services">The service collection to apply the policies to.</param>
    public void Apply(IServiceCollection services)
    {
        services.AddTransient<IServicePolicyBuilder, ServicePolicyBuilder>();
    }
}
