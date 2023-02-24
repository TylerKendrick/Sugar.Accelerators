
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Moq.Language.Flow;
using System.Reflection;

namespace Sugar.Accelerators.Core.Testing.NUnit;

/// <summary>
/// A generic test fixture base class for configuring dependencies and setting up mocks.
/// </summary>
/// <typeparam name="T">The type of the class being tested.</typeparam>
[TestFixture]
public abstract class TestFixture<T>
    where T: class
{
    private MockRepository _repository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
    private readonly Lazy<T> _lazyConcern;
    private readonly IServiceCollection _services;

    /// <summary>
    /// The instance of the class being tested, lazily initialized.
    /// </summary>
    protected T Concern => _lazyConcern.Value;

    /// <summary>
    /// Initializes a new instance of the TestFixture class.
    /// </summary>
    protected TestFixture() 
    {
        _services = new ServiceCollection();
        _lazyConcern = new(() => Allocate(_services.BuildServiceProvider()));
    }

    /// <summary>
    /// Allocates an instance of the class being tested using the provided service provider.
    /// </summary>
    /// <param name="services">The service provider to use for dependency injection.</param>
    /// <returns>The instance of the class being tested.</returns>
    protected virtual T Allocate(IServiceProvider services) => services.GetRequiredService<T>();


    /// <summary>
    /// Sets up the dependencies and mocks for the test before each test method is run.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _repository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
        SetupRegistrations(_services, _repository);
    }

    /// <summary>
    /// Verifies all mocks were used correctly after each test method is run.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        _repository.VerifyAll();
    }

    /// <summary>
    /// Sets up the dependencies and mocks for the test.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="repository">The mock repository to use for mocking dependencies.</param>
    protected virtual void SetupRegistrations(IServiceCollection services, MockRepository repository)
    {
        Exception Ex(string message) => new(message);
        var empty = Enumerable.Empty<Type>();
        var @params = typeof(T)
            .GetConstructors().Select(c => c.GetParameters().Select(x => x.ParameterType))
            .OrderByDescending(x => x.Count())
            .FirstOrDefault() ?? throw Ex("No DI constructor exposed.");
        var createMethod = typeof(MockRepository)
            .GetMethods()
            .Where(x => x.Name == nameof(MockRepository.Create))
            .Where(x => x.GetParameters().Count() == 0)
            .FirstOrDefault() ?? throw Ex("Create method not found.");
        Console.WriteLine("createMethodFound");

        var mockInstanceInfo = typeof(Moq.Mock)
            .GetProperties()
            .Where(x => x.Name == nameof(Moq.Mock.Object))
            .FirstOrDefault() ?? throw Ex("Object property not found");
        foreach(Type param in @params)
        {
            var boundArg = createMethod!.MakeGenericMethod(param);
            var arg = boundArg.Invoke(repository, null) ?? throw Ex("invoke failed.");
            var mockInstance = mockInstanceInfo.GetValue(arg) ?? throw Ex("Object was null.");
            Console.WriteLine($"{param} resolved as {arg.GetType()} with {mockInstance.GetType()}");
            services.AddTransient(param, x => mockInstance);
        }
        services.AddSingleton<T>();
    }

    /// <summary>
    /// Gets the list of mocks created by the mock repository.
    /// </summary>
    protected IEnumerable<Mock> Mocks => typeof(MockRepository)
        .GetProperty("Mocks", BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance)
        !.GetValue(_repository) as IEnumerable<Mock>
        ?? throw new Exception("Mocks list not found...");

    /// <summary>
    /// Finds a mock of the specified type, or creates one if it doesn't exist.
    /// </summary>
    /// <typeparam name="TMock">The type of the mock to find or create.</typeparam>
    /// <returns>The found or created mock.</returns>
    protected Mock<TMock> FindMock<TMock>()
        where TMock: class => this.Mocks
        .FirstOrDefault(x => x is Mock<TMock>) as Mock<TMock>
        ?? _repository.Create<TMock>();


    /// <summary>
    /// Sets up a mock method with the specified parameters.
    /// </summary>
    /// <typeparam name="TMock">The type of the mock to set up.</typeparam>
    /// <param name="setup">The expression representing the method to set up.</param>
    /// <param name="args">The arguments to use for the method call.</param>
    /// <returns>The setup object for further configuration.</returns>
    protected ISetup<TMock> Mock<TMock>(
        Expression<Action<TMock>> setup, params object[] args)
        where TMock : class => FindMock<TMock>().Setup(setup);

    /// <summary>
    /// Sets up a mock method with the specified parameters.
    /// </summary>
    /// <typeparam name="TMock">The type of the mock to set up.</typeparam>
    /// <typeparam name="TReturn">The return type of the method being set up.</typeparam>
    /// <param name="setup">The expression representing the method to set up.</param>
    /// <param name="args">The arguments to use for the method call.</param>
    /// <returns>The setup object for further configuration.</returns>
    protected ISetup<TMock, TReturn> Mock<TMock, TReturn>(
        Expression<Func<TMock, TReturn>> setup, params object[] args)
        where TMock : class => FindMock<TMock>().Setup(setup);
}