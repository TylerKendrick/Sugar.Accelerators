using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Represents a builder for configuring logging services.
/// </summary>
public class LoggingBuilder : ILoggingBuilder, IBuilder
{
    /// <summary>
    /// Applies the logging builder configuration to the specified services collection.
    /// </summary>
    /// <param name="services">The services collection to configure.</param>
    public void Apply(IServiceCollection services)
    {
        services.AddTransient<ILoggingBuilder, LoggingBuilder>();
    }
}
