using CollectorQi.Models.ESCL017;
using CollectorQi.Models.ESCL028;
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
    public static class ParametersInventarioReparoService
    {

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;

        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl017api/EnviarParametros";        

        // Metodo ObterParametros Totvs
        public static async Task<ResultInventarioJson> SendParametersAsync(ParametrosInventarioReparo parametrosInventarioReparo)
        {
            ResultInventarioJson parametros = null;

            try
            {
                //ParametrosNotaFiscal requestParam = new ParametrosNotaFiscal() { CodEstabel = "126" };
                RequestInventarioJson requestJson = new RequestInventarioJson() { Param = parametrosInventarioReparo };

                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());

                // Substituir por user e password
                var byteArray = new UTF8Encoding().GetBytes($"{SecurityAuxiliar.GetUsuarioNetwork()}:{SecurityAuxiliar.CodSenha}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var json = JsonConvert.SerializeObject(requestJson);

                client.DefaultRequestHeaders.Add("CompanyId", "1");

                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, URI + URI_SEND_PARAMETERS)
                    {
                        Content = content
                    };

                    var result = await client.SendAsync(req);

                    /*  if (result.IsSuccessStatusCode)
                      {
                          string responseData = await result.Content.ReadAsStringAsync();
                          parametros = JsonConvert.DeserializeObject<ResultInventarioJson>(responseData);
                      }
                      else
                      {
                          Debug.Write(result);
                      } */


                    if (result.IsSuccessStatusCode)
                    {
                        string responseData = await result.Content.ReadAsStringAsync();

                        if (responseData.Contains("Error"))
                        {
                            parametros = JsonConvert.DeserializeObject<ResultInventarioJson>(responseData);
                        }
                        else
                        {
                            var parametrosSuccess = JsonConvert.DeserializeObject<ResultInventarioSuccessJson>(responseData);

                            parametros = new ResultInventarioJson()
                            {
                                Retorno = parametrosSuccess.Retorno
                            }; 
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return parametros;
        }        

        public class RequestInventarioJson
        {
            [JsonProperty("Parametros")]
            public ParametrosInventarioReparo Param { get; set; }
        }

        public class ResultInventarioJson
        {           
            public string Retorno { get; set; }
            public string Id { get; set; }

            [JsonProperty("Conteudo")]
            public List<ResultSendInventarioErrorJson> Resultparam { get; set; }

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