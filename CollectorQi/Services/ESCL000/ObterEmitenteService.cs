using CollectorQi.Resources;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CollectorQi.Services.ESCL000
{
    public static class ObterEmitenteService
    {

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;
        //private const string URI = "https://62b47363a36f3a973d34604b.mockapi.io";
        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl000api/ObterEmitente";

        // Metodo ObterParametros Totvs
        public static async Task<ResultEmitenteJson> ObterEmitente(int pCodEmitente)
        {
            ResultEmitenteJson parametros = null;

            try
            {

                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());
                //client.BaseAddress = new Uri(URI);

                // Substituir por user e password
                var byteArray = new UTF8Encoding().GetBytes($"{SecurityAuxiliar.GetUsuarioNetwork()}:{SecurityAuxiliar.CodSenha}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                client.DefaultRequestHeaders.Add("CompanyId", SecurityAuxiliar.GetCodEmpresa());
                client.DefaultRequestHeaders.Add("x-totvs-server-alias", ServiceCommon.SystemAliasApp);

                // var json = JsonConvert.SerializeObject(requestJson);

                using (StringContent content = null)
                {
                    HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, URI + URI_SEND_PARAMETERS + $"?CodEmitente={pCodEmitente.ToString()}")
                    {
                        //  Content = content
                    };

                    var result = await client.SendAsync(req);

                    if (result.IsSuccessStatusCode)
                    {
                        string responseData = await result.Content.ReadAsStringAsync();

                        parametros = JsonConvert.DeserializeObject<ResultEmitenteJson>(responseData);
                    }
                    else
                    {
                        throw new Exception("Erro " + result.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return parametros;
        }

        public class ResultEmitenteJson
        {
            [JsonProperty("CodEmitente")]
            public string CodEmitente { get; set; }
            public string NomeAbrev { get; set; }
            public string Nome { get; set; }
            public string Endereco { get; set; }
            public string Cidade { get; set; }
            public string Estado { get; set; }
            public string CodTransp { get; set; }
            public string NomeTransp { get; set; }
        }

        /*
        public class ResultLocalizacoesItemJson
        {
            [JsonProperty("Localizacoes")]
            public List<ResultLocalizacaoItem> Resultparam { get; set; }
        }

        public class ResultLocalizacaoItem
        {
            [JsonProperty("CodLocaliz")]
            public string CodLocaliz { get; set; }

            public string SaldoInfo { get; set; }
        }

        public class ObterLocalizItemSend
        {
            public string CodEstabel { get; set; }
            public string CodDepos { get; set; }
            public string CodigoItem { get; set; }
            public bool SemSaldo { get; set; }
        }
        public class ObterLocalizItemSendJson
        {
            [JsonProperty("Param")]
            public ObterLocalizItemSend Param { get; set; }
        } */
    }
}