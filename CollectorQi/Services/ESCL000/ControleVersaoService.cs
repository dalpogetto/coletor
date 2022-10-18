using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CollectorQi.Services.ESCL000
{
    public interface IControleVersaoService
    {
        Task<ValidaAplicativoResponse> ValidaAplicativoAsync(ValidaAplicativoRequest request);
    }

    public class ControleVersaoService : IControleVersaoService
    {
        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;
        //private const string URI = "https://630e0869b37c364eb7116139.mockapi.io";

        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl000api/ValidarAPK";

        public async Task<ValidaAplicativoResponse> ValidaAplicativoAsync(ValidaAplicativoRequest request)
        {
            try
            {
                var client = new HttpClient(DependencyService
                    .Get<IHTTPClientHandlerCreationService>()
                    .GetInsecureHandler());
                client.BaseAddress = new Uri(URI);

                var json = JsonConvert.SerializeObject(request);

                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, URI_SEND_PARAMETERS)
                    {
                        Content = content
                    };

                    var result = await client.SendAsync(req);

                    if (result.IsSuccessStatusCode)
                    {
                        string responseData = await result.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<ValidaAplicativoResponse>(responseData);
                    }
                    else
                    {
                        Debug.Write(result);
                        return default;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Write(e);
                return default;
            }
        }
    }

    public class ValidaAplicativoRequest
    {
        public string Modelo { get; set; }
        public string Imei { get; set; }
        public string Versao { get; set; }
        public string VersaoAndroid { get; set; }
    }

    public class ValidaAplicativoResponse
    {
        public IList<ValidaAplicativoInfo> APKInfo { get; set; } = new List<ValidaAplicativoInfo>();
        public string id { get; set; }
    }

    public class ValidaAplicativoInfo
    {
        [JsonProperty("loginValidado")]
        public bool LoginValidado { get; set; }
        [JsonProperty("versaoMobile")]
        public string VersaoMobile { get; set; }
        [JsonProperty("linkVersao")]
        public string LinkVersao { get; set; }
        [JsonProperty("mensagem")]
        public string Mensagem { get; set; }
        [JsonProperty("usuarioLogado")]
        public string UsuarioLogado { get; set; }
        [JsonProperty("versao")]
        public string Versao { get; set; }
    }
}
