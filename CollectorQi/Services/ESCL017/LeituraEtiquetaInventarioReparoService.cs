using CollectorQi.Models.ESCL017;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CollectorQi.Services.ESCL017
{
    public class LeituraEtiquetaInventarioReparoService
    {
        ResultInventarioJson parametros = null;

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        //private const string URI = "https://brspupapl01.ad.diebold.com:8143";
        private const string URI = "https://62e1257efa99731d75cf5269.mockapi.io";

        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl017api/LeituraEtiqueta";

        // Metodo ObterParametros Totvs
        public async Task<ResultInventarioJson> SendParametersAsync(ParametrosInventarioReparo parametrosReparo, LeituraEtiquetaInventarioReparo leituraReparo)
        {
            try
            {
                //ParametrosNotaFiscal requestParam = new ParametrosNotaFiscal() { CodEstabel = "126" };
                RequestInventarioReparoJson requestJson = new RequestInventarioReparoJson() { ParametrosReparo = parametrosReparo, LeituraReparo = leituraReparo };

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

        public class RequestInventarioReparoJson
        {
            [JsonProperty("Parametros")]
            public ParametrosInventarioReparo ParametrosReparo { get; set; }
            [JsonProperty("ListaReparos")]
            public LeituraEtiquetaInventarioReparo LeituraReparo { get; set; }
        }

        public class ResultInventarioJson
        {
            [JsonProperty("Conteudo")]
            public LeituraEtiquetaInventarioReparoJson Param { get; set; }
            public string Retorno { get; set; }
        }

        public class LeituraEtiquetaInventarioReparoJson
        {
            [JsonProperty("ListaReparos")]
            public List<LeituraEtiquetaInventarioReparo> Resultparam { get; set; }
        }
    }
}