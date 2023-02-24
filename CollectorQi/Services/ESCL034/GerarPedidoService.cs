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

namespace CollectorQi.Services.ESCL034
{
    public static class GerarPedidoService
    {

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;
        //private const string URI = "https://62b47363a36f3a973d34604b.mockapi.io";
        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl034api/GerarPedido";

        // Metodo ObterParametros Totvs
        public static async Task<ResultConteudoJson> GerarPedido(GerarPedidoService.GerarPedidoParam pParam,  List<GerarPedidoListaReparos> pLstListaReparos)
        {

            ResultConteudoJson parametros = null;
            
            try
            {

                /*
                ObterLocalizSend requestParam = new ObterLocalizSend() {
                    CodDepos = pCodDepos,
                    CodEstabel = SecurityAuxiliar.GetCodEstabel() 
                };
                */
                //ObterLocalizSendJson requestJson = new ObterLocalizSendJson() { Param = requestParam };

                GerarPedidoJsonResponse requestJson = new GerarPedidoJsonResponse {
                    ListaReparos = pLstListaReparos,
                    Parametros = pParam
                };

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

                    /*
                    if (result.IsSuccessStatusCode)
                    {
                        string responseData = await result.Content.ReadAsStringAsync();

                        System.Diagnostics.Debug.Write(parametros);

                        parametros = JsonConvert.DeserializeObject<ResulLocalizJson>(responseData);
                    }
                    else
                    {
                        //   ErroConnectionERP.ValidateConnection(result.StatusCode);
                    }
                    */

                    if (result.IsSuccessStatusCode)
                    {
                        string responseData = await result.Content.ReadAsStringAsync();

                        if (responseData.Contains("Error"))
                        {
                            parametros = JsonConvert.DeserializeObject<ResultConteudoJson>(responseData);
                        }
                        else
                        {
                            var parametrosSuccess = JsonConvert.DeserializeObject<ResultConteudoSuccessJson>(responseData);

                            parametros = new ResultConteudoJson()
                            {
                                Retorno = parametrosSuccess.Retorno,
                                ConteudoPedido = parametrosSuccess.Conteudo
                                //ParamReparo = parametrosSuccess.ParamReparo
                            };
                        }
                    }
                }
                
            }
            catch (Exception e)
            {
                throw e;
            }

            return parametros;
        }

        public class GerarPedidoParam : GerarPedidoEtiqueta.GerarPedidoParametros
        {
            public string Peso { get; set; }
            public string Volumes { get; set; }
        }
        
        public class GerarPedidoListaReparos
        {
            public string RowId { get; set; }
            public string VlOrcado { get; set; }
        }

        public class GerarPedidoJsonResponse
        {
            public GerarPedidoParam Parametros { get; set; }
            public List<GerarPedidoListaReparos> ListaReparos { get; set; }
        }

        public class ResultConteudoJson
        {
           // public ResultReparoJson ParamReparo { get; set; }
            public string Retorno { get; set; }
            public string Id { get; set; }

            [JsonProperty("Conteudo")]
            public List<ResultSendErrorJson> Resultparam { get; set; }

            public ResultPedidoJson ConteudoPedido { get; set; }
        }

        public class ResultSendErrorJson
        {
            public string ErrorDescription { get; set; }
            public string ErrorHelp { get; set; }
        }

        public class ResultConteudoSuccessJson
        {
           [JsonProperty("Conteudo")]
            public ResultPedidoJson Conteudo { get; set; }
            public string Retorno { get; set; }
        }

        public class ResultPedidoJson
        {
            [JsonProperty("Pedido")]
            public PedidoList Pedido { get; set; }
        }

        public class PedidoList
        {
            public string NrPedCli { get; set; }
            public string NomeAbrev { get; set; }
            public string Embarque { get; set; }
        }

        /*
        public class ResulLocalizJson
        {
            [JsonProperty("Conteudo")]
            public ResultLocalizacoesJson param { get; set; }
        }

        public class ResultLocalizacoesJson
        {
            [JsonProperty("Localizacoes")]
            public List<ResultLocalizacao> Resultparam { get; set; }
        }

        public class ResultLocalizacao
        {
            [JsonProperty("CodLocaliz")]
            public string CodLocaliz { get; set; }
        }

        public class ObterLocalizSend
        {
            public string CodEstabel { get; set; }
            public string CodDepos { get; set; }
        }
        public class ObterLocalizSendJson
        {
            [JsonProperty("Param")]
            public ObterLocalizSend Param { get; set; }
        }*/
    }
}