using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using CollectorQi.Services.ESCL000;

namespace CollectorQi.Services.ESCL006
{
    public class ArmazenagemImprimirEtiquetaService
    {
        ResultImpressaoJson parametros = null;

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;

        //private const string URI = "https://6315f4ed5b85ba9b11ebfdc6.mockapi.io";
        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl006api/ImprimirEtiqueta";

        public async Task<ResultImpressaoJson> SendImpressaoAsync(string codFilial)
        {
            try
            {
                RequestImpressaoJson requestJson = new RequestImpressaoJson() { CodFilial = codFilial };

                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());
                client.BaseAddress = new Uri(URI);

                // Substituir por user e password
                //var byteArray = new UTF8Encoding().GetBytes("super:prodiebold11");
                //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                client.DefaultRequestHeaders.Add("x-totvs-server-alias", ServiceCommon.SystemAliasApp);

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
                        parametros = JsonConvert.DeserializeObject<ResultImpressaoJson>(responseData);
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

        public class RequestImpressaoJson
        {
            [JsonProperty("Impressao")]
            public string CodFilial { get; set; }
        }

        public class ResultImpressaoJson
        {
            public string Conteudo { get; set; }
            //[JsonProperty("Conteudo")]
            //public ParametrosImpressaoResult Resultparam { get; set; }
        }

        //public class ParametrosImpressaoResult
        //{
        //    public string OK { get; set; }
        //}       
    }
}