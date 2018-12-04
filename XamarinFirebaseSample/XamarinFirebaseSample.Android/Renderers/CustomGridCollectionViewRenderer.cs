using System;
using AiForms.Renderers;
using AiForms.Renderers.Droid;
using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XamarinFirebaseSample.Controls;
using XamarinFirebaseSample.Droid.Renderers;

[assembly: ExportRenderer(typeof(CustomGridCollectionView), typeof(CustomGridCollectionViewRenderer))]
namespace XamarinFirebaseSample.Droid.Renderers
{
    public class CustomGridCollectionViewRenderer : GridCollectionViewRenderer
    {
        public CustomGridCollectionViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<AiForms.Renderers.CollectionView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                Adapter = new CustomCollectionViewAdapter(Context, (GridCollectionView)Element, RecyclerView, this);

                RecyclerView.SetAdapter(Adapter);

                Adapter.IsAttachedToWindow = IsAttached;
            }
        }
    }
}
