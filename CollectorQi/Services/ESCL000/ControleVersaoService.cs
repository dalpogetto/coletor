using CollectorQi.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CollectorQi.Services.ESCL000
{
    public interface IControleVersaoService
    {
        Task<ValidaAplicativoResponse> ValidaAplicativoAsync(ValidaAplicativoRequest request);
    }

    public class ControleVersaoService 
    {
        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;
        //private const string URI = "https://630e0869b37c364eb7116139.mockapi.io";

        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl000api/ValidarAPK";

        public async Task<ValidaAplicativoErrorResponse> ValidaAplicativoAsync(ValidaAplicativoRequest request)
        {
            ValidaAplicativoErrorResponse parametros = null;


            try
            {


                ValidaMobileRequest mobile = new ValidaMobileRequest();

                mobile.Mobile = request;

                var client = new HttpClient(DependencyService
                    .Get<IHTTPClientHandlerCreationService>()
                    .GetInsecureHandler());

                //client.BaseAddress = new Uri(URI);
                var byteArray = new UTF8Encoding().GetBytes($"{SecurityAuxiliar.GetUsuarioNetwork()}:{SecurityAuxiliar.CodSenha}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var json = JsonConvert.SerializeObject(mobile);

                client.DefaultRequestHeaders.Add("CompanyId", "1");

                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, URI + URI_SEND_PARAMETERS)
                    {
                        Content = content
                    };

                    var result = await client.SendAsync(req);

                    /*
                    if (result.IsSuccessStatusCode)
                    {
                        string responseData = await result.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<ValidaAplicativoResponse>(responseData);
                    }
                    else
                    {
                        Debug.Write(result);
                        return default;
                    }*/

                    if (result.IsSuccessStatusCode)
                    {
                        string responseData = await result.Content.ReadAsStringAsync();

                        if (responseData.Contains("Error"))
                        {
                            parametros = JsonConvert.DeserializeObject<ValidaAplicativoErrorResponse>(responseData);
                        }
                        else
                        {
                            var parametrosSuccess = JsonConvert.DeserializeObject<ValidaAplicativoResponse>(responseData);

                            parametros = new ValidaAplicativoErrorResponse()
                            {
                                Retorno = parametrosSuccess.Retorno,
                                APKInfo = parametrosSuccess.Conteudo.APKInfo
                            };
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //Debug.Write(e);
                throw e;
            }

            return parametros;
        }
    }

    public class ValidaMobileRequest
    {
        public ValidaAplicativoRequest Mobile { get; set; }
    }

    public class ValidaAplicativoRequest
    {
        public string Modelo { get; set; }
        public string Imei { get; set; }
        public string Versao { get; set; }
        public string VersaoAndroid { get; set; }
    }

    public class ValidaAplicativoResponse
    {
        public ApkInfo Conteudo { get; set; }

        public string Retorno { get; set; }
        public string id { get; set; }
    }

    public class ApkInfo
    {
        public IList<ValidaAplicativoInfo> APKInfo { get; set; } = new List<ValidaAplicativoInfo>();
    }

    public class ValidaAplicativoInfo
    {
        [JsonProperty("loginValidado")]
        public bool LoginValidado { get; set; }
        [JsonProperty("versaoMobile")]
        public string VersaoMobile { get; set; }
        [JsonProperty("linkVersao")]
        public string LinkVersao { get; set; }
        [JsonProperty("mensagem")]
        public string Mensagem { get; set; }
        [JsonProperty("usuarioLogado")]
        public string UsuarioLogado { get; set; }
        [JsonProperty("versao")]
        public string Versao { get; set; }
    }


    public class ValidaAplicativoErrorResponse
    {
        public string Retorno { get; set; }

        public IList<ValidaAplicativoInfo> APKInfo { get; set; } = new List<ValidaAplicativoInfo>();

        public string Id { get; set; }

        [JsonProperty("Conteudo")]
        public List<ResultSendInventarioErrorJson> Resultparam { get; set; }

    }

    public class ResultInventarioSuccessJson
    {
        public string Retorno { get; set; }
    }

    public class ResultSendInventarioErrorJson
    {
        public string ErrorDescription { get; set; }
        public string ErrorHelp { get; set; }
    }

}
