using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using CollectorQi.Droid;
using ListViewXamarin.Droid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

[assembly: Dependency(typeof(KeyboardHelper))]
namespace ListViewXamarin.Droid
{
    [Preserve(AllMembers = true)]
    public class KeyboardHelper : IKeyboardHelper
    {
        static Context _context;

        public KeyboardHelper()
        {

        }

        public static void Init(Context context)
        {
            _context = context;
        }

        public void HideKeyboard()
        {
            var context = Android.App.Application.Context;
            var inputMethodManager = context.GetSystemService(Context.InputMethodService) as InputMethodManager;
            if (inputMethodManager != null && context is Activity)
            {
                var activity = context as Activity;
                var token = activity.CurrentFocus?.WindowToken;
                inputMethodManager.HideSoftInputFromWindow(token, HideSoftInputFlags.None);
                activity.Window.DecorView.ClearFocus();
            }
        }
    }
}