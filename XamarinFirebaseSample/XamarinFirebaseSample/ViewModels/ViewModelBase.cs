using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using Prism.NavigationEx;
using Prism.AppModel;
using Plugin.FirebaseAnalytics;

namespace XamarinFirebaseSample.ViewModels
{
    public abstract class ViewModelBase : NavigationViewModel, IPageLifecycleAware
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        protected ViewModelBase(INavigationService navigationService) : base(navigationService)
        {
        }

        public virtual void OnAppearing()
        {
            var typeName = GetType().Name;
            CrossFirebaseAnalytics.Current.SetCurrentScreen(typeName, typeName);
        }

        public virtual void OnDisappearing()
        {
        }
    }

    public abstract class ViewModelBase<TParameter> : NavigationViewModel<TParameter>, IPageLifecycleAware
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        protected ViewModelBase(INavigationService navigationService) : base(navigationService)
        {
        }

        public virtual void OnAppearing()
        {
            var typeName = GetType().Name;
            CrossFirebaseAnalytics.Current.SetCurrentScreen(typeName, typeName);
        }

        public virtual void OnDisappearing()
        {
        }
    }

    public abstract class ViewModelBaseResult<TResult> : NavigationViewModelResult<TResult>, IPageLifecycleAware
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        protected ViewModelBaseResult(INavigationService navigationService) : base(navigationService)
        {
        }

        public virtual void OnAppearing()
        {
            var typeName = GetType().Name;
            CrossFirebaseAnalytics.Current.SetCurrentScreen(typeName, typeName);
        }

        public virtual void OnDisappearing()
        {
        }
    }

    public abstract class ViewModelBase<TParameter, TResult> : NavigationViewModel<TParameter, TResult>, IPageLifecycleAware
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        protected ViewModelBase(INavigationService navigationService) : base(navigationService)
        {
        }

        public virtual void OnAppearing()
        {
            var typeName = GetType().Name;
            CrossFirebaseAnalytics.Current.SetCurrentScreen(typeName, typeName);
        }

        public virtual void OnDisappearing()
        {
        }
    }
}
