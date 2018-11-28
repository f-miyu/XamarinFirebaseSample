using System;
using System.Threading.Tasks;
using Plugin.FirebaseAuth;
using XamarinFirebaseSample.Models;
using Plugin.CloudFirestore;
using System.Reactive.Subjects;
using Reactive.Bindings;
using XamarinFirebaseSample.Helpers;

namespace XamarinFirebaseSample.Services
{
    [Singleton]
    public class AccountService : IAccountService
    {
        private readonly IAuthService _authService;
        private readonly IAuth _firebaseAuth;
        private readonly IFirestore _firestore;

        private readonly ReactivePropertySlim<bool> _isInitialized = new ReactivePropertySlim<bool>();
        public ReadOnlyReactivePropertySlim<bool> IsInitialized { get; }

        private readonly ReactivePropertySlim<bool> _isLoggedIn = new ReactivePropertySlim<bool>();
        public ReadOnlyReactivePropertySlim<bool> IsLoggedIn { get; }

        private readonly Subject<string> _loginErrorNotifier = new Subject<string>();
        public IObservable<string> LoginErrorNotifier => _loginErrorNotifier;

        public AccountService(IAuthService authService)
        {
            _authService = authService;
            _firebaseAuth = CrossFirebaseAuth.Current.Instance;
            _firestore = CrossCloudFirestore.Current.Instance;

            IsInitialized = _isInitialized.ToReadOnlyReactivePropertySlim();
            IsLoggedIn = _isLoggedIn.ToReadOnlyReactivePropertySlim();

            _firebaseAuth.AddAuthStateChangedListener(user =>
            {
                _isLoggedIn.Value = user != null;

                if (!_isInitialized.Value)
                {
                    _isInitialized.Value = true;
                }
            });
        }

        public async Task LoginWithGoogle()
        {
            try
            {
                var (idToken, accessToken) = await _authService.LoginWithGoogle();

                if (idToken != null)
                {
                    var credential = CrossFirebaseAuth.Current
                                                      .GoogleAuthProvider
                                                      .GetCredential(idToken, accessToken);

                    var result = await _firebaseAuth.SignInWithCredentialAsync(credential).ConfigureAwait(false);

                    var authUser = result.User;

                    var reference = _firestore.GetCollection(User.CollectionPath)
                                              .GetDocument(authUser.Uid);

                    var document = await reference.GetDocumentAsync().ConfigureAwait(false);

                    if (!document.Exists)
                    {
                        var user = new User
                        {
                            Name = authUser.DisplayName,
                            Image = authUser.PhotoUrl.ToString()
                        };

                        await reference.SetDataAsync(user).ConfigureAwait(false);
                    }
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
