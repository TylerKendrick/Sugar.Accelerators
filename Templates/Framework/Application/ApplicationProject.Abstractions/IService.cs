namespace ApplicationProject.Abstractions;

/// <summary>
/// Defines a service that provides access to person information.
/// </summary>
public interface IService
{
    /// <summary>
    /// Gets information for a person with the specified name.
    /// </summary>
    /// <param name="name">The name of the person to retrieve information for.</param>
    /// <returns>
    ///     A <see cref="Task{TResult}"/> representing the asynchronous operation,
    ///     which returns a <see cref="Person"/> object containing the person's information.
    /// </returns>
    protected static Task<Person> GetPerson(IService service, string name)
    {
        var source = new TaskCompletionSource<Person>();
        switch (name)
        {
            case var _ when string.IsNullOrWhiteSpace(name):
                source.SetException(new ArgumentException("Not a valid name."));
                break;
            case "John":
                source.SetResult(new() { Name = "John", Id = 25});
                break;
            case "Error.AggregateException":
                throw new AggregateException();
            case "Task.Cancelled":
                source.SetCanceled();
                break;
            case "Task.LongRunning":
                //Don't set source result. This will cause an infinite wait condition.
                break;
            default:
                throw new InvalidOperationException("An unknown error occurred.");
        }
        return source.Task;
    }
    
    /// <summary>
    /// Gets information for a person with the specified name.
    /// </summary>
    /// <param name="name">The name of the person to retrieve information for.</param>
    /// <returns>
    ///     A <see cref="Task{TResult}"/> representing the asynchronous operation,
    ///     which returns a <see cref="Person"/> object containing the person's information.
    /// </returns>
    Task<Person> GetPerson(string name) => IService.GetPerson(this, name);

}
