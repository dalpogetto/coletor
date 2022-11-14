using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CollectorQi.Models;
using ESCL = CollectorQi.Models.ESCL018;
using Newtonsoft.Json;
using Xamarin.Forms;
using System.Collections.Generic;
using CollectorQi.Services.ESCL000;

namespace CollectorQi.Services.ESCL018
{
    public class ParametersLocalizacaoLeituraEtiquetaService
    {
        //private IEnumerable<Parametros> parametros;

        ResultInventarioJson parametros = null;

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;
        //private const string URI_GET_PARAMETERS = "/api/integracao/coletores/v1/escl002api/ObterParametros";
        //private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl002api/EnviarParametros";

        //private const string URI = "https://62b47363a36f3a973d34604b.mockapi.io";
        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl018api/LeituraEtiqueta_Localiz";

        // Metodo ObterParametros Totvs
        public async Task<ResultInventarioJson> SendInventarioAsync(ESCL.Inventario requestParam)
        {
            try
            {
                RequestInventarioJson requestJson = new RequestInventarioJson() { Param = requestParam };
                
                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());
                client.BaseAddress = new Uri(URI);

                // Substituir por user e password
                //var byteArray = new UTF8Encoding().GetBytes("super:prodiebold11");
                //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var json = JsonConvert.SerializeObject(requestJson);

                client.DefaultRequestHeaders.Add("CompanyId", "1");

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
                        parametros = JsonConvert.DeserializeObject<ResultInventarioJson>(responseData);
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
            public ESCL.Inventario Param { get; set; }
        }      

        public class ResultInventarioJson
        {
            [JsonProperty("Conteudo")]
            public ParametrosInventarioResult Resultparam { get; set; }
        }      

        public class ParametrosInventarioResult
        {
            public string Lote { get; set; }
            public string Localizacao { get; set; }
        }       
    }
}