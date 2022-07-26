using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CollectorQi.Models;
using ESCL = CollectorQi.Models.ESCL028;
using Newtonsoft.Json;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Diagnostics;

namespace CollectorQi.Services.ESCL028
{
    public class ValidarReparosNotaFiscalService
    {
        ResultInventarioJson parametros = null;

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        //private const string URI = "https://brspupapl01.ad.diebold.com:8543";
        private const string URI = "https://62d19f93d4eb6c69e7e10a56.mockapi.io";

        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl028api/ValidarReparos";

        // Metodo ObterParametros Totvs
        public async Task<ResultInventarioJson> SendValidarReparosAsync()
        {
            try
            {
                ESCL.ParametrosNotaFiscal requestParam = new ESCL.ParametrosNotaFiscal() { CodEstabel = "126" };

                RequestInventarioJson requestJson = new RequestInventarioJson() { Param = requestParam };

                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());
                client.BaseAddress = new Uri(URI);

                // Substituir por user e password
                //var byteArray = new UTF8Encoding().GetBytes("super:prodiebold11");
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
            public ESCL.ParametrosNotaFiscal Param { get; set; }
        }

        public class ResultInventarioJson
        {
            [JsonProperty("Conteudo")]
            public ResultConteudoJson param { get; set; }
        }

        public class ResultConteudoJson
        {
            [JsonProperty("ListaReparos")]
            public List<ESCL.ParametrosNotaFiscal> Resultparam { get; set; }
        }        
    }
}