
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using CollectorQi.Droid.Notification;
using Java.Lang;

[assembly: Xamarin.Forms.Dependency(typeof(DroidClass))]
namespace CollectorQi.Droid.Notification
{
    public class DroidClass
    {
        Context context = Android.App.Application.Context;

        public DroidClass(eTpNotificacao byTpNotification, string byStrMensagem, bool byErro = false)
        {
            if (byTpNotification == eTpNotificacao.Transferencia)
            {
                // Pass the current button press count value to the next activity:

                var valuesForActivity = new Bundle();
                valuesForActivity.PutInt(MainActivity.COUNT_KEY, MainActivity.COUNT_VALUE);

                // When the user clicks the notification, SecondActivity will start up.
                var resultIntent = new Intent(context, typeof(MainActivity));

                // Pass some values to SecondActivity:
                resultIntent.PutExtras(valuesForActivity);


                resultIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);

                resultIntent.PutExtra("id_transferencia", eTpNotificacao.Transferencia.ToString());

                // Construct a back stack for cross-task navigation:
                var stackBuilder = Android.App.TaskStackBuilder.Create(context);
                stackBuilder.AddParentStack(Class.FromType(typeof(MainActivity)));
                stackBuilder.AddNextIntent(resultIntent);

                // Create the PendingIntent with the back stack:
                var resultPendingIntent = stackBuilder.GetPendingIntent(0, (Android.App.PendingIntentFlags)(int)PendingIntentFlags.UpdateCurrent);

                // Build the notification:
                var builder = new NotificationCompat.Builder(context, MainActivity.CHANNEL_ID_TRANSF)
                              .SetAutoCancel(true) // Dismiss the notification from the notification area when the user clicks on it
                              .SetContentIntent(resultPendingIntent) // Start up this activity when the user clicks the intent.
                              .SetContentTitle("(Integração) Transfêrencia") // Set the title
                                                                 //.SetNumber() // Display the count in the Content Info
                              .SetSmallIcon(Resource.Drawable.almoxarifado) // This is the icon to display
                              .SetContentText(byStrMensagem); // the message to display.

                if (byErro)
                    builder.SetSmallIcon(Resource.Drawable.intErroMed);

                // Finally, publish the notification:
                var notificationManager = NotificationManagerCompat.From(context);
                notificationManager.Notify(MainActivity.NOTIFICATION_ID, builder.Build());

                // Increment the button press count:
                MainActivity.COUNT_VALUE++;
            }
            else
            {
                // Pass the current button press count value to the next activity:
                var valuesForActivity = new Bundle();
                valuesForActivity.PutInt(MainActivity.COUNT_KEY, MainActivity.COUNT_VALUE);

                // When the user clicks the notification, SecondActivity will start up.
                var resultIntent = new Intent(context, typeof(MainActivity));

                // Pass some values to SecondActivity:
                resultIntent.PutExtras(valuesForActivity);

                resultIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);

                resultIntent.PutExtra("id_inventario", eTpNotificacao.Inventario.ToString());

                // Construct a back stack for cross-task navigation:
                var stackBuilder = Android.App.TaskStackBuilder.Create(context);
                stackBuilder.AddParentStack(Class.FromType(typeof(MainActivity)));
                stackBuilder.AddNextIntent(resultIntent);

                // Create the PendingIntent with the back stack:
                var resultPendingIntent = stackBuilder.GetPendingIntent(1, (Android.App.PendingIntentFlags)(int)PendingIntentFlags.UpdateCurrent);

                // Build the notification:
                var builder = new NotificationCompat.Builder(context, MainActivity.CHANNEL_ID_INVENTARIO)
                              .SetAutoCancel(true) // Dismiss the notification from the notification area when the user clicks on it
                              .SetContentIntent(resultPendingIntent) // Start up this activity when the user clicks the intent.
                              .SetContentTitle("(Integração) Inventário") // Set the title
                                                                 //.SetNumber() // Display the count in the Content Info
                              .SetSmallIcon(Resource.Drawable.inventario) // This is the icon to display
                              .SetContentText(byStrMensagem); // the message to display.

                if (byErro)
                    builder.SetSmallIcon(Resource.Drawable.intErroMed);

                // Finally, publish the notification:
                var notificationManager = NotificationManagerCompat.From(context);
                notificationManager.Notify(MainActivity.NOTIFICATION_ID + 1 , builder.Build());

                // Increment the button press count:
                MainActivity.COUNT_VALUE++;
            }

        }
    }
}