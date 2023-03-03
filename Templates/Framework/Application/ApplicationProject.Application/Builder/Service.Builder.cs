using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// A builder class for configuring services in the Microsoft.Extensions.DependencyInjection framework.
/// </summary>
public class ServiceBuilder
{
    private readonly IServiceCollection services;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceBuilder"/> class with the given service collection.
    /// </summary>
    /// <param name="services">The service collection to use for configuring services.</param>
    public ServiceBuilder(IServiceCollection services)
        => this.services = services ?? new ServiceCollection();

    private ServiceBuilder Configure<TBuilder, TConcrete>(
        Action<TBuilder> configure,
        Func<TConcrete> factory)
        where TConcrete : class, TBuilder, IBuilder
    {
        var builder = factory();
        configure(builder);
        builder.Apply(services);
        return this;
    }

    /// <summary>
    /// Configures logging services using the specified logging builder.
    /// </summary>
    /// <param name="configureLogging">A delegate that takes a logging builder and configures it.</param>
    /// <returns>The current instance of the <see cref="ServiceBuilder"/> class.</returns>
    public ServiceBuilder ConfigureLogging(Action<ILoggingBuilder> configureLogging)
        => Configure(configureLogging, () => new LoggingBuilder());

    /// <summary>
    /// Configures service policies using the specified policy builder.
    /// </summary>
    /// <param name="configurePolicies">A delegate that takes a service policy builder and configures it.</param>
    /// <returns>The current instance of the <see cref="ServiceBuilder"/> class.</returns>
    public ServiceBuilder ConfigurePolicies(Action<IServicePolicyBuilder> configurePolicies)
        => Configure(configurePolicies, () => new ServicePolicyBuilder());

    /// <summary>
    /// Configures service validators using the specified validator builder.
    /// </summary>
    /// <param name="configureValidators">A delegate that takes a service validator builder and configures it.</param>
    /// <returns>The current instance of the <see cref="ServiceBuilder"/> class.</returns>
    public ServiceBuilder ConfigureValidators(Action<IServiceValidatorBuilder> configureValidators)
        => Configure(configureValidators, () => new ServiceValidatorBuilder());

    /// <summary>
    /// Configures service sanitizers using the specified sanitizer builder.
    /// </summary>
    /// <param name="configureSanitizers">A delegate that takes a service sanitizer builder and configures it.</param>
    /// <returns>The current instance of the <see cref="ServiceBuilder"/> class.</returns>
    public ServiceBuilder ConfigureSanitizers(Action<IServiceSanitizerBuilder> configureSanitizers)
        => Configure(configureSanitizers, () => new ServiceSanitizerBuilder());

    /// <summary>
    /// Configures services using the specified delegate.
    /// </summary>
    /// <param name="configureServices">A delegate that takes a service collection and configures it.</param>
    /// <returns>The current instance of the <see cref="ServiceBuilder"/> class.</returns>
    public ServiceBuilder Configure(Action<IServiceCollection> configureServices)
    {
        configureServices(services);
        return this;
    }

    /// <summary>
    /// Builds an <see cref="IServiceProvider"/> with the configured services.
    /// </summary>
    /// <returns>The built <see cref="IServiceProvider"/>.</returns>
    public IServiceProvider BuildServiceProvider()
        => services.BuildServiceProvider();
}
