using Android.Net;
using CollectorQi.Droid.WebService;
using CollectorQi.Services;
using Javax.Net.Ssl;
using System.Net.Http;
using Xamarin.Android.Net;
using Xamarin.Forms;

/*
internal class HTTPClientHandlerCreationService_Android
{
    HttpClientHandler GetInsecureHandler();
}
*/


[assembly: Dependency(typeof(HTTPClientHandlerCreationService_Android))]
namespace CollectorQi.Droid.WebService
{
    public class HTTPClientHandlerCreationService_Android : IHTTPClientHandlerCreationService
    {
        public HttpClientHandler GetInsecureHandler()
        {
            return new IgnoreSSLClientHandler();
        }
    }

    internal class IgnoreSSLClientHandler : AndroidClientHandler
    {
        protected override SSLSocketFactory ConfigureCustomSSLSocketFactory(HttpsURLConnection connection)
        {
            return SSLCertificateSocketFactory.GetInsecure(1000, null);
        }
        protected override IHostnameVerifier GetSSLHostnameVerifier(HttpsURLConnection connection)
        {
            return new IgnoreSSLHostnameVerifier();
        }
    }

    internal class IgnoreSSLHostnameVerifier : Java.Lang.Object, IHostnameVerifier
    {
        public bool Verify(string hostname, ISSLSession session)
        {
            return true;
        }
    }
}