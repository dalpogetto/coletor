using CollectorQi.Models.ESCL028;
using CollectorQi.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CollectorQi.Services.ESCL028
{
    public static class ParametersNotaFiscalService
    {

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        //private const string URI = "https://brspupapl01.ad.diebold.com:8543";
        private const string URI = "https://62d19f93d4eb6c69e7e10a56.mockapi.io";

        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl028api/EnviarParametros";

        // Metodo ObterParametros Totvs
        public static async Task<ResultInventarioJson> SendParametersAsync(string pCodEstabel)
        {
            ResultInventarioJson parametros = null;

            try
            {
                ParametrosNotaFiscal requestParam = new ParametrosNotaFiscal() { CodEstabel = pCodEstabel };

                RequestInventarioJson requestJson = new RequestInventarioJson() { Param = requestParam };

                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());
                client.BaseAddress = new Uri(URI);

                // Substituir por user e password
                //var byteArray = new UTF8Encoding().GetBytes($"{SecurityAuxiliar.GetUsuarioNetwork()}:{SecurityAuxiliar.CodSenha}");
                //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var json = JsonConvert.SerializeObject(requestJson);

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
                        Debug.Write(result);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Write(e);
            }

            return parametros;
        }

        public class RequestInventarioJson
        {
            [JsonProperty("Parametros")]
            public ParametrosNotaFiscal Param { get; set; }
        }

        public class ResultInventarioJson
        {
            [JsonProperty("Conteudo")]
            public ResultConteudoJson param { get; set; }
        }

        public class ResultConteudoJson
        {
            [JsonProperty("ListaReparos")]
            public List<ParametrosNotaFiscal> Resultparam { get; set; }
            [JsonProperty("ListaDocumentos")]
            public List<ListaDocumentosNotaFiscal> ListaDocumentos { get; set; }
        }
    }
}