using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using CollectorQi.Droid;
using CollectorQi.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(DepositosUsuarioPorTransacaoListaPopUp), typeof(PopUpBaseRenderer))]
namespace CollectorQi.Droid
{
    
        [Preserve(AllMembers = true)]
        public class PopUpBaseRenderer : Rg.Plugins.Popup.Droid.Renderers.PopupPageRenderer
        {
            private Android.Graphics.Rect LastVisibleRect;

            public PopUpBaseRenderer(Context context) : base(context) { }

            protected override void OnAttachedToWindow()
            {
                ViewTreeObserver.GlobalLayout += ViewTreeObserver_GlobalLayout;
                base.OnAttachedToWindow();
            }

            protected override void OnDetachedFromWindow()
            {
                ViewTreeObserver.GlobalLayout -= ViewTreeObserver_GlobalLayout;
                base.OnDetachedFromWindow();
            }

            private void ViewTreeObserver_GlobalLayout(object sender, EventArgs e)
            {
                Console.WriteLine($"global layout");

                try
                {
                    var activity = (Activity?)Context;
                    var decoreView = activity?.Window?.DecorView;
                    var visibleRect = new Android.Graphics.Rect();
                    decoreView?.GetWindowVisibleDisplayFrame(visibleRect);
                    Console.WriteLine($"visible Rect: {visibleRect.FlattenToString()}");

                    if (LastVisibleRect?.FlattenToString() != visibleRect.FlattenToString())
                    {
                        Console.WriteLine($"global layout RequestLayout");
                        LastVisibleRect = visibleRect;
                        this.RequestLayout();
                    }

               
                    
                 }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            public void showKeyboard(EditText v)
            {

                var activity = (Activity?)Context;
                InputMethodManager inputMethodManager = (InputMethodManager)activity.GetSystemService(Context.InputMethodService);
                v.RequestFocus();
                inputMethodManager.ShowSoftInput(v, 0);
                inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);//personal line added
            }
        
    }
}