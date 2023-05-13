using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace CollectorQi
{
    public class CustomEntryNumeric : Entry
    {
        public static BindableProperty AllowNegativeProperty = BindableProperty.Create("AllowNegative", typeof(bool), typeof(CustomEntryNumeric), false, BindingMode.TwoWay);
        public static BindableProperty AllowFractionProperty = BindableProperty.Create("AllowFraction", typeof(bool), typeof(CustomEntryNumeric), false, BindingMode.TwoWay);

        public CustomEntryNumeric()
        {
            this.Keyboard = Keyboard.Numeric;
        }

        public bool AllowNegative
        {
            get { return (bool)GetValue(AllowNegativeProperty); }
            set { SetValue(AllowNegativeProperty, value); }
        }

        public bool AllowFraction
        {
            get { return (bool)GetValue(AllowFractionProperty); }
            set { SetValue(AllowFractionProperty, value); }
        }
    }
}
