using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing;
using ZXing.Net.Mobile.Forms;


namespace CollectorQi.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ZXingScannerPage : ContentPage
	{

        ZXingScannerView zxing;
        ZXingDefaultOverlayA overlay;

        public Action<string> ResultAction;
        public Action OpenBuscaItem;

        public ZXingScannerPage() : base()
        {
            zxing = new ZXingScannerView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                AutomationId = "zxingScannerView",
                IsTorchOn = true,
                IsAnalyzing = false,
                IsScanning = false,

            };
            var resultado_teste = "07;47.400.00020-7;BATERIA 12V/7,0AH C/;19765 UNICOBA INDUSTRI;0181854;04/04/2022;282596;     1/  4000;69575";

            this.ResultAction(resultado_teste);
            zxing.IsAnalyzing = false;

            zxing.OnScanResult += (result) =>  Device.BeginInvokeOnMainThread(async () =>
            {
                // Stop analysis until we navigate away so we don't keep reading barcodes
                zxing.IsAnalyzing = false;

                ResultAction(resultado_teste);
                //ResultAction(result.Text);

                // Navigate away

                try
                {
                    // Se ja foi fechado, da erro!
                    await Navigation.PopModalAsync();
                }
                catch (Exception ex)
                { }
            });

            overlay = new ZXingDefaultOverlayA
            {
                TopText = "Centralize o QRCode ou código de barras do Item",
                BottomText = "",
                //ShowFlashButton = zxing.HasTorch,
                ShowFlashButton = true,
                AutomationId = "zxingDefaultOverlay",

            };

            overlay.FlashButtonClicked += (sender, e) => {
                zxing.IsTorchOn = !zxing.IsTorchOn;

            };

            var grid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            /*
            var a = new Grid
            {

                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
               
            };*/

            //zxing.AutoFocus();

            grid.Children.Add(zxing);
            grid.Children.Add(overlay);
            //grid.Children.Add(a);


            // The root page of your application
            Content = grid;
        }


        public void SetResultAction(Action<string> dp)
        {
            ResultAction = dp;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            zxing.IsScanning = true;
        }

        protected override void OnDisappearing()
        {
            zxing.IsScanning = false;

            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {

            OpenBuscaItem?.Invoke();

            base.OnBackButtonPressed();
            /*Application.Current.MainPage = new NavigationPage(new RequisicaoListaPage(_isDevolucao));
            */
            return true;
        }


    }

    public class ZXingDefaultOverlayA : Grid
    {
        Label topText;
        Label botText;
        Button flash;

        public delegate void FlashButtonClickedDelegate(Button sender, EventArgs e);
        public event FlashButtonClickedDelegate FlashButtonClicked;

        public ZXingDefaultOverlayA()
        {
            BindingContext = this;

            VerticalOptions = LayoutOptions.FillAndExpand;
            HorizontalOptions = LayoutOptions.FillAndExpand;

            RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
            RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            Children.Add(new BoxView
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Black,
                Opacity = 0.7,
            }, 0, 0);


            Children.Add(new BoxView
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Black,
                Opacity = 0.7,
            }, 0, 2);

            Children.Add(new BoxView
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start,
                Margin = new Thickness(80, 80, 80, 80),
                HeightRequest = 3,
                BackgroundColor = Color.Red,
                Opacity = 0.6,
            }, 0, 1);

            Children.Add(new BoxView
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start,
                Margin = new Thickness(80, 80, 80, 80),
                WidthRequest = 3,
                BackgroundColor = Color.Red,
                Opacity = 0.6,
            }, 0, 1);

            Children.Add(new BoxView
            {
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.End,
                Margin = new Thickness(80, 80, 80, 80),
                HeightRequest = 3,
                BackgroundColor = Color.Red,
                Opacity = 0.6,
            }, 0, 1);

            Children.Add(new BoxView
            {
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.End,
                Margin = new Thickness(80, 80, 80, 80),
                WidthRequest = 3,
                BackgroundColor = Color.Red,
                Opacity = 0.6,
            }, 0, 1);

            Children.Add(new BoxView
            {
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Start,
                Margin = new Thickness(80, 80, 80, 80),
                HeightRequest = 3,
                BackgroundColor = Color.Red,
                Opacity = 0.6,
            }, 0, 1);

            Children.Add(new BoxView
            {
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Start,
                Margin = new Thickness(80, 80, 80, 80),
                WidthRequest = 3,
                BackgroundColor = Color.Red,
                Opacity = 0.6,
            }, 0, 1);

            Children.Add(new BoxView
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.End,
                Margin = new Thickness(80, 80, 80, 80),
                HeightRequest = 3,
                BackgroundColor = Color.Red,
                Opacity = 0.6,
            }, 0, 1);

            Children.Add(new BoxView
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.End,
                Margin = new Thickness(80, 80, 80, 80),
                WidthRequest = 3,
                BackgroundColor = Color.Red,
                Opacity = 0.6,
            }, 0, 1);



            /* QR CODE MENOR */
            int iMargem = 140;
            Children.Add(new BoxView
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start,
                Margin = new Thickness(iMargem, iMargem, iMargem, iMargem),
                HeightRequest = 3,
                BackgroundColor = Color.Red,
                Opacity = 0.6,
            }, 0, 1);

            Children.Add(new BoxView
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start,
                Margin = new Thickness(iMargem, iMargem, iMargem, iMargem),
                WidthRequest = 3,
                BackgroundColor = Color.Red,
                Opacity = 0.6,
            }, 0, 1);

            Children.Add(new BoxView
            {
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.End,
                Margin = new Thickness(iMargem, iMargem, iMargem, iMargem),
                HeightRequest = 3,
                BackgroundColor = Color.Red,
                Opacity = 0.6,
            }, 0, 1);

            Children.Add(new BoxView
            {
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.End,
                Margin = new Thickness(iMargem, iMargem, iMargem, iMargem),
                WidthRequest = 3,
                BackgroundColor = Color.Red,
                Opacity = 0.6,
            }, 0, 1);

            Children.Add(new BoxView
            {
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Start,
                Margin = new Thickness(iMargem, iMargem, iMargem, iMargem),
                HeightRequest = 3,
                BackgroundColor = Color.Red,
                Opacity = 0.6,
            }, 0, 1);

            Children.Add(new BoxView
            {
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Start,
                Margin = new Thickness(iMargem, iMargem, iMargem, iMargem),
                WidthRequest = 3,
                BackgroundColor = Color.Red,
                Opacity = 0.6,
            }, 0, 1);

            Children.Add(new BoxView
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.End,
                Margin = new Thickness(iMargem, iMargem, iMargem, iMargem),
                HeightRequest = 3,
                BackgroundColor = Color.Red,
                Opacity = 0.6,
            }, 0, 1);

            Children.Add(new BoxView
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.End,
                Margin = new Thickness(iMargem, iMargem, iMargem, iMargem),
                WidthRequest = 3,
                BackgroundColor = Color.Red,
                Opacity = 0.6,
            }, 0, 1);

            topText = new Label
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Color.White,
                AutomationId = "zxingDefaultOverlay_TopTextLabel",
            };

            topText.SetBinding(Label.TextProperty, new Binding(nameof(TopText)));
            Children.Add(topText, 0, 0);

            botText = new Label
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Color.White,
                AutomationId = "zxingDefaultOverlay_BottomTextLabel",
            };

            botText.SetBinding(Label.TextProperty, new Binding(nameof(BottomText)));

            Children.Add(botText, 0, 2);

            flash = new Button
            {
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Start,
                Text = "Flash",
                TextColor = Color.White,

                AutomationId = "zxingDefaultOverlay_FlashButton",

            };

            flash.SetBinding(Button.IsVisibleProperty, new Binding(nameof(ShowFlashButton)));
            flash.Clicked += (sender, e) => {
                FlashButtonClicked?.Invoke(flash, e);
            };

            Children.Add(flash, 0, 0);
        }

        public static readonly BindableProperty TopTextProperty =
            BindableProperty.Create(nameof(TopText), typeof(string), typeof(ZXingDefaultOverlay), string.Empty);
        public string TopText
        {
            get { return (string)GetValue(TopTextProperty); }
            set { SetValue(TopTextProperty, value); }
        }

        public static readonly BindableProperty BottomTextProperty =
            BindableProperty.Create(nameof(BottomText), typeof(string), typeof(ZXingDefaultOverlay), string.Empty);
        public string BottomText
        {
            get { return (string)GetValue(BottomTextProperty); }
            set { SetValue(BottomTextProperty, value); }
        }

        public static readonly BindableProperty ShowFlashButtonProperty =
            BindableProperty.Create(nameof(ShowFlashButton), typeof(bool), typeof(ZXingDefaultOverlay), false);
        public bool ShowFlashButton
        {
            get { return (bool)GetValue(ShowFlashButtonProperty); }
            set { SetValue(ShowFlashButtonProperty, value); }
        }

        public static BindableProperty FlashCommandProperty =
            BindableProperty.Create(nameof(FlashCommand), typeof(ICommand), typeof(ZXingDefaultOverlay),
                defaultValue: default(ICommand),
                propertyChanged: OnFlashCommandChanged);

        public ICommand FlashCommand
        {
            get { return (ICommand)GetValue(FlashCommandProperty); }
            set { SetValue(FlashCommandProperty, value); }
        }

        private static void OnFlashCommandChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            /*
            var overlay = bindable as ZXingDefaultOverlay;
            if (overlay?.flash == null) return;
            overlay.flash.Command = newValue as Command;*/
        }
    }

}