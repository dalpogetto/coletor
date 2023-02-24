﻿using CollectorQi.Models.ESCL021;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using CollectorQi.Services.ESCL000;
using CollectorQi.Resources;
using CollectorQi.Resources.Batch;

namespace CollectorQi.Services.ESCL021
{
    public class LeituraEtiquetaTransferenciaDepositoService
    {
        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;
        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl021api/LeituraEtiqueta";

        public async static Task<ResultTransferenciaDepositoJson> SendLeituraEtiquetaAsync(DadosLeituraItemTransferenciaDeposito dadosLeituraItemTransferenciaDeposito)
        {
            try
            {
                ResultTransferenciaDepositoJson leituraEtiqueta = null;
                try
                {
                    leituraEtiqueta = await SendLeituraEtiquetaAOnline(dadosLeituraItemTransferenciaDeposito);
                }
                catch
                {

                }

                if (leituraEtiqueta == null)
                {
                    var dadosLeitura = LeituraEtiqueta.Item(dadosLeituraItemTransferenciaDeposito.CodigoBarras);

                    leituraEtiqueta = new ResultTransferenciaDepositoJson();
                    leituraEtiqueta.Param = new ResultDepositosItemJson();
                    leituraEtiqueta.Retorno = "OK";
                    leituraEtiqueta.Param.ParamResult = new List<DadosLeituraDadosItemTransferenciaDeposito>();
                    leituraEtiqueta.Param.ParamResult.Add(new DadosLeituraDadosItemTransferenciaDeposito
                    {
                        CodItem = dadosLeitura.CodItem,
                        NF  = dadosLeitura.NotaFiscal,
                        Quantidade = dadosLeitura.QtdeItem,
                        Serie = dadosLeitura.NumeroSerie,
                        Saldo = "Atenção! Não foi possivel capturar o saldo do item!"

                    });
                }

                return leituraEtiqueta;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        // Metodo ObterParametros Totvs
        public async static Task<ResultTransferenciaDepositoJson> SendLeituraEtiquetaAOnline(DadosLeituraItemTransferenciaDeposito dadosLeituraItemTransferenciaDeposito)
        {
            ResultTransferenciaDepositoJson parametros = null;

            try
            {
                RequestDadosLeituraItemJson requestJson = new RequestDadosLeituraItemJson() { Param = dadosLeituraItemTransferenciaDeposito };

                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());
                //client.BaseAddress = new Uri(URI);

                var byteArray = new UTF8Encoding().GetBytes($"{SecurityAuxiliar.GetUsuarioNetwork()}:{SecurityAuxiliar.CodSenha}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                client.DefaultRequestHeaders.Add("CompanyId", SecurityAuxiliar.GetCodEmpresa());
                client.DefaultRequestHeaders.Add("x-totvs-server-alias", ServiceCommon.SystemAliasApp);

                client.Timeout = TimeSpan.FromSeconds(4);

                var json = JsonConvert.SerializeObject(requestJson);

                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, URI + URI_SEND_PARAMETERS)
                    {
                        Content = content
                    };

                    var result = await client.SendAsync(req);

                    if (result.IsSuccessStatusCode)
                    {
                        string responseData = await result.Content.ReadAsStringAsync();

                        if (responseData.Contains("Error"))
                        {
                            //    parametros = JsonConvert.DeserializeObject<ResultSendInventarioReturnJson>(responseData);
                        }
                        else
                        { 
                             parametros = JsonConvert.DeserializeObject<ResultTransferenciaDepositoJson>(responseData);
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

        public class RequestDadosLeituraItemJson
        {
            [JsonProperty("DadosLeituraItem")]
            public DadosLeituraItemTransferenciaDeposito Param { get; set; }
        }

        public class ResultTransferenciaDepositoJson
        {
            [JsonProperty("Conteudo")]
            public ResultDepositosItemJson Param { get; set; }
            public string Retorno { get; set; }
        }

        public class ResultDepositosItemJson
        {
            [JsonProperty("DadosItem")]
            public List<DadosLeituraDadosItemTransferenciaDeposito> ParamResult { get; set; }
        }
    }
}