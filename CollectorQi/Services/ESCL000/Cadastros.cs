
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using CollectorQi.Models;
using CollectorQi.Resources;
using CollectorQi.Resources.DataBaseHelper;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace CollectorQi.Services.ESCL000
{
    public static class Cadastros
    {
        //private IEnumerable<Parametros> parametros;

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;
        private const string URI_OBTER_EMITENTE = "/api/integracao/coletores/v1/escl000api/ObterEmitente";
        private const string URI_OBTER_ITEM = "/api/integracao/coletores/v1/escl000api/ObterItem";

        // Metodo Finalizar Conferencia - Totvs
        public static async Task<string> ObterEmitente(string user, string password, string codEmitente)
        {
            try
            {

                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());

                client.BaseAddress = new Uri(URI) ;

                // Substituir por user e password
                var byteArray = new UTF8Encoding().GetBytes($"{user}:{password}");

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                //var json = JsonConvert.SerializeObject(requestJson);
                client.DefaultRequestHeaders.Add("x-totvs-server-alias", ServiceCommon.SystemAliasApp);

                /*
                using (var content = new StringContent(null, Encoding.UTF8, "application/json"))
                { */
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, URI_OBTER_EMITENTE + "?CodEmitente=" + codEmitente.ToString())
                    {
                        //Content = content
                    }; 

                    var result = await client.SendAsync(req);

                    if (result.IsSuccessStatusCode)
                    {
                    string responseData = await result.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.Write(result);

                    var resultConvert = JsonConvert.DeserializeObject<ResultJsonEmitente>(responseData);


                    return resultConvert.nome;
                    


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

        public static async Task<string> ObterItem(string pItCodigo)
        {
            try
            {

                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());

                client.BaseAddress = new Uri(URI);

                // Substituir por user e password
                var byteArray = new UTF8Encoding().GetBytes($"{SecurityAuxiliar.GetUsuarioNetwork()}:{SecurityAuxiliar.CodSenha}");

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                //var json = JsonConvert.SerializeObject(requestJson);

                /*
                using (var content = new StringContent(null, Encoding.UTF8, "application/json"))
                { */
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, URI_OBTER_ITEM + "?CodItem=" + pItCodigo.ToString())
                {
                    //Content = content
                };

                var result = await client.SendAsync(req);

                if (result.IsSuccessStatusCode)
                {
                    string responseData = await result.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.Write(result);

                    var resultConvert = JsonConvert.DeserializeObject<ResultJsonItem>(responseData);


                    return resultConvert.DescItem;



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

            return "";
        }

        public class ResultJsonEmitente
        {
            public string codEmitente { get; set; }
            public string nomeAbrev { get; set; }
            public string nome { get; set; }
        }

        public class ResultJsonItem
        {
            public string CodItem { get; set; }
            public string DescItem { get; set; }
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