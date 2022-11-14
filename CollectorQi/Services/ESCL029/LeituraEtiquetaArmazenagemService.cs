using CollectorQi.Models.ESCL017;
using CollectorQi.Models.ESCL029;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using CollectorQi.Services.ESCL000;

namespace CollectorQi.Services.ESCL029
{
    public class LeituraEtiquetaArmazenagemService
    {
        ResultConteudoJson parametros = null;

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;
        //private const string URI = "https://63178523ece2736550b57c6c.mockapi.io";

        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl029api/LeituraEtiqueta";

        // Metodo ObterParametros Totvs
        public async Task<ResultConteudoJson> SendLeituraEtiquetaAsync(ParametrosMovimentoReparo leituraMovimentoReparo)
        {
            try
            {
                //ParametrosNotaFiscal requestParam = new ParametrosNotaFiscal() { CodEstabel = "126" };
                RequestParametrosJson requestJson = new RequestParametrosJson() { Param = leituraMovimentoReparo };

                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());
                //client.BaseAddress = new Uri(URI);

                // Substituir por user e password
                var byteArray = new UTF8Encoding().GetBytes("super:prodiebold11");
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

                    if (result.IsSuccessStatusCode)
                    {
                        string responseData = await result.Content.ReadAsStringAsync();
                        parametros = JsonConvert.DeserializeObject<ResultConteudoJson>(responseData);
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

        public class RequestParametrosJson
        {
            [JsonProperty("Reparo")]
            public ParametrosMovimentoReparo Param { get; set; }
        }

        public class ResultConteudoJson
        {
            [JsonProperty("Conteudo")]
            public ResultReparoJson ParamReparo { get; set; }
        }

        public class ResultReparoJson
        {
            [JsonProperty("Reparo")]
            public LeituraMovimentoReparo ParamLeitura { get; set; }
        }
    }
}