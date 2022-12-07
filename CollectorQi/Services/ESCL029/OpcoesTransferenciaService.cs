﻿using CollectorQi.Models.ESCL017;
using CollectorQi.Models.ESCL029;
using CollectorQi.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using CollectorQi.Services.ESCL000;

namespace CollectorQi.Services.ESCL029
{
    public class OpcoesTransferenciaService
    {


        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;
        //private const string URI = "https://63178523ece2736550b57c6c.mockapi.io";

        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl029api/ObterOpcoesTransferencia";

        // Metodo ObterParametros Totvs
        public async Task<ResultOpcoesTransferenciaJson> SendOpcoesTransferenciaAsync()
        {
            ResultOpcoesTransferenciaJson parametros = null;

            try
            {
                //ParametrosNotaFiscal requestParam = new ParametrosNotaFiscal() { CodEstabel = "126" };
                RequestOpcoesTransferenciaJson requestJson = new RequestOpcoesTransferenciaJson(); // { Param = parametrosInventarioReparo };

                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());
               // client.BaseAddress = new Uri(URI);  

                // Substituir por user e password
                //var byteArray = new UTF8Encoding().GetBytes("super:prodiebold11");
                //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var byteArray = new UTF8Encoding().GetBytes($"{SecurityAuxiliar.GetUsuarioNetwork()}:{SecurityAuxiliar.CodSenha}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                client.DefaultRequestHeaders.Add("CompanyId", SecurityAuxiliar.GetCodEmpresa());
                client.DefaultRequestHeaders.Add("x-totvs-server-alias", ServiceCommon.SystemAliasApp);

                var json = JsonConvert.SerializeObject(requestJson);

                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, URI + URI_SEND_PARAMETERS);
                  // {
                  //     Content = content
                  // };

                    var result = await client.SendAsync(req);

                    System.Diagnostics.Debug.Write(result);

                    if (result.IsSuccessStatusCode)
                    {
                        string responseData = await result.Content.ReadAsStringAsync();

                        System.Diagnostics.Debug.Write(responseData);
                        parametros = JsonConvert.DeserializeObject<ResultOpcoesTransferenciaJson>(responseData);
                   //
                   //     if (responseData.Contains("Error"))
                   //     {
                   //         parametros = JsonConvert.DeserializeObject<ResultReparoReturnJson>(responseData);
                   //     }
                   //     else
                   //     {
                   //         var parametroSuccess = JsonConvert.DeserializeObject<ResultImpressaoSuccessJson>(responseData);
                   //
                   //         parametros = new ResultImpressaoReturnJson()
                   //         {
                   //             Retorno = parametroSuccess.Retorno
                   //         };
                   //     }
                   //
                   //     System.Diagnostics.Debug.Write(parametros);
                    }
                    else
                        System.Diagnostics.Debug.Write(result);

                }
            }
            catch (Exception e)
            {
                Debug.Write(e);
            }

            return parametros;
        }

        public class ResultParametrosSuccessJson
        {
            public string Retorno { get; set; }

        }

        public class ResultReparoReturnJson
        {
            [JsonProperty("Conteudo")]

            public List<ParametrosReparoResultError> Resultparam { get; set; }

            public string Retorno { get; set; }
        }

        public class ParametrosReparoResultError
        {
            public string ErrorDescription { get; set; }
            public string ErrorHelp { get; set; }

        }
        public class RequestOpcoesTransferenciaJson
        {
            //[JsonProperty("Parametros")]
            //public ParametrosInventarioReparo Param { get; set; }
        }

        public class ResultOpcoesTransferenciaJson
        {
            [JsonProperty("Conteudo")]
            public ResultConteudoJson ResultConteudo { get; set; }
        }

        public class ResultConteudoJson
        {
            [JsonProperty("Opcoes")]
            public List<OpcoesTransferenciaMovimentoReparo> ResultParam { get; set; }
        }
    }
}