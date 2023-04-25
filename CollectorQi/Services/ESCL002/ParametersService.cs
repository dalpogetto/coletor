﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CollectorQi.Models;
using Newtonsoft.Json;
using Xamarin.Forms;
using CollectorQi.Services.ESCL000;

namespace CollectorQi.Services.ESCL002
{
    public class ParametersService
    {
        //private IEnumerable<Parametros> parametros;

        ParametrosResult parametros = null;

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;
        private const string URI_GET_PARAMETERS = "/api/integracao/coletores/v1/escl002api/ObterParametros";
        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl002api/EnviarParametros";

        // Metodo ObterParametros Totvs
        public async Task<ParametrosResult> GetParametersAsync(string user, string password)
        {
            try
            {

               // #if (DEBUG)

               // var result = ParametersServiceMock.GetParametersResult();

               // parametros = result.param;


               // #else
                //return ParametersServiceMock.GetParametersResult();

                Parametros requestParam = new Parametros() { UsuarioTotvs = user };

                RequestJson requestJson = new RequestJson() { Param = requestParam };
                
                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());
                client.Timeout = TimeSpan.FromSeconds(30);

                client.BaseAddress = new Uri(URI);

                // Substituir por user e password
                //var byteArray = new UTF8Encoding().GetBytes("super:prodiebold11");

                var byteArray = new UTF8Encoding().GetBytes("a.alvessouzasilva@DIEBOLD_MASTER:D!ebold2022");

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                client.DefaultRequestHeaders.Add("x-totvs-server-alias", ServiceCommon.SystemAliasApp);

                var json = JsonConvert.SerializeObject(requestJson);
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, URI_GET_PARAMETERS)
                    {
                        Content = content
                    };

                    var result = await client.SendAsync(req);

                    if (result.IsSuccessStatusCode)
                    {
                        string responseData = await result.Content.ReadAsStringAsync();
                        System.Diagnostics.Debug.Write(result);

                        var resultConvert = JsonConvert.DeserializeObject<ResultJsonV2>(responseData);

                        System.Diagnostics.Debug.Write(resultConvert);


                        if (resultConvert != null && resultConvert.Conteudo != null && resultConvert.Conteudo.param != null && resultConvert.Retorno == "OK")
                        {
                            parametros = resultConvert.Conteudo.param;
                        }

                    }
                    else {
                        // Throw
                    }

                }
                //#endif
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e);
            }


            return parametros;
        }

        //public List<ResultRepair> SendParametersAsync(string usuario_totvs, string cod_estabel, int cod_emitente, string dt_entrega, string nf_ret, string serie, decimal qtde_item, decimal valor_total, int dias_xml)
        public async Task<List<ResultRepair>> SendParametersAsync(ParametrosResult requestParam)
        {
            List<ResultRepair> resultRepair = null;
            try
            {

                //#if (DEBUG)
                //var result = ParametersServiceMock.GetRepairs();

                //return result.ListaReparos;

                //#else

/*                ParametrosResult requestParam = new ParametrosResult() { 
                    UsuarioTotvs = usuario_totvs,
                    CodEstabel = cod_estabel,
                    CodEmitente = cod_emitente, 
                    DtEntrada = dt_entrega,
                    NFRet = nf_ret,
                    Serie = serie,  
                    QtdeItem = qtde_item,
                    ValorTotal = valor_total,  
                    DiasXML = dias_xml
                };
*/

                ResultJson requestJson = new ResultJson() { param = requestParam };

                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());

                client.BaseAddress = new Uri(URI);

                // Substituir por user e password
                var byteArray = new UTF8Encoding().GetBytes("super:prodiebold11");

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
                        System.Diagnostics.Debug.Write(result);

                        var resultConvert = JsonConvert.DeserializeObject<ResultSendV2>(responseData);

                        System.Diagnostics.Debug.Write(resultConvert);
                    }
                    else
                    {
                        System.Diagnostics.Debug.Write(result);
                        // Throw
                    }
                }
                //#endif
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e);
            }

            return resultRepair;
        }

        public ParametrosResult GetParametrosRepairAsync(ParametrosResult parametrosResult, List<ResultRepair> ListaReparos)
        {
            try
            {
                ResultSendParametrosRepair r = new ResultSendParametrosRepair();
                r.Param = parametrosResult;
                r.ListaReparos = ListaReparos;
            }

            catch (Exception e)
            {
                Debug.Write(e);
            }

            return parametros;
        }

        public ParametrosResult SendParametersListaReparosAsync(ParametrosResult parametrosResult, List<ResultRepair> ListaReparos)
        {
            return parametrosResult;
        }

        public List<ResultRepair> GetListRepair(ParametrosResult parametrosResult, List<ResultRepair> resultRepair)
        {
            return ParametersServiceMock.GetRepairs().ListaReparos;
        }

        public class RequestJson
        {
            [JsonProperty("Parametros")]
            public Parametros Param { get; set; }
        }

        public class RequestParameters
        {
            public string UsuarioTotvs { get; set; }
        }

        public class ResultJson
        {
            [JsonProperty("Parametros")]
            public ParametrosResult param { get; set; }
        }


        public class ResultJsonV2
        {
            [JsonProperty("Conteudo")]
            public ResultJson Conteudo { get; set; }
            public string Retorno { get; set; }
        }

        public class ParametrosResult
        {
            public int? CodEmitente { get; set; }
            public string UsuarioTotvs { get; set; }
            public int? DiasXML { get; set; }
            public string CodEstabel { get; set; }
            public string NFRet { get; set; }
            public string DtEntrada { get; set; }
            public decimal? ValorTotal { get; set; }
            public string Serie { get; set; }
            public decimal? QtdeItem { get; set; }
        }
        
        public class ResultSend
        {
            public string Mensagem { get; set; }
            public List<ResultRepair> ListaReparos { get; set; }
        }

        public class ResultSendV2
        {
            public ResultSend Conteudo { get; set; }
            public string Retorno { get; set; }
        }

        public class ResultSendParametrosRepair
        {
            public string Mensagem { get; set; }
            public ParametrosResult Param { get; set; }
            public List<ResultRepair> ListaReparos { get; set; }
        }

        public class ResultRepair
        {
            public int CodEmitente { get; set; }
            public string RowId { get; set; }
            public decimal Qtde { get; set; }
            public string CodEstabel { get; set; }
            public int Digito { get; set; }
            public string Localiza { get; set; }
            public string CodItem { get; set; }
            public string NumRR { get; set; }
            public string Situacao { get; set; }
            public string CodFilial { get; set; }
            public string Mensagem { get; set; }
            public decimal Valor { get; set; }
            public string CodBarras { get; set; }
            public string Origem { get; set; }
            public string DescItem { get; set; }
            public string RetornoMsg { get; set; }
        }
    }
}