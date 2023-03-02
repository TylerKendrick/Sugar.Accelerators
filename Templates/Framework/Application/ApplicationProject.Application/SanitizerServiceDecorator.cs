namespace ApplicationProject.Application
{
    class SanitizerServiceDecorator : ServiceDecorator
    {
        private readonly ISanitizerProvider _sanitizerProvider;

        public SanitizerServiceDecorator(
            IService service,
            ISanitizerProvider sanitizerProvider)
            : base(service)
        {
            _sanitizerProvider = sanitizerProvider;
        }
        public async override Task<Person> GetPerson(string name)
        {
            var sanitizer = _sanitizerProvider.Resolve<string>(nameof(name));
            await sanitizer.SanitizeAsync(ref name);
            return await base.GetPerson(name);
        }
    }
}
