﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CollectorQi.Resources;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace CollectorQi.Services.ESCL000
{
    public static class CadastrosLocalizacao
    {
        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;
        private const string URI_CADASTRO_FILIAIS = "/api/integracao/coletores/v1/escl000api/ObterListaLocalizacoes";

        // Metodo Finalizar Conferencia - Totvs
        public static async Task<List<Models.ESCL000.Deposito>> ObterListaLocalizacao(string pCodDepos)
        {
            var filiais = new List<Models.ESCL000.Deposito>();

            try
            {
                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());

                var byteArray = new UTF8Encoding().GetBytes($"{SecurityAuxiliar.GetUsuarioNetwork()}:{SecurityAuxiliar.CodSenha}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, URI + URI_CADASTRO_FILIAIS + $"?codEstabel={SecurityAuxiliar.GetCodEstabel()}&codDepos={pCodDepos}");

                var codEmpresa = SecurityAuxiliar.GetCodEmpresa();
                
                client.DefaultRequestHeaders.Add("CompanyId", codEmpresa);
                client.DefaultRequestHeaders.Add("x-totvs-server-alias", ServiceCommon.SystemAliasApp);

                var result = await client.SendAsync(req);

                if (result.IsSuccessStatusCode)
                {
                    string responseData = await result.Content.ReadAsStringAsync();

                    var resultResponse = JsonConvert.DeserializeObject<ResultDepositos>(responseData);

                    filiais = resultResponse.items;
                }
                else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new Exception("Unauthorized");
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return filiais;
        }

        public class ResultDepositos
        {
            [JsonProperty("items")]
            public List<Models.ESCL000.Deposito> items { get; set; }
        }
    }
}