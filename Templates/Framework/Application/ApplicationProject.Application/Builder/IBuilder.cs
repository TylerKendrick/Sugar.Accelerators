using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Represents a builder that can apply services to an instance of <see cref="IServiceCollection"/>.
/// </summary>
internal interface IBuilder
{
    /// <summary>
    /// Applies services to an instance of <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance to apply services to.</param>
    void Apply(IServiceCollection services);
}
