using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CollectorQi.Models;
using ESCL = CollectorQi.Models.ESCL018;
using Newtonsoft.Json;
using Xamarin.Forms;
using System.Collections.Generic;

namespace CollectorQi.Services.ESCL018
{
    public class ParametersItemLeituraEtiquetaService
    {
        ResultInventarioItemJson parametros = null;

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        //private const string URI = "https://brspupapl01.ad.diebold.com:8543";
        //private const string URI_GET_PARAMETERS = "/api/integracao/coletores/v1/escl002api/ObterParametros";
        //private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl002api/EnviarParametros";

        private const string URI = "https://62b47363a36f3a973d34604b.mockapi.io";
        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl018api/LeituraEtiqueta_Item";

        // Metodo ObterParametros Totvs
        public async Task<ResultInventarioItemJson> SendInventarioAsync(ESCL.Inventario requestParam)
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
                        parametros = JsonConvert.DeserializeObject<ResultInventarioItemJson>(responseData);
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

        public class ResultInventarioItemJson
        {
            [JsonProperty("Conteudo")]
            public ResultConteudoJson paramConteudo { get; set; }
        }

        public class ResultConteudoJson
        {
            [JsonProperty("FichasUsuario")]
            public List<ParametrosItemInventarioResult> Resultparam { get; set; }
        }    

        public class ParametrosItemInventarioResult
        {            
            public int IdInventario { get; set; }
            public string Lote { get; set; }
            public string CodEstabel { get; set; }
            public string Localizacao { get; set; }
            public string CodItem { get; set; }
            public int Contagem { get; set; }
            public string Serie { get; set; }
            public int IVL { get; set; }
            public string CodEmp { get; set; }
            public string CodDepos { get; set; }
            public int Quantidade { get; set; }
        }       
    }
}