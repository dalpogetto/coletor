using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CollectorQi.Models;
using ESCL = CollectorQi.Models.ESCL018;
using Newtonsoft.Json;
using Xamarin.Forms;
using System.Collections.Generic;
using CollectorQi.Resources;
using static CollectorQi.Services.ESCL018.ParametersFichasUsuarioService;

namespace CollectorQi.Services.ESCL018
{
    public static class ParametersLeituraEtiquetaService
    {
       // ResultInventarioJson parametros = null;

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private const string URI = "https://brspupapl01.ad.diebold.com:8543";
        //private const string URI_GET_PARAMETERS = "/api/integracao/coletores/v1/escl002api/ObterParametros";
        //private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl002api/EnviarParametros";

        //private const string URI = "https://62b47363a36f3a973d34604b.mockapi.io";
        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl018api/LeituraEtiqueta";

        // Metodo ObterParametros Totvs
        public static async Task<ResultInventarioItemJson> SendInventarioAsync(ESCL.InventarioItemBarra requestParam)
        {


            ResultInventarioItemJson parametros = null;
            try
            {
                RequestInventarioBarraJson requestJson = new RequestInventarioBarraJson() { Param = requestParam };

                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());
                //client.BaseAddress = new Uri(URI);

                // Substituir por user e password
                var byteArray = new UTF8Encoding().GetBytes($"{SecurityAuxiliar.GetUsuarioNetwork()}:{SecurityAuxiliar.CodSenha}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var json = JsonConvert.SerializeObject(requestJson);

                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, URI + URI_SEND_PARAMETERS)
                    {
                        Content = content
                    };

                    var result = await client.SendAsync(req);

                    if (result.IsSuccessStatusCode)
                    {
                        string responseData = await result.Content.ReadAsStringAsync();
                        parametros = JsonConvert.DeserializeObject<ResultInventarioItemJson>(responseData);
                    }
                    else
                    {
                        System.Diagnostics.Debug.Write(result);
                    }
                }                
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e);
            }

            return parametros;
        }      

        public class RequestInventarioJson
        {
            [JsonProperty("Inventario")]
            public ESCL.InventarioItem Param { get; set; }
        }

        public class RequestInventarioBarraJson
        {
            [JsonProperty("Inventario")]
            public ESCL.InventarioItemBarra Param { get; set; }
        }

        public class ResultInventarioJson
        {
            [JsonProperty("Conteudo")]
            public ParametrosLimparLeituraResult Resultparam { get; set; }
        }

        public class ParametrosLimparLeituraResult
        {
            public string LimparLeitura { get; set; }
        }
    }
}