using CollectorQi.Models.ESCL018;
using CollectorQi.Resources;
using CollectorQi.Resources.DataBaseHelper.ESCL018;
using CollectorQi.ViewModels.Interface;
using CollectorQi.VO.ESCL018;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static CollectorQi.Services.ESCL018.ParametersObterLocalizacaoUsuarioService;
using CollectorQi.Services.ESCL000;

namespace CollectorQi.Services.ESCL027
{
    public static class Item
    {

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;
        //private const string URI = "https://62b47363a36f3a973d34604b.mockapi.io";
        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl027api/ObterLocalizDoItemPorEstabDep";

        // Metodo ObterParametros Totvs
        public static async Task<ResulLocalizItemJson> ObterLocalizDoItemPorEstabDep(string pCodDepos, string pCodLocaliz, bool pSemSaldo)
        {

            ResulLocalizItemJson parametros = null;

            try
            {
                ObterLocalizItemSend requestParam = new ObterLocalizItemSend() {
                    CodDepos = pCodDepos,
                    CodEstabel = SecurityAuxiliar.GetCodEstabel() ,
                    CodLocaliza = pCodLocaliz,
                    SemSaldo = pSemSaldo
                };

                ObterLocalizItemSendJson requestJson = new ObterLocalizItemSendJson() { Param = requestParam };

                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());
                //client.BaseAddress = new Uri(URI);

                // Substituir por user e password
                var byteArray = new UTF8Encoding().GetBytes($"{SecurityAuxiliar.GetUsuarioNetwork()}:{SecurityAuxiliar.CodSenha}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                client.DefaultRequestHeaders.Add("CompanyId", SecurityAuxiliar.GetCodEmpresa());
                client.DefaultRequestHeaders.Add("x-totvs-server-alias", ServiceCommon.SystemAliasApp);

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
                        string responseData = await result.Content.ReadAsStringAsync();

                        System.Diagnostics.Debug.Write(parametros);

                        parametros = JsonConvert.DeserializeObject<ResulLocalizItemJson>(responseData);
                    }
                    else
                    {
                        //   ErroConnectionERP.ValidateConnection(result.StatusCode);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return parametros;
        }

        public class ResulLocalizItemJson
        {
            [JsonProperty("Conteudo")]
            public ResultLocalizItemJson param { get; set; }
        }

        public class ResultLocalizItemJson
        {
            [JsonProperty("ListaItens")]
            public List<ResultLocalizacaoItem> Resultparam { get; set; }
        }

        public class ResultLocalizacaoItem
        {
            public string Lote { get; set; }
            public decimal SaldoInfo { get; set; }
            public string CodEstabel { get; set; }
            public string CodigoItem { get; set; }
            public string SemSaldo { get; set; }
            public string CodLocaliza { get; set; }
            public string CodDepos { get; set; }
        }

        public class ObterLocalizItemSend
        {
            public string CodEstabel { get; set; }
            public string CodDepos { get; set; }
            public string CodLocaliza { get; set; }
            public bool SemSaldo { get; set; }
        }
        public class ObterLocalizItemSendJson
        {
            [JsonProperty("Param")]
            public ObterLocalizItemSend Param { get; set; }
        }
    }
}