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

namespace CollectorQi.Services.ESCL000
{
    public static class CadastrosFiliais
    {
        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private const string URI = "https://brspupapl01.ad.diebold.com:8543";
        private const string URI_CADASTRO_FILIAIS = "/api/integracao/coletores/v1/escl000api/ObterListaFiliais";

        // Metodo Finalizar Conferencia - Totvs
        public static async Task<List<Models.ESCL000.Filial>> ObterListaFiliais(string user, string password)
        {
            var filiais = new List<Models.ESCL000.Filial>();

            try
            {
                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());

                client.BaseAddress = new Uri(URI) ;

                // Substituir por user e password
                var byteArray = new UTF8Encoding().GetBytes($"{user}:{password}");

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, URI_CADASTRO_FILIAIS);

                var result = await client.SendAsync(req);

                if (result.IsSuccessStatusCode)
                {
                    string responseData = await result.Content.ReadAsStringAsync();

                    var resultResponse = JsonConvert.DeserializeObject<ResultFiliais>(responseData);

                    filiais = resultResponse.items;
                }
                else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new Exception("Senha Invalida");
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return filiais;
        }

        public class ResultFiliais
        {
            [JsonProperty("items")]
            public List<Models.ESCL000.Filial> items { get; set; }
        }
    }
}