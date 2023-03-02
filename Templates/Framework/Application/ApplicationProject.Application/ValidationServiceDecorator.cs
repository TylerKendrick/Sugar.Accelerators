using FluentValidation;

namespace ApplicationProject.Application
{
    class ValidationServiceDecorator : ServiceDecorator
    {
        private readonly IValidatorProvider _validatorProvider;

        public ValidationServiceDecorator(
            IService service,
            IValidatorProvider validatorProvider)
            : base(service)
        {
            _validatorProvider = validatorProvider;
        }
        public async override Task<Person> GetPerson(string name)
        {
            var validator = _validatorProvider.Resolve<string>(nameof(name));
            await validator.ValidateAndThrowAsync(name);
            return await base.GetPerson(name);
        }
    }
}
