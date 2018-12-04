using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Reactive.Bindings;
using Reactive.Bindings.Notifiers;
using System.Reactive.Linq;
using Reactive.Bindings.Extensions;

namespace XamarinFirebaseSample.Services
{
    public class SignupService : ISignupService
    {
        private readonly IAccountService _accountService;

        public ReactivePropertySlim<string> Email { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> Name { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> Password { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> Image { get; } = new ReactivePropertySlim<string>();
        public ReadOnlyReactivePropertySlim<bool> CanSignup { get; }

        public BusyNotifier _signingUpNotifier = new BusyNotifier();
        public ReadOnlyReactivePropertySlim<bool> IsSigningUp { get; }

        public Subject<string> _signupErrorNotifier = new Subject<string>();
        public IObservable<string> SignupErrorNotifier => _signupErrorNotifier;

        private readonly Subject<Unit> _signupCompletedNotifier = new Subject<Unit>();
        public IObservable<Unit> SignupCompletedNotifier => _signupCompletedNotifier;

        public SignupService(IAccountService accountService)
        {
            _accountService = accountService;

            CanSignup = new[]
            {
                Email.Select(s => !string.IsNullOrEmpty(s)),
                Name.Select(s => !string.IsNullOrEmpty(s)),
                Password.Select(s => !string.IsNullOrEmpty(s))
            }
            .CombineLatestValuesAreAllTrue()
            .ToReadOnlyReactivePropertySlim();

            IsSigningUp = _signingUpNotifier.ToReadOnlyReactivePropertySlim();
        }

        public async Task Signup()
        {
            if (!CanSignup.Value)
                return;

            try
            {
                using (_signingUpNotifier.ProcessStart())
                {
                    await _accountService.SignupAsync(Email.Value, Password.Value, Name.Value, Image.Value);
                }
                _signupCompletedNotifier.OnNext(Unit.Default);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                _signupErrorNotifier.OnNext(e.Message);
            }
        }
    }
}
