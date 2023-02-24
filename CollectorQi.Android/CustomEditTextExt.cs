using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
//using App18.Droid;
using CollectorQi.Droid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

/*
[assembly: ExportRenderer(typeof(Entry), typeof(MyEntryRender))]
namespace App18.Droid
{
    class MyEntryRender : ViewRenderer<Entry, EditText>, CustomEditText.OnEditTextKeyBackListener
    {
        private Context _context;
        public MyEntryRender(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            // CustomEditText editext = new CustomEditText(_context);

            CustomEditText editext = new CustomEditText(_context);

            SetNativeControl(editext);
            editext.SetOnEditTextKeyBackListener(this);
        }


        public void onKeyBack()
        {
            _context.GetActivity().OnBackPressed();
        }
    }
}
*/