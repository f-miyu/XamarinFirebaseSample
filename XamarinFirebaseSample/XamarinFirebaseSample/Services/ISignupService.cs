using System;
using System.Reactive;
using System.Threading.Tasks;
using Reactive.Bindings;

namespace XamarinFirebaseSample.Services
{
    public interface ISignupService
    {
        ReactivePropertySlim<string> Email { get; }
        ReactivePropertySlim<string> Name { get; }
        ReactivePropertySlim<string> Password { get; }
        ReactivePropertySlim<string> Image { get; }
        ReadOnlyReactivePropertySlim<bool> CanSignup { get; }
        IObservable<bool> DoingSignupNotifier { get; }
        IObservable<string> SignupErrorNotifier { get; }
        IObservable<Unit> SignupCompletedNotifier { get; }

        Task Signup();
    }
}