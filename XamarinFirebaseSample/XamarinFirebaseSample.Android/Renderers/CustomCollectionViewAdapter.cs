using System;
using AiForms.Renderers;
using AiForms.Renderers.Droid;
using Android.Content;
using Android.Support.V7.Widget;
using Xamarin.Forms;

namespace XamarinFirebaseSample.Droid.Renderers
{
    public class CustomCollectionViewAdapter : CollectionViewAdapter
    {
        RecyclerView _recyclerView;

        public CustomCollectionViewAdapter(Context context, AiForms.Renderers.CollectionView collectionView, RecyclerView recyclerView, ICollectionViewRenderer renderer) : base(context, collectionView, recyclerView, renderer)
        {
            _recyclerView = recyclerView;
        }

        protected override void InvalidateItemDecoration()
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(250), () =>
            {
                _recyclerView?.Invalidate();
                return false;
            });
        }
    }
}
