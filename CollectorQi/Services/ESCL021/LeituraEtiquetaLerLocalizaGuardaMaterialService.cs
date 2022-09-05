using CollectorQi.Models.ESCL021;
using CollectorQi.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CollectorQi.Services.ESCL021
{
    public static class LeituraEtiquetaLerLocalizaGuardaMaterialService
    {
        

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private const string URI = "https://brspupapl01.ad.diebold.com:8543";
        //private const string URI = "https://6303e29c761a3bce77e090d4.mockapi.io";

        // Utilizar no diebold esse caminho -->  "/api/integracao/coletores/v1/escl027api/LeituraEtiquetaLocaliza";
        //private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl027api/LeituraEtiqueta_LerLocaliz";
        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl027api/LeituraEtiqueta";

        // Metodo ObterParametros Totvs
        public static async Task<ResultImpressaoReturnJson> SendLeituraEtiquetaAsync(DadosLeituraItemGuardaMaterial dadosLeituraItemGuardaMaterial)
        {
            ResultImpressaoReturnJson parametros = null;

            try
            {

                //ESCL.ParametrosNotaFiscal requestParam = new ESCL.ParametrosNotaFiscal() { CodEstabel = "126" };
                RequestDadosLeituraItemJson requestJson = new RequestDadosLeituraItemJson() { Param = dadosLeituraItemGuardaMaterial };

                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());
                client.BaseAddress = new Uri(URI);

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
                        /*
                        string responseData = await result.Content.ReadAsStringAsync();
                        parametros = JsonConvert.DeserializeObject<ResultGuardaMaterialJson>(responseData); */

                        string responseData = await result.Content.ReadAsStringAsync();

                        if (responseData.Contains("Error"))
                        {
                            parametros = JsonConvert.DeserializeObject<ResultImpressaoReturnJson>(responseData);

                            System.Diagnostics.Debug.Write(parametros);
                        }
                        else
                        {
                            var parametroSuccess = JsonConvert.DeserializeObject<ResultGuardaMaterialJson>(responseData);


                            System.Diagnostics.Debug.Write(parametroSuccess);


                            parametros = new ResultImpressaoReturnJson()
                            {
                                Retorno      = parametroSuccess.Retorno,
                                paramRetorno = parametroSuccess.Param.ParamResult
                            };
                        }
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

        public class RequestDadosLeituraItemJson
        {
            [JsonProperty("DadosLeitura")]
            public DadosLeituraItemGuardaMaterial Param { get; set; }
        } 

        public class ResultGuardaMaterialJson
        {
            [JsonProperty("Conteudo")]
            public ResultDepositosItemJson Param { get; set; }
            public string Retorno { get; set; }
        }

        public class ResultDepositosItemJson
        {
            [JsonProperty("ListaItens")]
            public List<DepositosGuardaMaterialItem> ParamResult { get; set; }
        }

        public class ResultImpressaoReturnJson
        {
            [JsonProperty("Conteudo")]

            public List<ParametrosImpressaoResultError> Resultparam { get; set; }

            public string Retorno { get; set; }


            public List<DepositosGuardaMaterialItem> paramRetorno { get; set; }
        }
        public class ParametrosImpressaoResultError
        {
            public string ErrorHelp { get; set; }
            public string ErrorDescription { get; set; }
        }


    }
}