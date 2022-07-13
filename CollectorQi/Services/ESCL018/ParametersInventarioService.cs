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
    public class ParametersInventarioService
    {
        //private IEnumerable<Parametros> parametros;

        ResultInventarioJson parametros = null;

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        //private const string URI = "https://brspupapl01.ad.diebold.com:8543";
        //private const string URI_GET_PARAMETERS = "/api/integracao/coletores/v1/escl002api/ObterParametros";
        //private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl002api/EnviarParametros";

        private const string URI = "https://62b47363a36f3a973d34604b.mockapi.io";
        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl018api/EnviarParametros";

        // Metodo ObterParametros Totvs
        public async Task<ResultInventarioJson> SendParametersAsync()
        {
            try
            {
                ESCL.Parametros requestParam = new ESCL.Parametros() { CodEstabel = "101" };

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
                        parametros = JsonConvert.DeserializeObject<ResultInventarioJson>(responseData);
                    }
                    else
                    {
                        System.Diagnostics.Debug.Write(result);
                    }
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
            [JsonProperty("Parametros")]
            public ESCL.Parametros Param { get; set; }
        }      

        public class ResultInventarioJson
        {
            [JsonProperty("Conteudo")]
            public ResultConteudoJson param { get; set; }
        }

        public class ResultConteudoJson
        {
            [JsonProperty("ListaInventarios")]
            public List<ESCL.Parametros> Resultparam { get; set; }
        }

        public class ParametrosInventarioResult
        {
            public string DtSaldo { get; set; }
            public int IdInventario { get; set; }
            public string CodEstabel { get; set; }
            public string DescEstabel { get; set; }
            public string CodDeposito { get; set; }
            public string DescDepos { get; set; }
        }    

        //public class ResultSend
        //{
        //    public string Mensagem { get; set; }
        //    public List<ResultRepair> ListaReparos { get; set; }
        //}
        // public class ResultSendParametrosRepair
        //{
        //    public string Mensagem { get; set; }
        //    public ParametrosResult Param { get; set; }
        //    public List<ResultRepair> ListaReparos { get; set; }
        //}

        //public class ResultRepair
        //{
        //    public int CodEmitente { get; set; }
        //    public string RowId { get; set; }
        //    public decimal Qtde { get; set; }
        //    public string CodEstabel { get; set; }
        //    public int Digito { get; set; }
        //    public string Localiza { get; set; }
        //    public string CodItem { get; set; }
        //    public string NumRR { get; set; }
        //    public string Situacao { get; set; }
        //    public string CodFilial { get; set; }
        //    public string Mensagem { get; set; }
        //    public decimal Valor { get; set; }
        //    public string CodBarras { get; set; }
        //    public string Origem { get; set; }
        //    public string DescItem { get; set; }
        //    public string RetornoMsg { get; set; }
        //}
    }
}