using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

/*
namespace CollectorQi.Droid
{
    class CustomEditText : EditText
    {

        protected CustomEditText(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }


        public CustomEditText(Context context) : base(context)
        {
            Init();
        }

        public CustomEditText(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init(attrs);
        }

        public CustomEditText(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Init(attrs);
        }

        public CustomEditText(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Init(attrs);
        }

        //This method can be used to intercept keystroke events, typically handling the back key, updating the UI, and if you don't override this method, the IME will default to handling the keyboard (usually the keyboard disappears)
        public override bool OnKeyPreIme(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back)
            {
                if (listener != null)
                {
                    listener.onKeyBack();
                    return true;
                }
            }
            return base.OnKeyPreIme(keyCode, e);
        }

        private OnEditTextKeyBackListener listener;
        public void SetOnEditTextKeyBackListener(OnEditTextKeyBackListener listener)
        {
            this.listener = listener;
        }

        public interface OnEditTextKeyBackListener
        {
            void onKeyBack();

        }


        private void Init(IAttributeSet attrs = null)
        {
           // var activity = (Activity)Forms.Context;
           // if (activity == null)
           //     return;

            //clearButton = ContextCompat.GetDrawable(Android.App.Application.Context, Resource.Drawable.abc_cab_background_internal_bg);
            //clearButton.SetBounds(0, 0, clearButton.IntrinsicWidth, clearButton.IntrinsicHeight);

            SetupEvents();
        }

        private void SetupEvents()
        {
            // Handle clear button visibility
            this.TextChanged += (sender, e) => {
                UpdateClearButton();
            };
            this.FocusChange += (sender, e) => {
                UpdateClearButton(e.HasFocus);
            };

            // Handle clearing the text
            this.Touch += (sender, e) => {
                if (this.GetCompoundDrawables()[2] == null || e.Event.Action != MotionEventActions.Up)
                {
                    e.Handled = false;
                    return;
                }
               //if (e.Event.GetX() > (this.Width - this.PaddingRight - clearButton.IntrinsicWidth))
               //{
               //    this.Text = "";
               //    UpdateClearButton();
               //    e.Handled = true;
               //}
               else
                    e.Handled = false;
            };
        }

        private void UpdateClearButton(bool hasFocus = true)
        {
            var compoundDrawables = this.GetCompoundDrawables();
          //  var compoundDrawable = this.Text.Length == 0 || !hasFocus ? null : clearButton;
          //  this.SetCompoundDrawables(compoundDrawables[0], compoundDrawables[1], compoundDrawable, compoundDrawables[3]);
        }
    }
}
*/