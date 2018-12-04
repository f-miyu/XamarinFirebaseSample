using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;

namespace XamarinFirebaseSample.Services
{
    public class EmailLoginService : IEmailLoginService
    {
        private readonly IAccountService _accountService;

        public ReactivePropertySlim<string> Email { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> Password { get; } = new ReactivePropertySlim<string>();
        public ReadOnlyReactivePropertySlim<bool> CanLogin { get; }

        public BusyNotifier _loggingInNotifier = new BusyNotifier();
        public ReadOnlyReactivePropertySlim<bool> IsLoggingIn { get; }

        public Subject<string> _loginErrorNotifier = new Subject<string>();
        public IObservable<string> LoginErrorNotifier => _loginErrorNotifier;

        private readonly Subject<Unit> _loginCompletedNotifier = new Subject<Unit>();
        public IObservable<Unit> LoginCompletedNotifier => _loginCompletedNotifier;

        public EmailLoginService(IAccountService accountService)
        {
            _accountService = accountService;

            CanLogin = new[]
            {
                Email.Select(s => !string.IsNullOrEmpty(s)),
                Password.Select(s => !string.IsNullOrEmpty(s))
            }
            .CombineLatestValuesAreAllTrue()
            .ToReadOnlyReactivePropertySlim();

            IsLoggingIn = _loggingInNotifier.ToReadOnlyReactivePropertySlim();
        }

        public async Task Login()
        {
            if (!CanLogin.Value)
                return;

            try
            {
                using (_loggingInNotifier.ProcessStart())
                {
                    await _accountService.LoginWithEmailAndPasswordAsync(Email.Value, Password.Value);
                }
                _loginCompletedNotifier.OnNext(Unit.Default);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                _loginErrorNotifier.OnNext(e.Message);
            }
        }
    }
}
