using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using CollectorQi.Models;
using CollectorQi.Resources.DataBaseHelper;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace CollectorQi.Services.ESCL002
{
    public class RepairService
    {
        private IEnumerable<Parametros> parametros;

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private const string URI = "https://brspupapl01.ad.diebold.com:8143";
        private const string URI_VALIDATE_REPAIR = "/api/integracao/coletores/v1/escl002api/ValidarReparos";
        private const string URI_DEL_REPAIR = "/api/integracao/coletores/v1/escl002api/ExcluirReparos";

        // Metodo ValidarReparos - Totvs
        public async Task<List<RepairResult>> ValidateRepairAsync(string user, string password, List<Repair> repairs)
        {
            try
            {

                Parametros requestParam = new Parametros() { UsuarioTotvs = user };
                ValidateRequestJson requestJson = new ValidateRequestJson() { Param = requestParam, Repairs = repairs };

                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());

                client.BaseAddress = new Uri(URI);

                // Substituir por user e password
                var byteArray = new UTF8Encoding().GetBytes("super:prodiebold11");

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var json = JsonConvert.SerializeObject(requestJson);
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, URI_VALIDATE_REPAIR)
                    {
                        Content = content
                    };

                    var result = await client.SendAsync(req);

                    if (result.IsSuccessStatusCode)
                    {
                        string responseData = await result.Content.ReadAsStringAsync();
                        System.Diagnostics.Debug.Write(result);

                        var resultConvert = JsonConvert.DeserializeObject<ValidateResultJsonV2>(responseData);

                        System.Diagnostics.Debug.Write(resultConvert);

                        return resultConvert.Conteudo.RepairResult;
                    }
                    else {
                        // Throw
                    }

                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e);
            }

            return null;
        }


        // Metodo TOTVS - ExcluirReparos
        public async Task<IEnumerable<Parametros>> DelRepairAsync(string usuario_totvs, List<DelRepair> repairsDel)
        {
            try
            {

                DelRequestParameters requestParam = new DelRequestParameters() { UsuarioTotvs = usuario_totvs };
                DelRequestRepair requestJson = new DelRequestRepair() {  Param = requestParam, DelRepairs = repairsDel };

                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());

                client.BaseAddress = new Uri(URI);

                // Substituir por user e password
                var byteArray = new UTF8Encoding().GetBytes("super:prodiebold11");

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var json = JsonConvert.SerializeObject(requestJson);
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    try
                    {
                        HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, URI_DEL_REPAIR)
                        {
                            Content = content
                        };

                        var result = await client.SendAsync(req);

                        if (result.IsSuccessStatusCode)
                        {
                            string responseData = await result.Content.ReadAsStringAsync();
                            System.Diagnostics.Debug.Write(result);

                            var resultConvert = JsonConvert.DeserializeObject<DelRepairResult>(responseData);

                            System.Diagnostics.Debug.Write(resultConvert);
                        }
                        else
                        {
                            // Enviado para Valter validar porque o ExcluirReparo nao retorna 200
                            //var responseData = await result.Content.No();
                            //System.Diagnostics.Debug.Write(responseData);

                    //        var resultConvert = JsonConvert.DeserializeObject<DelRepairResult>(responseData);

                    //        System.Diagnostics.Debug.Write(resultConvert);

                            // Throw
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        System.Diagnostics.Debug.Write(ex);
                    }

                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e);
            }

            return parametros;
        }
        
        public class ValidateResultJsonV2
        {
            [JsonProperty("Conteudo")]
            public ValidateResultJson Conteudo { get; set; }
            public string Retorno { get; set; }
        }

        #region Metodos utilizados para validate
        public class ValidateRequestJson
        {
            [JsonProperty("Parametros")]
            public Parametros Param { get; set; }

            [JsonProperty("ListaReparos")]
            public List<Repair> Repairs { get; set; }
        }

        public class ValidateResultJson
        {
            [JsonProperty("ReparosValidados")]
            public List<RepairResult> RepairResult { get; set; }
        }

        public class Repair
        {
            public string CodBarras { get; set; }
            public string CodEstabel { get; set; }
            public string CodFilial { get; set; }
            public string NumRR { get; set; }
            public string Digito { get; set; }
        }

          
        public class RepairResult : Repair
        {
            public int CodEmitente { get; set; }
            public string RowId { get; set; }
            public string Qtde { get; set; }
            public string Localiza { get; set; }

            public string CodItem { get; set; }

            public string Situcao { get; set; }
            public string Mensagem { get; set; }
            public decimal Valor { get; set; }
            public string Origem { get; set; }
            public string DescItem { get; set; }

        }
        public class ValidateRequestParameters
        {
            public string UsuarioTotvs { get; set; }
        }

        #endregion

        #region Metodos ExcluirReparos

        public class DelRequestParameters
        {
            public string UsuarioTotvs { get; set; }
        }

        public class DelRequestRepair
        {
            [JsonProperty("Parametros")]
            public DelRequestParameters Param { get; set; }

            [JsonProperty("ListaReparos")]
            public List<DelRepair> DelRepairs { get; set; }
        }

        public class DelRepair
        {
            public string RowId { get; set; }
        }

        public class DelRepairResult
        {
            public string message { get; set; }
        }

        #endregion

        /*
        public class ResultJson
        {
            [JsonProperty("Parametros")]
            public ParametrosResult param { get; set; }
        }
        */

        /*
        public class ParametrosResult
        {
            public int CodEmitente { get; set; }
            public string UsuarioTotvs { get; set; }
            public int DiasXML { get; set; }
            public string CodEstabel { get; set; }
            public string NFRet { get; set; }
            public string DtEntrega { get; set; }
            public decimal ValorTotal { get; set; }
            public string Serie { get; set; }
            public decimal QtdeItem { get; set; }
        }
        
        public class ResultSend
        {
            public string Mensagem { get; set; }
        } */
    }
}