﻿using CollectorQi.Models.ESCL029;
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

namespace CollectorQi.Services.ESCL029
{
    public class EfetivarArmazenagemService
    {
        ResultParametrosJson parametros = null;

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;
        //private const string URI = "https://63178523ece2736550b57c6c.mockapi.io";

        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl029api/Efetivar";

        // Metodo ObterParametros Totvs
        public async Task<ResultParametrosJson> SendParametersAsync(EfetivaReparo parametrosInventarioReparo)
        {
            try
            {
                //ParametrosNotaFiscal requestParam = new ParametrosNotaFiscal() { CodEstabel = "126" };
                RequestLeituraJson requestJson = new RequestLeituraJson() { Param = parametrosInventarioReparo };

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
                            parametros = JsonConvert.DeserializeObject<ResultParametrosJson>(responseData);
                        }
                        else
                        {
                            var parametroSuccess = JsonConvert.DeserializeObject<ResultSendConteudoSuccessJson>(responseData);

                            parametros = new ResultParametrosJson()
                            {
                                Retorno = parametroSuccess.Retorno
                            };
                        }
                        //string responseData = await result.Content.ReadAsStringAsync();
                        //parametros = JsonConvert.DeserializeObject<ResultParametrosJson>(responseData);
                    }
                    else
                    {
                        throw new Exception(result.StatusCode.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return parametros;
        }

        public class RequestLeituraJson
        {
            [JsonProperty("Reparo")]
            public EfetivaReparo Param { get; set; }
        }

        public class ResultParametrosJson
        {
            [JsonProperty("Conteudo")]
            public List<ResultConteudoJson> ParamConteudo { get; set; }

            public string Retorno { get; set; }
        }

        public class ResultConteudoJson
        {
            //[JsonProperty("ErrorDescription")]
            public string ErrorDescription { get; set; }

            public string ErrorHelp { get; set; }

        }

        public class ResultSendConteudoSuccessJson
        {
            public string Retorno { get; set; }
        }
    }
}

