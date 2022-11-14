using CollectorQi.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ESCL = CollectorQi.Models.ESCL018;
using CollectorQi.Services.ESCL000;

namespace CollectorQi.Services.ESCL018
{
    public class ParametersImprimirEtiquetaService
    {

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;
        //private const string URI_GET_PARAMETERS = "/api/integracao/coletores/v1/escl002api/ObterParametros";
        //private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl002api/EnviarParametros";

        //private const string URI = "https://62b47363a36f3a973d34604b.mockapi.io";
        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl018api/ImprimirEtiqueta";

        // Metodo ObterParametros Totvs
        public async static Task<ResultImpressaoReturnJson> SendImpressaoAsync(ESCL.ImpressaoItem impressaoItem, ESCL.ImpressaoLocalizacao impressaoLocalizacao, ESCL.ImpressaoReparo impressaoReparo, int opcaoImpressao)
        {

            ResultImpressaoReturnJson parametros = null;

            try
            {
                RequestInventarioJson requestJson = new RequestInventarioJson() {
                    OpcaoImpressao = opcaoImpressao.ToString(),
                    Item = impressaoItem ,
                    Localizacao = impressaoLocalizacao,
                    Reparo = impressaoReparo
                };

                RequestImpressaoSendJson requestJsonSend = new RequestImpressaoSendJson()
                {
                    Impressao = requestJson
                };

                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());
                //client.BaseAddress = new Uri(URI);

                var byteArray = new UTF8Encoding().GetBytes($"{SecurityAuxiliar.GetUsuarioNetwork()}:{SecurityAuxiliar.CodSenha}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
               
                var json = JsonConvert.SerializeObject(requestJsonSend);
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

                        if (responseData.Contains("Error"))
                        {
                            parametros = JsonConvert.DeserializeObject<ResultImpressaoReturnJson>(responseData);
                        }
                        else
                        {
                            var parametroSuccess = JsonConvert.DeserializeObject<ResultImpressaoSuccessJson>(responseData);

                            parametros = new ResultImpressaoReturnJson()
                            {
                                Retorno = parametroSuccess.Retorno
                            };
                        }

                        System.Diagnostics.Debug.Write(parametros);
                    }
                    else                    
                        System.Diagnostics.Debug.Write(result);                    
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
            [JsonProperty("Item")]
            public ESCL.ImpressaoItem Item { get; set; }
            [JsonProperty("Localizacao")]
            public ESCL.ImpressaoLocalizacao Localizacao { get; set; }
            [JsonProperty("Reparo")]
            public ESCL.ImpressaoReparo Reparo { get; set; }
            public string OpcaoImpressao { get; set; }            
        }

        public class RequestImpressaoSendJson
        {
            public RequestInventarioJson Impressao { get; set; }
        }
        public class ResultImpressaoReturnJson
        {
            [JsonProperty("Conteudo")]

            public List<ParametrosImpressaoResultError> Resultparam { get; set; }

            public string Retorno { get; set; }
        }

        public class ResultImpressaoSuccessJson
        {
            public string Retorno { get; set; }
        }

        /*
        public class ParametrosImpressaoResult
        {
            public string Retorno { get; set; }
            [JsonProperty("")]
            public List<ParametrosImpressaoResultError> erro { get; set; }
        }
        */

        public class ParametrosImpressaoResultError
        {
            public string ErrorDescription { get; set; }
            public string ErrorHelp { get; set; }
        }


        //public class ParametrosItemImpressaoResult
        //{  
        //    public string CodEstabel { get; set; }
        //    public string CodDeposito { get; set; }
        //    public string CodItem { get; set; }
        //    public int Quantidade { get; set; }
        //    public int QtdeEtiqueta { get; set; }        
        //}

        //public class ParametrosLocalizacaoImpressaoResult
        //{
        //    public string CodEstabel { get; set; }
        //    public string CodDeposito { get; set; }
        //    public string Localizacao { get; set; }
        //    public string AreaIni { get; set; }
        //    public string AreaFim { get; set; }
        //    public string RuaIni { get; set; }
        //    public string RuaFim { get; set; }
        //}

        //public class ParametrosReparoImpressaoResult
        //{
        //    public string CodigoBarras { get; set; }
        //    public string CodEstabel { get; set; }
        //    public string CodFilial { get; set; }
        //    public string CodItem { get; set; }
        //    public int NumRR { get; set; }
        //    public int Digito { get; set; }            
        //}
    }
}