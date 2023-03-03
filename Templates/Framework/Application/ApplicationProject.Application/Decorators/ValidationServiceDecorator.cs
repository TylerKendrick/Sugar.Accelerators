using FluentValidation;

namespace ApplicationProject.Application
{
    /// <summary>
    /// A decorator class that provides validation to a service instance.
    /// </summary>
    class ValidationServiceDecorator : ServiceDecorator
    {
        private readonly IValidatorProvider _validatorProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationServiceDecorator"/> class with a service instance and a validator provider.
        /// </summary>
        /// <param name="service">The service instance to decorate.</param>
        /// <param name="validatorProvider">The validator provider to use for validation.</param>
        public ValidationServiceDecorator(
            IService service,
            IValidatorProvider validatorProvider)
            : base(service)
        {
            _validatorProvider = validatorProvider;
        }

        /// <summary>
        /// Gets a person by name with validation.
        /// </summary>
        /// <param name="name">The name of the person to retrieve.</param>
        /// <returns>The person with the given name.</returns>
        /// <exception cref="ValidationException">Thrown if the name is invalid.</exception>
        public async override Task<Person> GetPerson(string name)
        {
            var validator = _validatorProvider.Resolve<string>(nameof(name));
            await validator.ValidateAndThrowAsync(name);
            return await base.GetPerson(name);
        }
    }
}
