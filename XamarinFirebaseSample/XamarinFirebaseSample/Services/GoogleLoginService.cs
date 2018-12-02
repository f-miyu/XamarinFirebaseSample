using System;
using System.Threading.Tasks;
using Reactive.Bindings.Notifiers;
using System.Reactive.Subjects;
using System.Reactive;

namespace XamarinFirebaseSample.Services
{
    public class GoogleLoginService : IGoogleLoginService
    {
        private readonly IAccountService _accountService;
        private readonly IAuthService _authService;

        public BusyNotifier _doingLoginNotifier = new BusyNotifier();
        public IObservable<bool> DoingLoginNotifier => _doingLoginNotifier;

        public Subject<string> _loginErrorNotifier = new Subject<string>();
        public IObservable<string> LoginErrorNotifier => _loginErrorNotifier;

        private readonly Subject<Unit> _loginCompletedNotifier = new Subject<Unit>();
        public IObservable<Unit> LoginCompletedNotifier => _loginCompletedNotifier;

        public GoogleLoginService(IAccountService accountService, IAuthService authService)
        {
            _accountService = accountService;
            _authService = authService;
        }

        public async Task Login()
        {
            try
            {
                var (idToken, accessToken) = await _authService.LoginWithGoogle().ConfigureAwait(false);

                if (idToken != null)
                {
                    using (_doingLoginNotifier.ProcessStart())
                    {
                        await _accountService.LoginWithGoogleAsync(idToken, accessToken);
                    }
                    _loginCompletedNotifier.OnNext(Unit.Default);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                _loginErrorNotifier.OnNext(e.Message);
            }
        }
    }
}
