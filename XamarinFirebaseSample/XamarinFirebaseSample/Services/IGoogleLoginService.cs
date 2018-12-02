using System;
using System.Reactive;
using System.Threading.Tasks;

namespace XamarinFirebaseSample.Services
{
    public interface IGoogleLoginService
    {
        IObservable<bool> DoingLoginNotifier { get; }
        IObservable<string> LoginErrorNotifier { get; }
        IObservable<Unit> LoginCompletedNotifier { get; }
        Task Login();
    }
}
