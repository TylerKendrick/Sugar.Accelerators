using Microsoft.Extensions.Logging;

namespace ApplicationProject.Application
{
    /// <summary>
    /// Service decorator that logs information about retrieved persons.
    /// </summary>
    public class LoggingServiceDecorator : ServiceDecorator
    {
        private readonly ILogger<IService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingServiceDecorator"/> class with the specified decorated service.
        /// </summary>
        /// <param name="decoratedService">The decorated service instance.</param>
        /// <param name="logger">The logger to wrap behavior</param>
        public LoggingServiceDecorator(
            IService decoratedService,
            ILogger<IService> logger)
            : base(decoratedService)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task<Person> GetPerson(string name)
        {
            _logger.LogInformation($"Getting person with name '{name}'");
            var person = await base.GetPerson(name);
            _logger.LogInformation($"Retrieved person with name '{person.Name}'");
            return person;
        }
    }
}
