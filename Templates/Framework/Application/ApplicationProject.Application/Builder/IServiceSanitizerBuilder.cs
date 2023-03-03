/// <summary>
/// Represents a builder for configuring input and output data sanitization for a service.
/// </summary>
public interface IServiceSanitizerBuilder
{
    /// <summary>
    /// Configures input data sanitization for the service.
    /// </summary>
    void AddInputSanitization();

    /// <summary>
    /// Configures output data sanitization for the service.
    /// </summary>
    void AddOutputSanitization();
}
