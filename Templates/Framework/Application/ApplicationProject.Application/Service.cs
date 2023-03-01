namespace ApplicationProject.Application;

using Polly;
using Hygiene;
using FluentValidation;

/// <summary>
/// A service that provides application logic.
/// </summary>
public class Service : IService
{
    private readonly IAsyncPolicy<Person> _asyncPolicy;
    private readonly ISanitizer<string> _nameSanitizer;
    private readonly IValidator<string> _nameValidator;

    /// <summary>
    /// The default ctor used for Dependency Injection.
    /// </summary>
    /// <param name="asyncPolicy">A policy to determine transient fault handling.</param>
    /// <param name="nameSanitizer">A sanitizer for the name parameter.</param>
    /// <param name="nameValidator">A validator for the name parameter.</param>
    public Service(
        IAsyncPolicy<Person> asyncPolicy,
        ISanitizer<string> nameSanitizer,
        IValidator<string> nameValidator)
    {
        _asyncPolicy = asyncPolicy;
        _nameSanitizer = nameSanitizer;
        _nameValidator = nameValidator;
    }

    async Task<Person> GetPerson(string name) 
    {
        var result = _asyncPolicy.ExecuteAsync(async () => 
        {
            await _nameSanitizer.SanitizeAsync(ref name);
            var validationResult = await _nameValidator.ValidateAsync(name);
            return await IService.GetPerson(this, name);
        });
        return await result;
    }
}