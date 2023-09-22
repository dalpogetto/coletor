using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CollectorQi.Droid;
using CollectorQi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(AtualizacaoAPK))]
namespace CollectorQi.Droid
{
    public class AtualizacaoAPK : IAtualizacaoAPK
    {
        public void AtualizarAPK(string uri)
        {
           /* 
            var instancia = MainActivity.InstanciaApk;
            Intent promptInstall = new Intent(Intent.ActionView)
                                      .SetDataAndType(Android.Net.Uri.Parse(uri),
                                      "application/vnd.android.package-archive");
            
            instancia.StartActivity(promptInstall);
           */
            

             
        }
    }
}