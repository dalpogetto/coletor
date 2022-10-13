using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ESCL = CollectorQi.Models.ESCL028;
using CollectorQi.Services.ESCL000;

namespace CollectorQi.Services.ESCL028
{
    public class FinalizarConferenciaNotaFiscalService
    {
        ResultNotaFiscalJson parametros = null;

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;
       // private const string URI = "https://62d19f93d4eb6c69e7e10a56.mockapi.io";        

        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl028api/FinalizarConferencia";

        // Metodo ObterParametros Totvs
        public async Task<ResultNotaFiscalJson> SendFinalizarConferenciaAsync(ESCL.FinalizarConferenciaNotaFiscal finalizarConferenciaNotaFiscal)
        {
            try
            {
                finalizarConferenciaNotaFiscal.CodEstabel = "126";

                //ESCL.ParametrosNotaFiscal requestParam = new ESCL.ParametrosNotaFiscal() { CodEstabel = "126" };

                RequestNotaFiscalJson requestJson = new RequestNotaFiscalJson() { Param = finalizarConferenciaNotaFiscal };

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
                        parametros = JsonConvert.DeserializeObject<ResultNotaFiscalJson>(responseData);
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

        public class RequestNotaFiscalJson
        {
            [JsonProperty("Parametros")]
            public string CodEstabel { get; set; }

            [JsonProperty("ListaReparos")]
            public ESCL.FinalizarConferenciaNotaFiscal Param { get; set; }
        }

        public class ResultNotaFiscalJson
        {
            [JsonProperty("Conteudo")]
            public ResultConteudoJson Param { get; set; }
            public string Retorno { get; set; }
        }

        public class ResultConteudoJson
        {           
            public string Mensagem { get; set; }
        }
    }
}