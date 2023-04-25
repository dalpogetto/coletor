using System;
using Xamarin.Forms;

namespace CollectorQi
{
    public class CustomEntry : Entry
    {

        public static readonly BindableProperty IsNumericProperty = BindableProperty.Create("IsNumeric", typeof(Boolean), typeof(Entry), false);

        public Boolean IsNumeric
        {
            get { return (Boolean)GetValue(IsNumericProperty); }
            set { SetValue(IsNumericProperty, value); }

        }
    }

    public class TextChangedBehavior : Behavior<SearchBar>
    {
        protected override void OnAttachedTo(SearchBar bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.TextChanged += Bindable_TextChanged;
        }

        protected override void OnDetachingFrom(SearchBar bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.TextChanged -= Bindable_TextChanged;
        }

        private void Bindable_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((SearchBar)sender).SearchCommand?.Execute(e.NewTextValue);
        }
    }
}