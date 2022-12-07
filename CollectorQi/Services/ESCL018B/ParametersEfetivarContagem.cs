using CollectorQi.Services.ESCL000;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using ESCL = CollectorQi.Models.ESCL018;
using Xamarin.Forms;
using CollectorQi.Resources;
using static CollectorQi.Services.ESCL018.ParametersObterLocalizacaoUsuarioService;
using CollectorQi.ViewModels.Interface;
using CollectorQi.Resources.DataBaseHelper.ESCL018;
using CollectorQi.VO.ESCL018;
using CollectorQi.VO.Batch.ESCL018;
using CollectorQi.Resources.DataBaseHelper.Batch.ESCL018;
using AutoMapper;
using CollectorQi.Models.ESCL018;

namespace CollectorQi.Services.ESCL018B
{
    public static class ParametersEfetivarContagem
    {
        // ResultInventarioJson parametros = null;

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;
        //private const string URI = "https://brspupapl01.ad.diebold.:854";
        //private const string URI_GET_PARAMETERS = "/api/integracao/coletores/v1/escl002api/ObterParametros";
        //private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl002api/EnviarParametros";

        //private const string URI = "https://62b47363a36f3a973d34604b.mockapi.io";
        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl018api/EfetivarContagem";

        public async static Task<string> SendInventarioBatchAsync(InventarioItemVO inventarioItemVO)
        {

            var inventarioBarra = new InventarioItemBarra()
            {
                IdInventario = inventarioItemVO.InventarioId,
                Lote = inventarioItemVO.Lote.Trim(),
                Localizacao = inventarioItemVO.Localizacao.Trim(),
                CodItem = inventarioItemVO.CodItem.Trim(),
                CodDepos = inventarioItemVO.CodDepos.Trim(),
                QuantidadeDigitada = int.Parse(inventarioItemVO.Quantidade.ToString()),
                CodEmp = "1",
                Contagem = 1,
                CodEstabel = inventarioItemVO.CodEstabel,
                CodigoBarras = inventarioItemVO.CodigoBarras

            };

            await SendInventarioAsync(inventarioBarra, inventarioItemVO, 0, null);

            return "Inventário Integrado com sucesso";

            //return Task.Str<"Inventário Integrado com sucesso">;
        }

        public static async Task<ResultSendInventarioReturnJson> SendInventarioAsync(InventarioItemBarra requestParam, InventarioItemVO byInventarioItemVO, int inventarioItemId, ContentPage modal)
        {
            ResultSendInventarioReturnJson result = new ResultSendInventarioReturnJson();

            try
            {
                var integraERP = await SendInventarioAsyncERP(requestParam);

                if (integraERP.Retorno == "OK")
                {
                    result.Retorno = integraERP.Retorno;
                    result.Localizacao = requestParam.Localizacao;
                    result.Item = requestParam.CodItem;

                    InventarioItemDB.DeletarInventarioByInventarioId(byInventarioItemVO);
                }
            }
            catch (Exception e)
            {
                if (e.Message == "Unauthorized")
                {
                    LoginPageInterface.ShowModalLogin(modal);
                }
                else
                {

                    try
                    {
                        InventarioItemDB.AtualizaInventarioItemBatch(byInventarioItemVO, eStatusInventarioItem.ErroIntegracao);
                        var batchInventarioItem = Mapper.Map<InventarioItemVO, BatchInventarioItemVO>(byInventarioItemVO);
                        await BatchInventarioItemDB.AtualizaBatchInventario(batchInventarioItem);

                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Write(e);
                    }
                    result.Retorno = "IntegracaoBatch";
                }
            }

            return result;

            // return lstInventarioItemVO;
        }

        // Metodo ObterParametros Totvs
        private static async Task<ResultSendInventarioReturnJson> SendInventarioAsyncERP(InventarioItemBarra requestParam)
        {
            ResultSendInventarioReturnJson parametros = null;
            try
            {
                RequestInventarioBarraJson requestJson = new RequestInventarioBarraJson() { Param = requestParam };

                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());
                //client.BaseAddress = new Uri(URI);

                // Substituir por user e password
                var byteArray = new UTF8Encoding().GetBytes($"{SecurityAuxiliar.GetUsuarioNetwork()}:{SecurityAuxiliar.CodSenha}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var json = JsonConvert.SerializeObject(requestJson);

                client.DefaultRequestHeaders.Add("CompanyId", SecurityAuxiliar.GetCodEmpresa());
                client.DefaultRequestHeaders.Add("x-totvs-server-alias", ServiceCommon.SystemAliasApp);

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
                            parametros = JsonConvert.DeserializeObject<ResultSendInventarioReturnJson>(responseData);
                        }
                        else
                        {
                            var parametroSuccess = JsonConvert.DeserializeObject<ResultSendInventarioSuccessJson>(responseData);

                            parametros = new ResultSendInventarioReturnJson()
                            {
                                Retorno = parametroSuccess.Retorno
                            };
                        }

                    }
                    else
                    {
                        ErroConnectionERP.ValidateConnection(result.StatusCode);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e);
                throw e;
            }

            return parametros;
        }

        public class RequestInventarioJson
        {
            [JsonProperty("Inventario")]
            public InventarioItem Param { get; set; }
        }

        public class RequestInventarioBarraJson
        {
            [JsonProperty("Inventario")]
            public InventarioItemBarra Param { get; set; }
        }


        public class ResultSendInventarioSuccessJson
        {
            public string Retorno { get; set; }
        }


        public class ResultSendInventarioReturnJson : ResultEfetivaContagemSuccess
        {
            [JsonProperty("Conteudo")]

            public List<ResultSendInventarioErrorJson> Resultparam { get; set; }

            public string Retorno { get; set; }
        }

        public class ResultEfetivaContagemSuccess
        {
            public string Localizacao { get; set; }
            public string Item { get; set; }
        }

        public class ResultSendInventarioErrorJson
        {
            public string ErrorDescription { get; set; }
            public string ErrorHelp { get; set; }
        }

        public class ParametrosLimparLeituraResult
        {
            public string Error { get; set; }
        }

    }
}
