using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Navigation;
using Prism.Logging;
using Prism.Services;

namespace XamarinFirebaseSample.ViewModels
{
    public class LoadingPageViewModel : ViewModelBase
    {
        public LoadingPageViewModel(INavigationService navigationService) : base(navigationService)
        {
        }
    }
}
