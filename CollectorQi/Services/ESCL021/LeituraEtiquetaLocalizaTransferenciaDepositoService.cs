﻿using CollectorQi.Models.ESCL027;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using CollectorQi.Services.ESCL000;

namespace CollectorQi.Services.ESCL021
{
    public class LeituraEtiquetaLocalizaTransferenciaDepositoService
    {
        ResultGuardaMaterialJson parametros = null;

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;
        //private const string URI = "https://62fa31c73c4f110faa941620.mockapi.io";

        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl021api/LeituraEtiquetaLocaliza";

        // Metodo ObterParametros Totvs
        public async Task<ResultGuardaMaterialJson> SendLeituraEtiquetaLocalizaAsync(DadosLeituraLocalizaGuardaMaterial dadosLeituraLocalizaGuardaMaterial)
        {
            try
            {
                RequestDadosLeituraLocalizaJson requestJson = new RequestDadosLeituraLocalizaJson() { Param = dadosLeituraLocalizaGuardaMaterial };

                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());
                client.BaseAddress = new Uri(URI);

                // Substituir por user e password
                //var byteArray = new UTF8Encoding().GetBytes("super:prodiebold11");
                //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                client.DefaultRequestHeaders.Add("x-totvs-server-alias", ServiceCommon.SystemAliasApp);

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
                        parametros = JsonConvert.DeserializeObject<ResultGuardaMaterialJson>(responseData);
                    }
                    else
                    {
                        Debug.Write(result);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Write(e);
            }

            return parametros;
        }

        public class RequestDadosLeituraLocalizaJson
        {
            [JsonProperty("DadosLeituraLocaliza")]
            public DadosLeituraLocalizaGuardaMaterial Param { get; set; }
        }

        public class RequestJson
        {
            public DadosLeituraLocalizaGuardaMaterial ParamResult { get; set; }
        }

        public class ResultGuardaMaterialJson
        {
            [JsonProperty("Conteudo")]
            public ResultGuardaJson Result { get; set; }

            public string Retorno { get; set; }
        }

        public class ResultGuardaJson
        {
            public string Local { get; set; }
        }
    }
}