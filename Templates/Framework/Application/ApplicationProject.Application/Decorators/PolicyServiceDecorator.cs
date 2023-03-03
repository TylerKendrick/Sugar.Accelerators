using Polly;

namespace ApplicationProject.Application
{
    /// <summary>
    /// A decorator class that implements policy-based resilience for Service methods using the IPolicyProvider interface.
    /// </summary>
    class PolicyServiceDecorator : ServiceDecorator
    {
        private readonly IPolicyProvider _provider;

        /// <summary>
        /// Initializes a new instance of the PolicyServiceDecorator class with the specified service and policy provider.
        /// </summary>
        /// <param name="service">The IService to decorate.</param>
        /// <param name="provider">The IPolicyProvider that provides policies to use for resilience.</param>
        public PolicyServiceDecorator(IService service, IPolicyProvider provider)
            : base(service)
        {
            _provider = provider;
        }

        /// <inheritdoc/>
        public async override Task<Person> GetPerson(string name)
        {
            var policy = _provider.ResolveAsync<Person>();
            var result = await policy.ExecuteAndCaptureAsync(
                async () => await base.GetPerson(name));
            return result.Outcome switch 
            {
                OutcomeType.Successful => result.FinalHandledResult,
                OutcomeType.Failure => result.FaultType switch
                {
                    FaultType.ExceptionHandledByThisPolicy => throw result.FinalException,
                    FaultType.ResultHandledByThisPolicy => result.FinalHandledResult,
                    FaultType.UnhandledException => throw new NotSupportedException(),
                    _ => throw new NotSupportedException()
                },
                _ => throw new NotSupportedException()
            };
        }
    }
}
