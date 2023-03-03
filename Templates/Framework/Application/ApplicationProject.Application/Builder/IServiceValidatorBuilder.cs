/// <summary>
/// Defines methods to add input validation to a service validator.
/// </summary>
public interface IServiceValidatorBuilder
{
    /// <summary>
    /// Adds input validation to the service validator.
    /// </summary>
    void AddInputValidation();

    /// <summary>
    /// Adds output validation to the service validator.
    /// </summary>
    void AddOutputValidation();
}
