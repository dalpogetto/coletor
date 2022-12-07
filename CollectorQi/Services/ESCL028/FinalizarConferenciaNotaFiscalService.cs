using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ESCL = CollectorQi.Models.ESCL028;
using CollectorQi.Services.ESCL000;
using System.Collections.Generic;
using CollectorQi.Resources;

namespace CollectorQi.Services.ESCL028
{
    public class FinalizarConferenciaNotaFiscalService
    {

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;
       // private const string URI = "https://62d19f93d4eb6c69e7e10a56.mockapi.io";        

        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl028api/FinalizarConferencia";

        // Metodo ObterParametros Totvs
        public static async Task<ResultNotaFiscalRetornoJson> SendFinalizarConferenciaAsync(RequestNotaFiscalJson finalizarConferenciaNotaFiscal)
        {
            ResultNotaFiscalRetornoJson parametros = null;

            try
            {

                RequestNotaFiscalJson requestJson = finalizarConferenciaNotaFiscal;

                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());
                
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

                        if (responseData.Contains("Error"))
                        {
                            parametros = JsonConvert.DeserializeObject<ResultNotaFiscalRetornoJson>(responseData);
                        }
                        else
                        {
                            var parametrosSuccess = JsonConvert.DeserializeObject<ResultNotaFiscalRetornoJson>(responseData);

                            parametros = new ResultNotaFiscalRetornoJson()
                            {
                                Retorno = parametrosSuccess.Retorno
                            };
                        }
                    }
                    else
                    {
                        throw new Exception("Ocorreu um erro na na finalização!");
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return parametros;
        }

        public class RequestNotaFiscalJson
        {
            [JsonProperty("Parametros")]
            public Param Parametros { get; set; }

            [JsonProperty("ListaDocumentos")]
            public List<ESCL.FinalizarConferenciaDocumentos> ListaConferenciaDocumentos { get; set; }

            [JsonProperty("ListaReparosConferidos")]
            public List<ESCL.FinalizarConferenciaReparosConferidos> ListaReparosConferidos { get; set; }
        }

        public class Param
        {
            public string CodEstabel { get; set; }
        }

        public class ResultNotaFiscalJson
        {
            [JsonProperty("Conteudo")]
            public ResultConteudoJson Param { get; set; }
            public string Retorno { get; set; }
        }


        public class ResultNotaFiscalRetornoJson
        {
            [JsonProperty("Conteudo")]
            public List<ResultSendInventarioErrorJson> Resultparam { get; set; }

            public string Retorno { get; set; }
        }


        public class ResultConteudoJson
        {           
            public string Mensagem { get; set; }
        }
        public class ResultSendInventarioErrorJson
        {
            public string ErrorDescription { get; set; }
            public string ErrorHelp { get; set; }
        }

    }
}