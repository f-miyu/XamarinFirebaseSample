using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using Prism.NavigationEx;

namespace XamarinFirebaseSample.ViewModels
{
    public abstract class ViewModelBase : NavigationViewModel
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ViewModelBase(INavigationService navigationService) : base(navigationService)
        {
        }
    }

    public abstract class ViewModelBase<TParameter> : NavigationViewModel<TParameter>
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ViewModelBase(INavigationService navigationService) : base(navigationService)
        {
        }
    }

    public abstract class ViewModelBaseResult<TResult> : NavigationViewModelResult<TResult>
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ViewModelBaseResult(INavigationService navigationService) : base(navigationService)
        {
        }
    }

    public abstract class ViewModelBase<TParameter, TResult> : NavigationViewModel<TParameter, TResult>
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ViewModelBase(INavigationService navigationService) : base(navigationService)
        {
        }
    }
}
