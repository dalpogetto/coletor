using Android.Graphics;
using Android.Text.Method;
using Android.Text;
using Android.Widget;
using MyApp.Droid.Renderer;
using static Java.Util.ResourceBundle;
using System.ComponentModel;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using CollectorQi.Resources;
using Android.Content;
using CollectorQi;

[assembly: ExportRenderer(typeof(CustomEntryNumeric), typeof(CustomEntryNumericRenderer))]
namespace MyApp.Droid.Renderer
{
    public class CustomEntryNumericRenderer : EntryRenderer
    {
        public CustomEntryNumericRenderer(Context context) : base(context)
        {

        }

        private EditText _native = null;

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
                return;

            _native = Control as EditText;
            _native.InputType = Android.Text.InputTypes.ClassNumber;
            if ((e.NewElement as CustomEntryNumeric).AllowNegative == true)
                _native.InputType |= InputTypes.NumberFlagSigned;
            if ((e.NewElement as CustomEntryNumeric).AllowFraction == true)
            {
                _native.InputType |= InputTypes.NumberFlagDecimal;
                _native.KeyListener = DigitsKeyListener.GetInstance(string.Format("1234567890{0}", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
            }
            if (e.NewElement.FontFamily != null)
            {
                var font = Typeface.CreateFromAsset(Android.App.Application.Context.Assets, e.NewElement.FontFamily);
                _native.Typeface = font;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (_native == null)
                return;

            if (e.PropertyName == CustomEntryNumeric.AllowNegativeProperty.PropertyName)
            {
                if ((sender as CustomEntryNumeric).AllowNegative == true)
                {
                    // Add Signed flag
                    _native.InputType |= InputTypes.NumberFlagSigned;
                }
                else
                {
                    // Remove Signed flag
                    _native.InputType &= ~InputTypes.NumberFlagSigned;
                }
            }
            if (e.PropertyName == CustomEntryNumeric.AllowFractionProperty.PropertyName)
            {
                if ((sender as CustomEntryNumeric).AllowFraction == true)
                {
                    // Add Decimal flag
                    _native.InputType |= InputTypes.NumberFlagDecimal;
                    _native.KeyListener = DigitsKeyListener.GetInstance(string.Format("1234567890{0}", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
                }
                else
                {
                    // Remove Decimal flag
                    _native.InputType &= ~InputTypes.NumberFlagDecimal;
                    _native.KeyListener = DigitsKeyListener.GetInstance(string.Format("1234567890"));
                }
            }
        }
    }
}