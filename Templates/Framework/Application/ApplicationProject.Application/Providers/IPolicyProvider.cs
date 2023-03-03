using System.Runtime.CompilerServices;
using Polly;

namespace ApplicationProject.Application
{
    interface IPolicyProvider
    {
        IAsyncPolicy<T> ResolveAsync<T>([CallerMemberName]string? callerMemberName = null);
    }
}
