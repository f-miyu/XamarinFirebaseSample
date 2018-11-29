using System;
using System.Threading.Tasks;
using Plugin.FirebaseAuth;
using XamarinFirebaseSample.Models;
using Plugin.CloudFirestore;
using System.Reactive.Subjects;
using Reactive.Bindings;
using XamarinFirebaseSample.Helpers;
using Xamarin.Forms;
using Prism.Mvvm;
using System.Reactive.Linq;
using Plugin.CloudFirestore.Extensions;
using Reactive.Bindings.Extensions;
using System.Reactive.Disposables;

namespace XamarinFirebaseSample.Services
{
    [Singleton]
    public class AccountService : IAccountService
    {
        private readonly IAuthService _authService;
        private readonly IAuth _firebaseAuth;
        private readonly IFirestore _firestore;
        private readonly Plugin.FirebaseAuth.IListenerRegistration _registration;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private readonly ReactivePropertySlim<bool> _isInitialized = new ReactivePropertySlim<bool>();
        public ReadOnlyReactivePropertySlim<bool> IsInitialized { get; }

        private readonly ReactivePropertySlim<bool> _isLoggedIn = new ReactivePropertySlim<bool>();
        public ReadOnlyReactivePropertySlim<bool> IsLoggedIn { get; }

        private readonly Subject<string> _loginErrorNotifier = new Subject<string>();
        public IObservable<string> LoginErrorNotifier => _loginErrorNotifier;

        private readonly ReactivePropertySlim<string> _userId = new ReactivePropertySlim<string>();
        public ReadOnlyReactivePropertySlim<string> UserId { get; }

        private readonly ReactivePropertySlim<string> _userName = new ReactivePropertySlim<string>();
        public ReadOnlyReactivePropertySlim<string> UserName { get; }

        private readonly ReactivePropertySlim<string> _userImage = new ReactivePropertySlim<string>();
        public ReadOnlyReactivePropertySlim<string> UserImage { get; }

        public AccountService(IAuthService authService)
        {
            _authService = authService;
            _firebaseAuth = CrossFirebaseAuth.Current.Instance;
            _firestore = CrossCloudFirestore.Current.Instance;

            IsInitialized = _isInitialized.ToReadOnlyReactivePropertySlim();
            IsLoggedIn = _isLoggedIn.ToReadOnlyReactivePropertySlim();
            UserId = _userId.ToReadOnlyReactivePropertySlim();
            UserName = _userName.ToReadOnlyReactivePropertySlim();
            UserImage = _userImage.ToReadOnlyReactivePropertySlim();

            _registration = _firebaseAuth.AddAuthStateChangedListener(user =>
            {
                if (user != null)
                {
                    _isLoggedIn.Value = true;
                    _userId.Value = user.Uid;
                    _userName.Value = user.DisplayName;
                    _userImage.Value = user.PhotoUrl.ToString();
                }
                else
                {
                    _isLoggedIn.Value = false;
                    _userId.Value = null;
                    _userName.Value = null;
                    _userImage.Value = null;
                }

                if (!_isInitialized.Value)
                {
                    _isInitialized.Value = true;
                }
            });

            UserId.Select(userId => userId == null ? Observable.Never<User>() :
                          _firestore.GetCollection(User.CollectionPath)
                                    .GetDocument(userId)
                                    .AsObservable()
                                    .Where(d => d.Exists)
                                    .Select(d => d.ToObject<User>()))
                  .Switch()
                  .Subscribe(user =>
                  {
                      _userName.Value = user.Name;
                      _userImage.Value = user.Image;
                  })
                  .AddTo(_disposables);
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

        public void Close()
        {
            _registration.Remove();
            _disposables.Dispose();
        }
    }
}
