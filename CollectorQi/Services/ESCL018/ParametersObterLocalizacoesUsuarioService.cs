﻿using CollectorQi.Models.ESCL018;
using CollectorQi.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CollectorQi.Services.ESCL018
{
    public static class ParametersObterLocalizacaoUsuarioService
    {

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private const string URI = "https://brspupapl01.ad.diebold.com:8543";
        //private const string URI = "https://62b47363a36f3a973d34604b.mockapi.io";
        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl018api/ObterLocalizacoesUsuario";

        // Metodo ObterParametros Totvs
        public static async Task<ResultInventarioItemJson> GetObterLocalizacoesUsuarioAsync(int pInventarioId)
        {

            ResultInventarioItemJson parametros = null;

            try
            {
                FichasUsuarioSend requestParam = new FichasUsuarioSend() { IdInventario = pInventarioId };

                RequestInventarioItemJson requestJson = new RequestInventarioItemJson() { Param = requestParam };
                
                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());
                //client.BaseAddress = new Uri(URI);

                // Substituir por user e password
                var byteArray = new UTF8Encoding().GetBytes($"{SecurityAuxiliar.GetUsuarioNetwork()}:{SecurityAuxiliar.CodSenha}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

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
                        System.Diagnostics.Debug.Write(result);
                    }
                }                
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e);
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
            [JsonProperty("ListaLocalizacoes")]
            public List<ResultLocalizacao> Resultparam { get; set; }
        }

        public class ResultLocalizacao
        {
            public string Localizacao { get; set; }
        }

        public class FichasUsuarioSend
        {
            public int IdInventario { get; set; }
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