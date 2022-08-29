using CollectorQi.Models.ESCL021;
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
    public class DepositosGuardaMaterialService
    {
        ResultGuardaMaterialJson parametros = null;

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        //private const string URI = "https://brspupapl01.ad.diebold.com:8543";
        private const string URI = "https://62fa31c73c4f110faa941620.mockapi.io";        

        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl021api/ObterDepositosUsuarioPorTransacao";

        // Metodo ObterParametros Totvs
        //public async Task<ResultGuardaMaterialJson> SendFinalizarConferenciaAsync(ESCL.FinalizarConferenciaNotaFiscal finalizarConferenciaNotaFiscal)
        public async Task<ResultGuardaMaterialJson> SendGuardaMaterialAsync()
        {
            try
            {
                //ESCL.ParametrosNotaFiscal requestParam = new ESCL.ParametrosNotaFiscal() { CodEstabel = "126" };
                //RequestGuardaMaterialJson requestJson = new RequestGuardaMaterialJson() { Param = finalizarConferenciaNotaFiscal };
                RequestGuardaMaterialJson requestJson = new RequestGuardaMaterialJson();

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
                        parametros = JsonConvert.DeserializeObject<ResultGuardaMaterialJson>(responseData);
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