using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ESCL = CollectorQi.Models.ESCL018;

namespace CollectorQi.Services.ESCL018
{
    public class ParametersImprimirEtiquetaService
    {
        ResultImpressaoJson parametros = null;

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        //private const string URI = "https://brspupapl01.ad.diebold.com:8543";
        //private const string URI_GET_PARAMETERS = "/api/integracao/coletores/v1/escl002api/ObterParametros";
        //private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl002api/EnviarParametros";

        private const string URI = "https://62b47363a36f3a973d34604b.mockapi.io";
        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl018api/ImprimirEtiqueta";

        // Metodo ObterParametros Totvs
        public async Task<ResultImpressaoJson> SendImpressaoAsync(ESCL.Inventario requestParam)
        {
            try
            {
                RequestInventarioJson requestJson = new RequestInventarioJson() { Param = requestParam };
                
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
                        parametros = JsonConvert.DeserializeObject<ResultImpressaoJson>(responseData);
                    }
                    else                    
                        System.Diagnostics.Debug.Write(result);                    
                }                
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e);
            }

            return parametros;
        }      

        public class RequestInventarioJson
        {
            public ESCL.Inventario Param { get; set; }
        }

        public class ResultImpressaoJson
        {
            [JsonProperty("Conteudo")]
            public ParametrosImpressaoResult Resultparam { get; set; }
        }

        public class ParametrosImpressaoResult
        {
            public string OK { get; set; }
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