﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using XamarinFirebaseSample.Models;

namespace XamarinFirebaseSample.Services
{
    public interface IItemListService
    {
        ReadOnlyObservableCollection<Item> Items { get; }
        Task LoadAsync();
        void Close();
    }
}
