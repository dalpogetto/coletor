using System.Collections.Generic;
using CollectorQi.Models;
using System.Threading.Tasks;
using Xamarin.Forms;
using Android.Support.V4.App;

using Android.App;
using CollectorQi.Droid.Notification;

[assembly: Dependency(typeof(CollectorQi.Droid.CallNotification))]
namespace CollectorQi.Droid
{
    public sealed class CallNotification : ICallNotification
    {
        public CallNotification()
         {

        }

        public bool callNotification(eTpNotificacao byTpNotificacao, string byStrMensagem, bool byErro = false)
        {
            DroidClass a = new DroidClass(byTpNotificacao, byStrMensagem, byErro);

            return true;
         
        }

    }
}