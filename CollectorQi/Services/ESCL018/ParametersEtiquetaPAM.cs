using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using CollectorQi.Models;
using CollectorQi.Resources;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Services.ESCL000;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace CollectorQi.Services.ESCL018
{
    public static class ParametersEtiquetaPAM
    {
        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;
        private static short filiais;
        private const string URI_ETIQUETA_PAM = "/api/integracao/coletores/v1/escl018api/ObterInfoEtiquetaPAM";

        // Metodo Finalizar Conferencia - Totvs
        public static async Task<ResultEtiquetaPAM> ObterEtiqueta(string pCodBarras)
        {

            ResultEtiquetaPAM parametros = null;

            try
            {
                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());

                client.BaseAddress = new Uri(URI);

                // Substituir por user e password
                var byteArray = new UTF8Encoding().GetBytes($"{SecurityAuxiliar.CodUsuario}:{SecurityAuxiliar.CodSenha}");

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                client.DefaultRequestHeaders.Add("x-totvs-server-alias", ServiceCommon.SystemAliasApp);

                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, URI_ETIQUETA_PAM + "?CodigoBarras=" + pCodBarras);

                var tstEmpresa = SecurityAuxiliar.GetCodEmpresa();

                 client.DefaultRequestHeaders.Add("CompanyId", tstEmpresa);

                var result = await client.SendAsync(req);

                if (result.IsSuccessStatusCode)
                {
                    string responseData = await result.Content.ReadAsStringAsync();

                    /*
                    var resultResponse = JsonConvert.DeserializeObject<ResultFiliais>(responseData);

                    if (resultResponse != null && resultResponse.items != null)
                    {
                        filiais = resultResponse.items;
                    }
                    */

                    if (responseData.Contains("Error"))
                    {
                        parametros = JsonConvert.DeserializeObject<ResultEtiquetaPAM>(responseData);
                    }
                    else
                    {
                        var paramResult = JsonConvert.DeserializeObject<ResultSendPAMJson>(responseData);

                        parametros = paramResult.Resultparam; 

                    }
                }
                else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new Exception("Unauthorized");
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return parametros;
        }


        public class ResultSendPAMJson
        {
            [JsonProperty("Conteudo")]
            public ResultEtiquetaPAM Resultparam { get; set; }
        }

        public class ResultSendInventarioReturnJson
        {
            [JsonProperty("Conteudo")]

            public List<ResultSendInventarioErrorJson> Resultparam { get; set; }

            public string Retorno { get; set; }
        }

        public class ResultEtiquetaPAM
        {
            /*
            public string ErrorDescription { get; set; }
            public string ErrorHelp { get; set; }
            */

            public string Item { get; set; }

            public decimal Qtde { get; set; }
        }

        public class ParametrosLimparLeituraResult
        {
            public string Error { get; set; }
        }
    }
}