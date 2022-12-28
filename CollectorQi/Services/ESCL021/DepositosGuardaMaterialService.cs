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
using CollectorQi.Services.ESCL000;

namespace CollectorQi.Services.ESCL021
{
    public static class DepositosGuardaMaterialService
    {


        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;
        //private const string URI = "https://62fa31c73c4f110faa941620.mockapi.io";        

        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl021api/ObterDepositosUsuarioPorTransacao";

        // Metodo ObterParametros Totvs
        //public async Task<ResultGuardaMaterialJson> SendFinalizarConferenciaAsync(ESCL.FinalizarConferenciaNotaFiscal finalizarConferenciaNotaFiscal)
        public static async Task<ResultGuardaMaterialJson> SendGuardaMaterialAsync()
        {
            ResultGuardaMaterialJson parametros = null;

            try
            {
                //ESCL.ParametrosNotaFiscal requestParam = new ESCL.ParametrosNotaFiscal() { CodEstabel = "126" };
                //RequestGuardaMaterialJson requestJson = new RequestGuardaMaterialJson() { Param = finalizarConferenciaNotaFiscal };
                RequestGuardaMaterialJson requestJson = new RequestGuardaMaterialJson();

                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());
                client.BaseAddress = new Uri(URI);

                // Substituir por user e password
                var byteArray = new UTF8Encoding().GetBytes($"{SecurityAuxiliar.GetUsuarioNetwork()}:{SecurityAuxiliar.CodSenha}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                client.DefaultRequestHeaders.Add("CompanyId", SecurityAuxiliar.GetCodEmpresa());
                client.DefaultRequestHeaders.Add("x-totvs-server-alias", ServiceCommon.SystemAliasApp);

                var json = JsonConvert.SerializeObject(requestJson);

                // using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                //{
                // HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, URI + URI_SEND_PARAMETERS + "?codEstabel=101&tipoTransacao=1")
                // {
                //     Content = content
                // };

                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, URI + URI_SEND_PARAMETERS + $"?codEstabel={SecurityAuxiliar.GetCodEstabel()}&tipoTransacao=1");

                var result = await client.SendAsync(req);

                    if (result.IsSuccessStatusCode)
                    {
                        string responseData = await result.Content.ReadAsStringAsync();
                        parametros = JsonConvert.DeserializeObject<ResultGuardaMaterialJson>(responseData);
                    }
                    else
                    {
                        Debug.Write(result);
                    }
             //   }
            }
            catch (Exception e)
            {
                Debug.Write(e);
            }

            return parametros;
        }

        public class RequestGuardaMaterialJson
        {
            //[JsonProperty("Parametros")]
            //public string CodEstabel { get; set; }
        }

        public class ResultGuardaMaterialJson
        {
            [JsonProperty("Conteudo")]
            public ResultDepositosJson Param { get; set; }
            public string Retorno { get; set; }
        }

        public class ResultDepositosJson
        {
            [JsonProperty("Depositos")]
            public List<DepositosGuardaMaterial> ParamResult { get; set; }
        }
    }
}