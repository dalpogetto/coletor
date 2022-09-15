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

namespace CollectorQi.Services.ESCL018
{
    public static class ParametersGravarFichasUsuarioService
    {
        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private const string URI = "https://brspupapl01.ad.diebold.com:8143";
        //private const string URI_GET_PARAMETERS = "/api/integracao/coletores/v1/escl002api/ObterParametros";
        //private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl002api/EnviarParametros";

        //private const string URI = "https://62b47363a36f3a973d34604b.mockapi.io";
        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl018api/GravarFichasUsuario";

        // Metodo ObterParametros Totvs
        public static async Task<ResultInventarioItemJson> SendGravarFichasUsuarioAsync(ESCL.InventarioItem requestParam)
        {

            ResultInventarioItemJson parametros = null;


            try
            {
                List<ESCL.InventarioItem> t = new List<ESCL.InventarioItem>();
                t.Add(requestParam);
                RequestInventarioJson requestJson = new RequestInventarioJson() { Param = t };

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
                        System.Diagnostics.Debug.Write(result);                    
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
            [JsonProperty("FichasUsuario")]
            public List<ESCL.InventarioItem> Param { get; set; }
        }

        public class ResultInventarioItemJson
        {
            [JsonProperty("Conteudo")]
            public ParametrosResult paramConteudo { get; set; }
        }

        //public class ResultConteudoJson
        //{
        //    [JsonProperty("FichasUsuario")]
        //    public List<ParametrosItemInventarioResult> Resultparam { get; set; }
        //}    

        public class ParametrosResult
        {            
            public string Ok { get; set; }
        }       
    }
}