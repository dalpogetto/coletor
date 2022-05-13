using System;
using Xamarin.Forms;
using Android.Views;

using Xamarin.Forms.Platform.Android;
using Android.Content;
using CollectorQi;
using Android.Text.Method;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CollectorQi.Droid.CustomEntryRenderer))]
namespace CollectorQi.Droid
{
    public class CustomEntryRenderer : EntryRenderer
    {
        public CustomEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (Control == null || Element == null) return;

            var element = (CustomEntry)Element;

            var isNumeric = element.IsNumeric;
            if (isNumeric)
            {
                // Force the keyboard to be numeric without sign                  
                Control.KeyListener = new DigitsKeyListener(false, true);
            }
        }
    }
}

