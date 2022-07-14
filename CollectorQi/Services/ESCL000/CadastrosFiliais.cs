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
    public class Cadastros
    {
        private IEnumerable<Parametros> parametros;

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private const string URI = "https://brspupapl01.ad.diebold.com:8543";
        private const string URI_CADASTRO_FILIAIS = "/api/integracao/coletores/v1/escl000api/ObterListaFiliais";

        // Metodo Finalizar Conferencia - Totvs
        public async Task<string> ObterListaFiliais(string user, string password)
        {
            try
            {

                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());

                client.BaseAddress = new Uri(URI) ;

                // Substituir por user e password
                var byteArray = new UTF8Encoding().GetBytes($"{user}:{password}");

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                //var json = JsonConvert.SerializeObject(requestJson);

                /*
                using (var content = new StringContent(null, Encoding.UTF8, "application/json"))
                { */
                    HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, URI_CADASTRO_FILIAIS)
                    {
                        //Content = content
                    }; 

                    var result = await client.SendAsync(req);

                    if (result.IsSuccessStatusCode)
                    {

                    /*string responseData = await result.Content.ReadAsStringAsync();
                        System.Diagnostics.Debug.Write(result);

                        var resultConvert = JsonConvert.DeserializeObject<EndConferenceResultV2>(responseData);

                        System.Diagnostics.Debug.Write(resultConvert);
                    */
                    return "OK";
                    }
               // }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e);
            }

            return "NOK";
        }

        #region Metodos utilizados para finalizar conferencia
        public class EndConferenceRequest
        {
            [JsonProperty("Parametros")]
            public EndConferenceParameters Param { get; set; }

            [JsonProperty("ListaReparos")]
            public List<Repair> Repairs { get; set; }
        }

        /*
        public class ValidateResultJson
        {
            [JsonProperty("ReparosValidados")]
            public List<RepairResult> RepairResult { get; set; }
        }*/

        public class Repair
        {
            public string RowId { get; set; }
        }

        public class EndConferenceParameters
        {
            public string UsuarioTotvs { get; set; }
            public string CodEstabel { get; set; }
            public int CodEmitente { get; set; }
            public string DtEntrada { get; set; }
            public string NfRet { get; set; }

            public string Serie { get; set; }
            public decimal QtdeItem { get; set; }
            public decimal ValorTotal { get; set; }
            public decimal DiasXml { get; set; }

        }

        public class EndConferenceResultV2
        {
            public EndConferenceResult Conteudo { get; set; }
            public string Retorno { get; set; }
        }
        public class EndConferenceResult
        {
            [JsonProperty("DocumentoGerado")]
            public EndConferenceResultDocto Docto {get;set;}
        }
        
        public class EndConferenceResultDocto
        {
            public int CodEmitente { get; set; }
            public string NroDocto { get; set; }
            public string NatOperacao { get; set; }

            public string SerieDocto { get; set; }
        }
        #endregion
    }
}