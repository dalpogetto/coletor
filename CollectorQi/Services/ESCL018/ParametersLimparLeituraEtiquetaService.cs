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
        public static async Task<ResultSendInventarioReturnJson> SendInventarioAsync(ESCL.InventarioItemBarra requestParam)
        {


            ResultSendInventarioReturnJson parametros = null;
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

                        if (responseData.Contains("Error"))
                        {
                            parametros = JsonConvert.DeserializeObject<ResultSendInventarioReturnJson>(responseData);
                        }
                        else
                        {
                            var parametroSuccess = JsonConvert.DeserializeObject<ResultSendInventarioSuccessJson>(responseData);

                            parametros = new ResultSendInventarioReturnJson()
                            {
                                Retorno = parametroSuccess.Retorno
                            };
                        }

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
                throw e;
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

      
        public class ResultSendInventarioSuccessJson
        {
            public string Retorno { get; set; }
        }


        public class ResultSendInventarioReturnJson
        {
            [JsonProperty("Conteudo")]

            public List<ResultSendInventarioErrorJson> Resultparam { get; set; }

            public string Retorno { get; set; }
        }

        public class ResultSendInventarioErrorJson
        {
            public string ErrorDescription { get; set; }
            public string ErrorHelp { get; set; }
        }

        public class ParametrosLimparLeituraResult
        { 
            public string Error { get; set; }
        }
    }
}