/// <summary>
/// Defines the methods for building service policies.
/// </summary>
public interface IServicePolicyBuilder
{
    /// <summary>
    /// Adds a retry policy to the service policy configuration.
    /// </summary>
    void AddRetryPolicy();
    
    /// <summary>
    /// Adds a circuit breaker policy to the service policy configuration.
    /// </summary>
    void AddCircuitBreakerPolicy();
}
