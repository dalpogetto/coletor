using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using CollectorQi.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(CollectorQi.Droid.KeyboardService))]

namespace CollectorQi.Droid
{
    public class KeyboardService : IKeyboardService
    {
        public event EventHandler KeyboardIsShown;
        public event EventHandler KeyboardIsHidden;

        private InputMethodManager inputMethodManager;

        private bool wasShown = false;

        public KeyboardService()
        {
            GetInputMethodManager();
            SubscribeEvents();
        }

        public void OnGlobalLayout(object sender, EventArgs args)
        {
            GetInputMethodManager();
            if (!wasShown && IsCurrentlyShown())
            {
                KeyboardIsShown?.Invoke(this, EventArgs.Empty);
                wasShown = true;
            }
            else if (wasShown && !IsCurrentlyShown())
            {
                KeyboardIsHidden?.Invoke(this, EventArgs.Empty);
                wasShown = false;
            }
        }

        private bool IsCurrentlyShown()
        {
            return inputMethodManager.IsAcceptingText;
        }

        private void GetInputMethodManager()
        {
            if (inputMethodManager == null || inputMethodManager.Handle == IntPtr.Zero)
            {
                inputMethodManager = (InputMethodManager)this.GetSystemService(Context.InputMethodService);
            }
        }

        private InputMethodManager GetSystemService(string inputMethodService)
        {
            throw new NotImplementedException();
        }

        private void SubscribeEvents()
        {   
            ((Activity)Xamarin.Forms.Forms.Context).Window.DecorView.ViewTreeObserver.GlobalLayout += this.OnGlobalLayout;
        }

        public void showSoftKeyboard(View view)
        {
           // Context context = Android.App.Application.Context;
           // InputMethodManager inputMethodManager = (InputMethodManager)activity.GetSystemService(Context.InputMethodService);
           // view.RequestFocus();
           // inputMethodManager.ShowSoftInput(view, 0);
           // inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);//personal line added
        }
    }

    public interface IKeyboardHelper
    {
        void HideKeyboard();
    }

}