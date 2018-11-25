using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using Prism.NavigationEx;

namespace XamarinFirebaseSample.ViewModels
{
    public class ViewModelBase : NavigationViewModel
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
