using Prism;
using Prism.Ioc;
using XamarinFirebaseSample.ViewModels;
using XamarinFirebaseSample.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Prism.NavigationEx;
using System.Linq;
using System;
using Xamarin.Forms.Internals;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace XamarinFirebaseSample
{
    public partial class App
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync<MainPageViewModel>(wrapInNavigationPage: true);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation(this);

            GetType().Assembly.GetTypes()
                     .Where(t => t.Namespace.EndsWith(".Services", StringComparison.Ordinal) && !t.IsAbstract && !t.IsInterface)
                     .Select(t => (Interface: t.GetInterface("I" + t.Name), Type: t))
                     .Where(t => t.Interface != null)
                     .ForEach(t =>
                     {
                         if (Attribute.GetCustomAttribute(t.Type, typeof(SingletonAttribute)) != null)
                         {
                             containerRegistry.RegisterSingleton(t.Interface, t.Type);
                         }
                         else
                         {
                             containerRegistry.Register(t.Interface, t.Type);
                         }
                     });
        }
    }
}
