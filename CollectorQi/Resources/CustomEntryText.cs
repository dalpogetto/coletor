using Xamarin.Forms;

namespace CollectorQi
{
    public class CustomEntryText : Entry
    {
        public CustomEntryText()
        {
            /*this.HeightRequest = 24;
             this.WidthRequest = 24; */
            this.HeightRequest = 40;
            this.ImageWidth = 24;
            this.ImageHeight = 24;
            this.CornerRadius = 5;
            this.IsCurvedCornersEnabled = false;
            this.VerticalOptions = LayoutOptions.End;

        }

        public static readonly BindableProperty BorderColorPropetary
            = BindableProperty.Create(
                nameof(BorderColor),
                typeof(Color),
                typeof(CustomEntryText),
                Color.Gray);

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorPropetary); }
            set { SetValue(BorderColorPropetary, value); }
        }


        public static readonly BindableProperty BorderWidthProperty =
        BindableProperty.Create(
            nameof(BorderWidth),
            typeof(int),
            typeof(CustomEntryText),
            Device.OnPlatform<int>(1, 2, 2));

        // Gets or sets BorderWidth value
        public int BorderWidth
        {
            get { return (int)GetValue(BorderWidthProperty); }
            set { SetValue(BorderWidthProperty, value); }
        }


        public static readonly BindableProperty CornerRadiusProperty =
      BindableProperty.Create(
          nameof(CornerRadius),
          typeof(double),
          typeof(CustomEntryText),
          Device.OnPlatform<double>(6, 7, 7));

        // Gets or sets CornerRadius value
        public double CornerRadius
        {
            get { return (double)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly BindableProperty IsCurvedCornersEnabledProperty =
        BindableProperty.Create(
            nameof(IsCurvedCornersEnabled),
            typeof(bool),
            typeof(CustomEntryText),
            true);

        // Gets or sets IsCurvedCornersEnabled value
        public bool IsCurvedCornersEnabled
        {
            get { return (bool)GetValue(IsCurvedCornersEnabledProperty); }
            set { SetValue(IsCurvedCornersEnabledProperty, value); }
        }

        public static readonly BindableProperty ImageProperty =
            BindableProperty.Create(nameof(Image), typeof(string), typeof(CustomEntryText), string.Empty);

        public static readonly BindableProperty LineColorProperty =
            BindableProperty.Create(nameof(LineColor), typeof(Xamarin.Forms.Color), typeof(CustomEntryText), Color.White);

        public static readonly BindableProperty ImageHeightProperty =
            BindableProperty.Create(nameof(ImageHeight), typeof(int), typeof(CustomEntryText), 40);

        public static readonly BindableProperty ImageWidthProperty =
            BindableProperty.Create(nameof(ImageWidth), typeof(int), typeof(CustomEntryText), 40);

        public static readonly BindableProperty ImageAlignmentProperty =
            BindableProperty.Create(nameof(ImageAlignment), typeof(ImageAlignment), typeof(CustomEntryText), ImageAlignment.Left);


        public static readonly BindableProperty ImageOpacityProperty =
            BindableProperty.Create(nameof(ImageOpacity), typeof(int), typeof(CustomEntryText), 1);

        public Color LineColor
        {
            get { return (Color)GetValue(LineColorProperty); }
            set { SetValue(LineColorProperty, value); }
        }

        public int ImageWidth
        {
            get { return (int)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        public int ImageHeight
        {
            get { return (int)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        public string Image
        {
            get { return (string)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public ImageAlignment ImageAlignment
        {
            get { return (ImageAlignment)GetValue(ImageAlignmentProperty); }
            set { SetValue(ImageAlignmentProperty, value); }
        }

        public int ImageOpacity
        {
            get { return (int)GetValue(ImageOpacityProperty); }
            set { SetValue(ImageOpacityProperty, value); }
        }

        public void OnFocusEsp()
        {
            this.Focus();
        }
    }

    public enum ImageAlignment
    {
        Left,
        Right
    }
}
