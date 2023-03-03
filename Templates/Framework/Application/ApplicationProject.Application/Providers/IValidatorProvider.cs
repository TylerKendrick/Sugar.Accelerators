using System.Runtime.CompilerServices;
using FluentValidation;

namespace ApplicationProject.Application
{
    interface IValidatorProvider
    {
        IValidator<T> Resolve<T>(string? @param = null, [CallerMemberName]string? callerMemberName = null);
    }
}
