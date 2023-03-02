using System.Runtime.CompilerServices;
using Hygiene;

namespace ApplicationProject.Application
{
    interface ISanitizerProvider
    {
        ISanitizer<T> Resolve<T>(string? @param = null, [CallerMemberName]string? callerMemberName = null);
    }
}
