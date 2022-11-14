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
using CollectorQi.Services.ESCL000;
using CollectorQi.Resources;

namespace CollectorQi.Services.ESCL028
{
    public static class ValidarReparosNotaFiscalService
    {

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;
        //private const string URI = "https://62d19f93d4eb6c69e7e10a56.mockapi.io";        

        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl028api/ValidarReparos";

        // Metodo ObterParametros Totvs
        public static async Task<ResultNotaFiscalJson> SendValidarReparosAsync(ESCL.ValidarReparosNotaFiscal validarReparosNotaFiscal)
        {
            ResultNotaFiscalJson parametros = null;


            try
            {

                //validarReparosNotaFiscal.CodEstabel = "126";

                //ESCL.ParametrosNotaFiscal requestParam = new ESCL.ParametrosNotaFiscal() { CodEstabel = "126" };


                Param p = new Param { 
                    CodEstabel = SecurityAuxiliar.GetCodEstabel()
                };

                List<ESCL.ValidarReparosNotaFiscal> v = new List<ESCL.ValidarReparosNotaFiscal>();


                v.Add(validarReparosNotaFiscal);

                RequestNotaFiscalJson requestJson = new RequestNotaFiscalJson() { 
                    Parametros = p,
                    ListaReparos = v };

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

                        parametros = JsonConvert.DeserializeObject<ResultNotaFiscalJson>(responseData);

                        /*
                        if (responseData.Contains("Error"))
                        {
                            parametros = JsonConvert.DeserializeObject<ValidaAplicativoErrorResponse>(responseData);
                        }
                        else
                        {
                            var parametrosSuccess = JsonConvert.DeserializeObject<ValidaAplicativoResponse>(responseData);

                            parametros = new ValidaAplicativoErrorResponse()
                            {
                                Retorno = parametrosSuccess.Retorno,
                                APKInfo = parametrosSuccess.Conteudo.APKInfo
                            };
                        }*/

                    }


                    /*
                    if (result.IsSuccessStatusCode)
                    {
                        string responseData = await result.Content.ReadAsStringAsync();
                        parametros = JsonConvert.DeserializeObject<ResultNotaFiscalJson>(responseData);
                    }
                    else
                    {
                        Debug.Write(result);
                    }
                    */
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

            [JsonProperty("ListaReparos")]
            public List<ESCL.ValidarReparosNotaFiscal> ListaReparos { get; set; }
        }


        public class Param
        {
            public string CodEstabel { get; set; }
        }

        public class ResultNotaFiscalJson
        {
            [JsonProperty("ListaReparos")]
            public List<ESCL.ValidarReparosNotaFiscal> Resultparam { get; set; }

            [JsonProperty("ListaDocumentos")]
            public List<ESCL.ListaDocumentosNotaFiscal> ListaDocumentos { get; set; }

            public string id { get; set; }
        }

        public class ResultInventarioSuccessJson
        {
            public string Retorno { get; set; }
        }

        public class ResultSendInventarioErrorJson
        {
            public string ErrorDescription { get; set; }
            public string ErrorHelp { get; set; }
        }

    }
}