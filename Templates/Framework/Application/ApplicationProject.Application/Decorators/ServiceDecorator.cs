namespace ApplicationProject.Application;
/// <summary>
/// Abstract base class for implementing the decorator pattern on the <see cref="IService"/> interface.
/// </summary>
public abstract class ServiceDecorator : IService
{
    private readonly IService _decoratedService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceDecorator"/> class with the specified decorated service.
    /// </summary>
    /// <param name="decoratedService">The decorated service instance.</param>
    protected ServiceDecorator(IService decoratedService)
    {
        _decoratedService = decoratedService ?? throw new ArgumentNullException(nameof(decoratedService));
    }

    /// <inheritdoc/>
    public virtual Task<Person> GetPerson(string name) => _decoratedService.GetPerson(name);
}