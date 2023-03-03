namespace ApplicationProject.Application
{
    /// <summary>
    /// Decorates a service with a sanitization feature.
    /// </summary>
    public class SanitizerServiceDecorator : ServiceDecorator
    {
        private readonly ISanitizerProvider _sanitizerProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SanitizerServiceDecorator"/> class.
        /// </summary>
        /// <param name="service">The service to decorate.</param>
        /// <param name="sanitizerProvider">The provider for sanitization.</param>
        public SanitizerServiceDecorator(
            IService service,
            ISanitizerProvider sanitizerProvider)
            : base(service)
        {
            _sanitizerProvider = sanitizerProvider;
        }

        /// <summary>
        /// Gets a person with a sanitized name.
        /// </summary>
        /// <param name="name">The name of the person to get.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the person with a sanitized name.</returns>
        public async override Task<Person> GetPerson(string name)
        {
            var sanitizer = _sanitizerProvider.Resolve<string>(nameof(name));
            await sanitizer.SanitizeAsync(ref name);
            return await base.GetPerson(name);
        }
    }
}
