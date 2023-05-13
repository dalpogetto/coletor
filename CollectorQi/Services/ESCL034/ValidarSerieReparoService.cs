﻿using CollectorQi.Models.ESCL018;
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
    public static class ValidarSerieReparoService
    {

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;
        //private const string URI = "https://62b47363a36f3a973d34604b.mockapi.io";
        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl034api/ValidarSerie";

        // Metodo ObterParametros Totvs
        public static async Task<ResultConteudoSuccessJson> ValidarReparo(string pRowId, string pSerie)
        {

            ResultConteudoSuccessJson parametros = null;

            try
            {
                InfoSerieSend requestParam = new InfoSerieSend() {
                    RowId = pRowId,
                    Serie = pSerie
                };

                InforSerieSendJson requestJson = new InforSerieSendJson() { Param = requestParam };

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

                        System.Diagnostics.Debug.Write(parametros);

                        parametros = JsonConvert.DeserializeObject<ResultConteudoSuccessJson>(responseData);
                    }
                    else
                    {
                        //   ErroConnectionERP.ValidateConnection(result.StatusCode);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return parametros;
        }

        public class ResultConteudoSuccessJson
        {
            [JsonProperty("Conteudo")]
            public ResultMensagem Conteudo { get; set; }
            public string Retorno { get; set; }
        }

        public class ResultMensagem
        {
            public string Mensagem { get; set; }
        }

        public class InfoSerieSend
        {
            public string RowId { get; set; }
            public string Serie { get; set; }
        }
        public class InforSerieSendJson
        {
            [JsonProperty("Parametros")]
            public InfoSerieSend Param { get; set; }
        }
    }
}