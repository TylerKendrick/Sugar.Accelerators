using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Builds and applies input and output sanitizers to a service collection.
/// </summary>
public class ServiceSanitizerBuilder : IServiceSanitizerBuilder, IBuilder
{
    /// <summary>
    /// Adds input sanitization to the service.
    /// </summary>
    public void AddInputSanitization()
    {
        // Add input sanitization implementation here
    }

    /// <summary>
    /// Adds output sanitization to the service.
    /// </summary>
    public void AddOutputSanitization()
    {
        // Add output sanitization implementation here
    }

    /// <summary>
    /// Applies sanitizers to the specified services.
    /// </summary>
    /// <param name="services">The services to apply sanitizers to.</param>
    public void Apply(IServiceCollection services)
    {
        // Apply sanitizers to services here
        services.AddTransient<IServiceSanitizerBuilder, ServiceSanitizerBuilder>();
    }
}
