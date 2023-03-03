using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Builder for configuring input and output validation for services.
/// </summary>
public class ServiceValidatorBuilder : IServiceValidatorBuilder, IBuilder
{
    /// <summary>
    /// Adds input validation to the builder.
    /// </summary>
    public void AddInputValidation()
    {
        // Add input validation implementation here
    }

    /// <summary>
    /// Adds output validation to the builder.
    /// </summary>
    public void AddOutputValidation()
    {
        // Add output validation implementation here
    }

    /// <summary>
    /// Applies validators to the given service collection.
    /// </summary>
    /// <param name="services">The service collection to apply validators to.</param>
    public void Apply(IServiceCollection services)
    {
        // Apply validators to services here
        services.AddTransient<IServiceValidatorBuilder, ServiceValidatorBuilder>();
    }
}
