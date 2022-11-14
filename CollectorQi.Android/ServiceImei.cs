using Android;
using Android.Content.PM;
using Android.Support.V4.Content;
using CollectorQi.Droid;
using CollectorQi.Services;
using Java.Lang;

[assembly: Xamarin.Forms.Dependency(typeof(ReadPhoneStateService))]
namespace CollectorQi.Droid
{
    public class ReadPhoneStateService : IReadPhoneState
    {
        /// <summary>
        /// Return the device imei
        /// </summary>
        public string GetPhoneIMEI()
        {
            try
            {
                var context = Android.App.Application.Context;

                if (ContextCompat.CheckSelfPermission(context, 
                    Manifest.Permission.ReadPhoneState) == (int)Permission.Granted)
                {
                    var telephonyManager = (Android.Telephony.TelephonyManager)context
                        .GetSystemService(Android.Content.Context.TelephonyService);
                    return telephonyManager.Imei;
                }

                return default;
            }
            catch (Exception ex)
            {
                Android.Util.Log.Warn("DeviceInfo", "Unable to get id: " + ex.ToString());
                return default;
            }
        }
    }
}
