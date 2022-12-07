using CollectorQi.Models.ESCL017;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using CollectorQi.Services.ESCL000;
using CollectorQi.Resources;

namespace CollectorQi.Services.ESCL017
{
    public static class LeituraEtiquetaInventarioReparoService
    {

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;
      //  private const string URI = "https://62e1257efa99731d75cf5269.mockapi.io";

        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl017api/LeituraEtiqueta";

        // Metodo ObterParametros Totvs
        public static async Task<ResultInventarioJson> SendParametersAsync(ParametrosInventarioReparo parametrosReparo, LeituraEtiquetaInventarioReparo leituraReparo)
        {
            ResultInventarioJson parametros = null;

            try
            {
                var lstLeituraReparo = new List<LeituraEtiquetaInventarioReparo>();

                lstLeituraReparo.Add(leituraReparo);

                RequestInventarioReparoJson requestJson = new RequestInventarioReparoJson() { 
                    ParametrosReparo = parametrosReparo, 
                    LeituraReparo = lstLeituraReparo
                };

                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());
                //client.BaseAddress = new Uri(URI);

                // Substituir por user e password
                var byteArray = new UTF8Encoding().GetBytes($"{SecurityAuxiliar.GetUsuarioNetwork()}:{SecurityAuxiliar.CodSenha}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                client.DefaultRequestHeaders.Add("CompanyId", SecurityAuxiliar.GetCodEmpresa());
                client.DefaultRequestHeaders.Add("x-totvs-server-alias", ServiceCommon.SystemAliasApp);

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
                throw e;
            }

            return parametros;
        }

        public class RequestInventarioReparoJson
        {
            [JsonProperty("Parametros")]
            public ParametrosInventarioReparo ParametrosReparo { get; set; }
            [JsonProperty("ListaReparos")]
            public List<LeituraEtiquetaInventarioReparo> LeituraReparo { get; set; }
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