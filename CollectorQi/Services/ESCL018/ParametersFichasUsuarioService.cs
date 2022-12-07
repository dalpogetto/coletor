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

namespace CollectorQi.Services.ESCL018
{
    public static class ParametersFichasUsuarioService
    {

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;
        //private const string URI = "https://62b47363a36f3a973d34604b.mockapi.io";
        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl018api/ObterFichasUsuario";

        public static async Task<List<InventarioItemVO>> GetObterFichasUsuarioAsync(int byInventarioId, string byLocalizacao, ContentPage modal)
        {

            List<InventarioItemVO> lstInventarioItemVO = new List<InventarioItemVO>();

            try
            {
                // api
                var itemERP = await GetObterFichasUsuarioAsyncERP(byInventarioId, byLocalizacao);

                // Atualiza localizacaoInventario Backend
                if (itemERP != null && itemERP.param != null && itemERP.param.Resultparam != null)
                {
                    itemERP.param.Resultparam.ForEach(delegate (FichasUsuario row)
                    {
                        lstInventarioItemVO.Add(new InventarioItemVO
                        {
                           // InventarioItemId = row.In
                            InventarioId = byInventarioId,
                            Lote = row.Lote,
                            CodEstabel = row.CodEstabel,
                            Localizacao = row.Localizacao,
                            CodItem = row.CodItem,
                            Contagem = row.Contagem,
                            Serie = row.Serie,
                            IVL = row.IVL,
                            CodEmp = row.CodEmp,
                            CodDepos = row.CodDepos,
                            Quantidade = row.Quantidade

                        });
                    });

                    lstInventarioItemVO = await InventarioItemDB.AtualizaInventarioItem(byInventarioId, byLocalizacao, lstInventarioItemVO);

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
                    lstInventarioItemVO = InventarioItemDB.GetInventarioItemByInventarioLocalizacao(byInventarioId, byLocalizacao);
                }
            }

            return lstInventarioItemVO;
        }

        // Metodo ObterParametros Totvs
        private static async Task<ResultInventarioItemJson> GetObterFichasUsuarioAsyncERP(int pInventarioId, string pLocalizacao)
        {

            ResultInventarioItemJson parametros = null;

            try
            {
                FichasUsuarioSend requestParam = new FichasUsuarioSend() { IdInventario = pInventarioId , Localizacao = pLocalizacao};

                RequestInventarioItemJson requestJson = new RequestInventarioItemJson() { Param = requestParam };
                
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
                        parametros = JsonConvert.DeserializeObject<ResultInventarioItemJson>(responseData);
                    }
                    else
                    {
                        ErroConnectionERP.ValidateConnection(result.StatusCode);
                    }
            }                
            }
            catch (Exception e)
            {
                throw e;
            }

            return parametros;
        }

        public class RequestInventarioItemJson
        {
            [JsonProperty("Inventario")]
            public FichasUsuarioSend Param { get; set; }
        }

        public class ResultInventarioItemJson
        {
            [JsonProperty("Conteudo")]
            public ResultConteudoJson param { get; set; }
        }

        public class ResultConteudoJson
        {
            [JsonProperty("FichasUsuario")]
            public List<FichasUsuario> Resultparam { get; set; }
        }

        public class FichasUsuarioSend
        {
            public int IdInventario { get; set; }
            public string Localizacao { get; set; }
          /*  public string Lote { get; set; }
            public string CodEstabel { get; set; }
            public string Localizacao { get; set; }
            public string CodItem { get; set; }
            public int Contagem { get; set; }
            public string Serie { get; set; }
            public int IVL { get; set; }
            public string CodEmp { get; set; }
            public string CodDepos { get; set; }
            public decimal Quantidade { get; set; } */
        }   
    }
}