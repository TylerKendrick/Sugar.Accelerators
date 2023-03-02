using Polly;

namespace ApplicationProject.Application
{
    class PolicyServiceDecorator : ServiceDecorator
    {
        private readonly IPolicyProvider _provider;

        public PolicyServiceDecorator(
            IService service,
            IPolicyProvider provider)
            : base(service)
        {
            _provider = provider;
        }
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
