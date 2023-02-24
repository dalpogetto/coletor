using CollectorQi.Droid.Notification;
using CollectorQi.Models;
using Xamarin.Forms;

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