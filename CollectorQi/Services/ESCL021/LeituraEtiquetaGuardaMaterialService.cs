using CollectorQi.Models.ESCL021;
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
using CollectorQi.Resources.DataBaseHelper;

namespace CollectorQi.Services.ESCL021
{
    public static class LeituraEtiquetaGuardaMaterialService
    {

        // Criar URI como parametrival no ambiente e nao utilizar a variavel
        private static string URI = ServiceCommon.SystemUrl;
        //private const string URI = "https://6303e29c761a3bce77e090d4.mockapi.io";        

        private const string URI_SEND_PARAMETERS = "/api/integracao/coletores/v1/escl027api/LeituraEtiqueta";

        private static string ObterCodigoProduto(string codigoBarras)
        {
            if (codigoBarras != null)
            {
                var conteudo = codigoBarras.Split(';');
                if (conteudo.Length == 0) return string.Empty;

                return conteudo[1];
            }
            return string.Empty;
        }

        private static string ObterNumeroSerie(string codigoBarras)
        {
            if (codigoBarras != null)
            {
                var conteudo = codigoBarras.Split(';');
                if (conteudo.Length == 0) return string.Empty;

                switch (conteudo[0])
                {
                    case "01": return conteudo[4];
                    case "02": return conteudo[8];
                    case "03": return conteudo[5];
                    case "04": return conteudo[6];
                    case "07": return conteudo[8];
                    case "08": return conteudo[8];
                }
            }

            return string.Empty;
        }

        // Metodo ObterParametros Totvs
        public static async Task<ResultSendGuardaMaterialReturnJson> SendLeituraEtiquetaAsync(DadosLeituraItemGuardaMaterial dadosLeituraItemGuardaMaterial)
        {
            ResultSendGuardaMaterialReturnJson parametros = null;
            var codProduto = ObterCodigoProduto(dadosLeituraItemGuardaMaterial.CodigoBarras);
            var numeroSerie = ObterNumeroSerie(dadosLeituraItemGuardaMaterial.CodigoBarras);

            //11-05-2023: Valter: Não permitir realizar duas transferencias para o mesmo Item x Serie
            if (! await LeituraEtiquetaDB.PrimeiraLeituraItemSerie(codProduto, numeroSerie))
            {
                throw new Exception($"Item: {codProduto} e Número de Série: {numeroSerie} já foram lidos nesta sessão !");
            }

            try
            {
                //ESCL.ParametrosNotaFiscal requestParam = new ESCL.ParametrosNotaFiscal() { CodEstabel = "126" };
                RequestDadosLeituraItemJson requestJson = new RequestDadosLeituraItemJson() { Param = dadosLeituraItemGuardaMaterial };
                var byteArray = new UTF8Encoding().GetBytes($"{SecurityAuxiliar.GetUsuarioNetwork()}:{SecurityAuxiliar.CodSenha}");
                var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler());
                client.BaseAddress = new Uri(URI);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                client.DefaultRequestHeaders.Add("CompanyId", SecurityAuxiliar.GetCodEmpresa());
                client.DefaultRequestHeaders.Add("x-totvs-server-alias", ServiceCommon.SystemAliasApp);

                var json = JsonConvert.SerializeObject(requestJson);

                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, URI_SEND_PARAMETERS)
                    {
                        Content = content
                    };

                    var result = await client.SendAsync(req);

                    if (result.IsSuccessStatusCode)
                    {
                        string responseData = await result.Content.ReadAsStringAsync();

                        if (responseData.Contains("Error"))
                        {
                            parametros = JsonConvert.DeserializeObject<ResultSendGuardaMaterialReturnJson>(responseData);
                        }
                        else
                        {
                            var parametroSuccess = JsonConvert.DeserializeObject<ResultGuardaMaterialJson>(responseData);

                            parametros = new ResultSendGuardaMaterialReturnJson()
                            {
                                Retorno = parametroSuccess.Retorno
                            };

                            //Gravar Item + Serie 
                            await LeituraEtiquetaDB.GravarLeituraItemSerie(codProduto, numeroSerie);
                        }
                    }
                    else
                    {
                        throw new Exception("Erro Conexão");
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return parametros;
        }

        public class RequestDadosLeituraItemJson
        {
            [JsonProperty("DadosLeitura")]
            public DadosLeituraItemGuardaMaterial Param { get; set; }
        }

        public class ResultSendGuardaMaterialReturnJson
        {
            [JsonProperty("Conteudo")]

            public List<ResultSendGuardaMaterialErrorJson> Resultparam { get; set; }

            public string Retorno { get; set; }
        }

        public class ResultSendGuardaMaterialErrorJson
        {
            public string ErrorDescription { get; set; }
            public string ErrorHelp { get; set; }
        }

        public class ResultGuardaMaterialJson
        {
            [JsonProperty("Conteudo")]
            public ResultDepositosItemJson Param { get; set; }
            public string Retorno { get; set; }
        }

        public class ResultDepositosItemJson
        {
            [JsonProperty("ListaItens")]
            public List<DepositosGuardaMaterialItem> ParamResult { get; set; }
        }
    }
}