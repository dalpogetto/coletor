using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Matcha.BackgroundService.Droid;
using Android.Content;
using Xamarin.Forms;
using Android.Support.V4.App;
using CollectorQi.Views;

namespace CollectorQi.Droid
{
    [Activity(Name= "CollectorQi.Mobile.Droid.MainActivity", Label = "CollectorQi", Icon = "@mipmap/iconsca", /*Theme = "@style/MainTheme"*/ Theme = "@style/MainTheme.Splash" , MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, LaunchMode = LaunchMode.SingleInstance, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static int COUNT_VALUE = 0;
        public static readonly int NOTIFICATION_ID = 1000;
        public static readonly string CHANNEL_ID_TRANSF = "location_notification_transf";
        public static readonly string CHANNEL_ID_INVENTARIO = "location_notification_inventario";
        internal static readonly string COUNT_KEY = "count";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //ZXing.Mobile.MobileBarcodeScanner.Initialize(Application);

       

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            //global::Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);


            BackgroundAggregator.Init(this);

            global::Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            global::ZXing.Mobile.MobileBarcodeScanner.Initialize(Application);

            var idTransferencia = Intent.GetStringExtra("id_transferencia");
            var idInventario = Intent.GetStringExtra("id_inventario");
       
            if (idInventario != null)
                csAuxiliar.idNotify = idInventario;
            else if (idTransferencia != null)
                csAuxiliar.idNotify = idTransferencia;


            if (csAuxiliar.idNotify == null)
            {
                Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);
            }
            else
            {

                Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
                Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAllAsync();
                Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);

            }

            LoadApplication(new App());

            CreateNotificationChannelTransf();
            CreateNotificationChannelInventario();



        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override void OnBackPressed()
        {

            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {

                // Do something if there are some pages in the `PopupStack`
            }
    
            //base.OnBackPressed();
        }

        void CreateNotificationChannelTransf()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                return;
            }

            var channelName         = "notification_services_transf";
            var channelDescription = "Notificaçao de integração";
            var channel = new NotificationChannel(CHANNEL_ID_TRANSF, channelName, NotificationImportance.Default)
            {
                Description = channelDescription
            };

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }


        void CreateNotificationChannelInventario()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                return;
            }

            var channelName = "notification_services_inventario";
            var channelDescription = "Notificaçao de integração";
            var channel = new NotificationChannel(CHANNEL_ID_INVENTARIO, channelName, NotificationImportance.Default)
            {
                Description = channelDescription
            };

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }


        protected override void OnNewIntent(Intent intent)
        {


       //     CollectorQi.Views.handlePop.handlePopUp.OnClose();
        //    CollectorQi.Views.handlePop.handlePopUp.Remo
           // System.Diagnostics.Debug.Write()
             base.OnNewIntent(intent); 
            //NotificationClickedOn(intent);

           // csAuxiliar.IsNotify = false;

        }

    }

}