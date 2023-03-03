using System.Runtime.CompilerServices;
using Hygiene;

namespace ApplicationProject.Application
{
    /// <summary>
    /// Provides an interface for resolving an <see cref="ISanitizer{T}"/> instance.
    /// </summary>
    public interface ISanitizerProvider
    {
        /// <summary>
        /// Resolves an <see cref="ISanitizer{T}"/> instance for the specified type.
        /// </summary>
        /// <typeparam name="T">The type of object to sanitize.</typeparam>
        /// <param name="param">An optional parameter.</param>
        /// <param name="callerMemberName">The name of the calling member.</param>
        /// <returns>An instance of <see cref="ISanitizer{T}"/>.</returns>
        ISanitizer<T> Resolve<T>(
            string? param = null, 
            [CallerMemberName]string? callerMemberName = null);
    }
}
