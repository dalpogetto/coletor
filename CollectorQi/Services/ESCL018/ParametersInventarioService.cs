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
using CollectorQi.VO.ESCL018;
using CollectorQi.ViewModels.Interface;
using CollectorQi.Resources.DataBaseHelper.ESCL018;
using static CollectorQi.Services.ESCL018.ParametersObterLocalizacaoUsuarioService;

namespace CollectorQi.Services.ESCL018
{
    public static class ParametersInventarioService
    {
        //private IEnumerable<Parametros> parametros;


        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private const string URI = "https://brspupapl01.ad.diebold.com:8543";
        //private const string URI_GET_PARAMETERS = "/api/integracao/coletores/v1/escl002api/ObterParametros";
        //private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl002api/EnviarParametros";

        //private const string URI = "https://62b47363a36f3a973d34604b.mockapi.io";
        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl018api/EnviarParametros";


        public static async Task<List<InventarioVO>> SendParametersAsync(ContentPage modal)
        {

            List<InventarioVO> lstInventarioVO = new List<InventarioVO>();

            try
            {
                var inventarioERP = await SendParametersAsyncERP();

                // Atualiza localizacaoInventario Backend
                if (inventarioERP != null && inventarioERP.param != null && inventarioERP.param.Resultparam != null)
                {
                    
                    inventarioERP.param.Resultparam.ForEach(delegate (ESCL.Parametros row)
                    {
                        DateTime dtSaldoRow = DateTime.MinValue;

                        if (!String.IsNullOrEmpty(row.DtSaldo))
                            dtSaldoRow = new DateTime(int.Parse(row.DtSaldo.Substring(6, 2)),
                                                        int.Parse(row.DtSaldo.Substring(3, 2)),
                                                        int.Parse(row.DtSaldo.Substring(0, 2)));


                        lstInventarioVO.Add(new InventarioVO
                        {
                            IdInventario = row.IdInventario,
                            CodEstabel = row.CodEstabel,
                            CodDepos = row.CodDeposito,
                            DescEstabel = row.DescEstabel,
                            DescDepos = row.DescDepos,
                            DtSaldo = dtSaldoRow
                            
                        });
                    }); 

                    InventarioDB.AtualizaInventarioByCodEstabel(SecurityAuxiliar.GetCodEstabel(), lstInventarioVO);
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
                    lstInventarioVO = InventarioDB.GetInventarioByCodEstabel(SecurityAuxiliar.GetCodEstabel());
                }
            }

            return lstInventarioVO;
        }

        // Metodo ObterParametros Totvs
        private static async Task<ResultInventarioJson> SendParametersAsyncERP()
        {
            ResultInventarioJson parametros = null;

            try
            {
                ESCL.Parametros requestParam = new ESCL.Parametros() { CodEstabel = SecurityAuxiliar.GetCodEstabel() };

                RequestInventarioJson requestJson = new RequestInventarioJson() { Param = requestParam };

                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());
                client.BaseAddress = new Uri(URI);

                // Substituir por user e password
                var byteArray = new UTF8Encoding().GetBytes($"{SecurityAuxiliar.GetUsuarioNetwork()}:{SecurityAuxiliar.CodSenha}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

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

                        System.Diagnostics.Debug.Write(parametros);
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
            public int IdInventario { get; set; }
            public string DtSaldo { get; set; }
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