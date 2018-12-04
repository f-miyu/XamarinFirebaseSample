using System;
using System.Reactive;
using System.Threading.Tasks;
using Reactive.Bindings;

namespace XamarinFirebaseSample.Services
{
    public interface IEmailLoginService
    {
        ReactivePropertySlim<string> Email { get; }
        ReactivePropertySlim<string> Password { get; }
        ReadOnlyReactivePropertySlim<bool> CanLogin { get; }
        ReadOnlyReactivePropertySlim<bool> IsLoggingIn { get; }
        IObservable<string> LoginErrorNotifier { get; }
        IObservable<Unit> LoginCompletedNotifier { get; }
        Task Login();
    }
}