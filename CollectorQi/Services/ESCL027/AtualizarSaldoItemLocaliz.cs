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
    public static class SaldoVirtual
    {

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;
        //private const string URI = "https://62b47363a36f3a973d34604b.mockapi.io";
        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl027api/AtualizarSaldoItemLocaliz";

        // Metodo ObterParametros Totvs
        public static async Task<ResultJson> AtualizarSaldoItemLocaliz(AtualizarSaldoVirtualSend pSaldoVirtual)
        {

            ResultJson parametros = null;

            try
            {

                AtualizarSaldoVirtualSendJson requestJson = new AtualizarSaldoVirtualSendJson() { Param = pSaldoVirtual  };

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

                        parametros = JsonConvert.DeserializeObject<ResultJson>(responseData);
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

        public class ResultJson
        {
            [JsonProperty("Conteudo")]
            public ResultJson param { get; set; }

            [JsonProperty("Retorno")]
            public string Retorno { get; set; }
        }



        /*
        public class ResultJson
        {
            [JsonProperty("Localizacoes")]
            public List<ResultLocalizacao> Resultparam { get; set; }
        }

        public class ResultLocalizacao
        {
            [JsonProperty("CodLocaliz")]
            public string CodLocaliz { get; set; }
        }
        */

        public class AtualizarSaldoVirtualSend
        {
            public string CodEstabel { get; set; }

            public string CodigoItem { get; set; }
            public string CodLocaliza { get; set; }

            public string CodDepos { get; set; }

            public string Lote { get; set; } = "";

            public Decimal SaldoInfo { get; set; }

        }
        public class AtualizarSaldoVirtualSendJson
        {
            [JsonProperty("Param")]
            public AtualizarSaldoVirtualSend Param { get; set; }
        }





        /*
        public class RequestInventarioJson
        {
            [JsonProperty("Parametros")]
            public ParametrosInventarioReparo Param { get; set; }
        }

        public class ResultInventarioJson
        {
            public string Retorno { get; set; }
            public string Id { get; set; }

            [JsonProperty("Conteudo")]
            public List<ResultSendInventarioErrorJson> Resultparam { get; set; }

        }

        public class ResultInventarioSuccessJson
        {
            public string Retorno { get; set; }
        }

        public class ResultSendInventarioErrorJson
        {
            public string ErrorDescription { get; set; }
            public string ErrorHelp { get; set; }
        }

        */
    }
}