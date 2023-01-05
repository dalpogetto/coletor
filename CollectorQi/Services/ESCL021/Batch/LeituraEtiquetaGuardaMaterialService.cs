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

namespace CollectorQi.Services.ESCL021
{
    public static class LeituraEtiquetaGuardaMaterialServiceBatch
    {
        // Metodo ObterParametros Totvs
        public static async Task<ResultSendGuardaMaterialReturnJson> SendLeituraEtiquetaBatch(DadosLeituraItemGuardaMaterial dadosLeituraItemGuardaMaterial)
        {
            ResultSendGuardaMaterialReturnJson parametros = null;

            try
            {
                //dadosLeituraItemGuardaMaterial.CodigoBarras
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