using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CollectorQi.Models;
using ESCL = CollectorQi.Models.ESCL018;
using Newtonsoft.Json;
using Xamarin.Forms;
using System.Collections.Generic;
using CollectorQi.Resources;
using static CollectorQi.Services.ESCL018.ObterLocalizPorEstabDepService;
using static CollectorQi.Services.ESCL018.ParametersObterLocalizacaoUsuarioService;
using CollectorQi.ViewModels.Interface;
using CollectorQi.Resources.DataBaseHelper.ESCL018;
using CollectorQi.VO.ESCL018;
using CollectorQi.VO.Batch.ESCL018;
using CollectorQi.Resources.DataBaseHelper.Batch.ESCL018;
using AutoMapper;
using CollectorQi.Models.ESCL018;
using CollectorQi.Services.ESCL000;
using CollectorQi.Resources.Batch;

namespace CollectorQi.Services.ESCL018
{
    public static class ParametersLeituraEtiquetaServiceB
    {
        // ResultInventarioJson parametros = null;

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;
        //private const string URI = "https://brspupapl01.ad.diebold.:854";
        //private const string URI_GET_PARAMETERS = "/api/integracao/coletores/v1/escl002api/ObterParametros";
        //private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl002api/EnviarParametros";

        //private const string URI = "https://62b47363a36f3a973d34604b.mockapi.io";
        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl018api/LeituraSequencialEtiqueta";

        public async static Task<string> SendInventarioBatchAsync(InventarioItemVO inventarioItemVO)
        {

            var inventarioBarra = new InventarioItemBarra()
            {
                IdInventario = inventarioItemVO.InventarioId,
                Lote = inventarioItemVO.Lote.Trim(),
                Localizacao = inventarioItemVO.Localizacao.Trim(),
                CodItem = inventarioItemVO.CodItem.Trim(),
                CodDepos = inventarioItemVO.__inventario__.CodDepos.Trim(),
                QuantidadeDigitada = decimal.Parse(inventarioItemVO.Quantidade.ToString()),
                CodEmp = SecurityAuxiliar.GetCodEmpresa(),
                Contagem = 1,
                CodEstabel = SecurityAuxiliar.GetCodEstabel(),
                CodigoBarras = inventarioItemVO.CodigoBarras

            };

            await SendInventarioAsync(inventarioBarra, inventarioItemVO, 0, null, 0, false, null);

            return "Inventário Integrado com sucesso";

            //return Task.Str<"Inventário Integrado com sucesso">;
        }

        public static async Task<bool> SendInventarioAsync(ESCL.InventarioItemBarra requestParam, InventarioItemVO byInventarioItemVO , int inventarioItemId, ContentPage modal, decimal quantidadeAcumulada, bool blnCodigoBarras, DadosLeituraEtiquetaPAM convertPAM)
        {
            ResultSendInventarioReturnJson result = new ResultSendInventarioReturnJson();

            try
            {
                // Le o banco, verifica se tem OUTRO item igual 3 e valida..
                //var getItemByCX = InventarioItemDB.GetInventarioItemByItemCx(byInventarioItemVO.InventarioId, byInventarioItemVO.CodItem);

                // if(getItemByCX.Count == 0)
                // {

                /*
                if (!blnCodigoBarras)
                {
                    var sendInventarioERP = await SendInventarioAsyncERP(requestParam);

                    if (sendInventarioERP.Retorno == "OK")
                    {
                        byInventarioItemVO.QuantidadeAcum = quantidadeAcumulada;
                        InventarioItemDB.AtualizaInventarioItemBatch(byInventarioItemVO, eStatusInventarioItem.IntegracaoCX);
                    }

                    result = sendInventarioERP;
                }
                else
                
                {*/
                    InventarioItemCodigoBarrasVO v = new InventarioItemCodigoBarrasVO();

                    v.InventarioItemKey = byInventarioItemVO.InventarioItemKey;
                    v.Quantidade        = byInventarioItemVO.Quantidade;

                    if (convertPAM != null)
                    {
                        v.CodigoBarras        = byInventarioItemVO.CodigoBarras;
                        v.CodEtiquetaDescript = convertPAM.CodEtiqueta24;
                        v.CodEtiqueta         = convertPAM.CodEtiqueta;
                    }

                    await InventarioItemCodigoBarrasDB.InserirEtiqueta(v);
                    await InventarioItemDB.AtualizaInventarioItemBatch(byInventarioItemVO, eStatusInventarioItem.IntegracaoCX);

                    return true;

                // }
                /*  } 
                  else
                  {
                      var itemPendenteDeEfetivacao = getItemByCX[0];

                      result.Retorno = "IntegracaoBatchErrorLeituraEtiqueta";
                      result.Localizacao = itemPendenteDeEfetivacao.Localizacao;
                      result.Item = itemPendenteDeEfetivacao.CodItem;
                  } */
            }
            catch (Exception e)
            {
                throw e;
                /*
                if (e.Message == "Unauthorized")
                {
                    LoginPageInterface.ShowModalLogin(modal);
                }
                else
                {
                    /*
                    if (!blnCodigoBarras) {}
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
                */
            }
           // return lstInventarioItemVO;
        }

        // Metodo ObterParametros Totvs
        private static async Task<ResultSendInventarioReturnJson> SendInventarioAsyncERP(ESCL.InventarioItemBarra requestParam)
        {
            //Alternar para Enviar Formato Decimal para o Progress
            ServiceCommon.SetarAmbienteCulturaUSA();

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
                                Retorno = parametroSuccess.Retorno,
                                Ficha = parametroSuccess.Conteudo.FichasUsuario[0]
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
            finally
            {
                ServiceCommon.SetarAmbienteCulturaBrasil();
            }
            

            return parametros;
        }      

        public class RequestInventarioJson
        {
            [JsonProperty("Inventario")]
            public ESCL.InventarioItem Param { get; set; }
        }

        public class RequestInventarioBarraJson
        {
            [JsonProperty("Inventario")]
            public ESCL.InventarioItemBarra Param { get; set; }
        }

      
        public class ResultSendInventarioSuccessJson
        {
            public string Retorno { get; set; }

            [JsonProperty("Conteudo")]
            public ConteudoRetorno Conteudo { get; set; }
        }

        public class ConteudoRetorno
        {
            public List<FichasUsuario> FichasUsuario { get; set; }
        }

        public class FichasUsuario
        {
            public string Quantidade { get; set; }
        }


        public class ResultSendInventarioReturnJson : ResultSendInventariErrorSequenciaEtiqueta
        {
            [JsonProperty("Conteudo")]

            public List<ResultSendInventarioErrorJson> Resultparam { get; set; }

            public string Retorno { get; set; }

            public FichasUsuario Ficha { get; set; }
        }
        
        public class ResultSendInventariErrorSequenciaEtiqueta
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