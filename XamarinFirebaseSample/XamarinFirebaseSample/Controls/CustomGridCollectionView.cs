using System;
using AiForms.Renderers;
using Xamarin.Forms;

namespace XamarinFirebaseSample.Controls
{
    public class CustomGridCollectionView : GridCollectionView
    {
        public CustomGridCollectionView()
        {
        }

        public CustomGridCollectionView(ListViewCachingStrategy cachingStrategy) : base(cachingStrategy)
        {
        }
    }
}
