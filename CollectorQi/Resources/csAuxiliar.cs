using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using CollectorQi.Resources.DataBaseHelper;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace CollectorQi
{
    public enum eStatusInventarioItem
    {
        [Description("Não Iniciado")]
        NaoIniciado = 1,
        [Description("Erro Integração")]
        ErroIntegracao = 2,
    }
    public enum eStatusConnection
    {
        [Description("Online")]
        Online = 1,
        [Description("Offline")]
        Offline = 2,
        [Description("Unauthorized")]
        Unauthorized = 3
    }

    public enum eStatusInventario
    {
        [Description("Não Iniciado")]
        NaoIniciado = 1,
        [Description("Iniciado")]
        IniciadoMobile = 2,
        [Description("Encerrado")]
        EncerradoMobile = 3
    }

    public enum eStatusNotaFiscal
    {
        [Description("Não Iniciado")]
        NaoIniciado = 1,
        [Description("Iniciado")]
        IniciadoMobile = 2,
        [Description("Encerrado")]
        EncerradoMobile = 3
    }

    public enum eStatusIntegracao
    { 
        [Description("Pendente")]
        PendenteIntegracao = 1,
        [Description("Enviado")]
        EnviadoIntegracao  = 2,
        [Description("Erro")]
        ErroIntegracao     = 3
    }


    public enum ePermSaldoNeg
    {
        [Description("Pendente Integração")]
        Nao = 1,
        [Description("Enviado Integração")]
        SimConfirmado = 2,
        [Description("Erro Integração")]
        Sim = 3
    }

    public enum eTpNotificacao
    {
        Inventario,
        Transferencia
    }

    public static class csAuxiliar
    {
        public static string idNotify           { get; set; }

        public static string GetDescription(this Enum value)
        {
            return
                value
                    .GetType()
                    .GetMember(value.ToString())
                    .FirstOrDefault()
                    ?.GetCustomAttribute<DescriptionAttribute>()
                    ?.Description;
        }

        public static ModelEtiqueta GetEtiqueta(string pStrQr)
        {

            string[] delimiter = { ";" };
            var arrayQr = pStrQr.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

            ModelEtiqueta mdlEtiqueta = new ModelEtiqueta();

            if (arrayQr.Count() > 0)
            {

                for (int i = 0; i < arrayQr.Count(); i++)
                {
                    if (i == 0)
                    {

                    }
                    else if (i == 1)
                    {
                        mdlEtiqueta.itCodigo = arrayQr[i];
                    }
                    else if (i == 2)
                    {
                        mdlEtiqueta.descItem = arrayQr[i];
                    }
                    else if (i == 3)
                    {
                        mdlEtiqueta.itOrigem = arrayQr[i];
                    }
                    else if (i == 4)
                    {
                        mdlEtiqueta.itNF = arrayQr[i];
                    }
                    else if (i == 5)
                    {
                        mdlEtiqueta.itDataRecebimento = arrayQr[i];
                    }
                    else if (i == 6)
                    {
                        mdlEtiqueta.itRIQ = arrayQr[i];
                    }
                    else if (i == 7)
                    {
                        mdlEtiqueta.itVol = arrayQr[i];
                    }
                    else if (i == 8)
                    {
                        mdlEtiqueta.itExtra = arrayQr[i];
                    }

                    ///* itCodigo */
                    //if (arrayQr[i].Contains("\"ItCodigo\"".Replace(@"\", "")))
                    //{
                    //    string[] delimiterValue = { @"/@\" };

                    //    var arrayValue = arrayQr[i].Split(delimiterValue, StringSplitOptions.RemoveEmptyEntries);

                    //    if (arrayValue.Count() > 0)
                    //        mdlEtiqueta.itCodigo = arrayValue[1].TrimStart('"').TrimEnd('"').Trim();
                    //}

                    ///* descItem */
                    //if (arrayQr[i].Contains("\"DescItem\"".Replace(@"\", "")))
                    //{
                    //    string[] delimiterValue = { @"/@\" };

                    //    var arrayValue = arrayQr[i].Split(delimiterValue, StringSplitOptions.RemoveEmptyEntries);

                    //    if (arrayValue.Count() > 0)
                    //        mdlEtiqueta.descItem = arrayValue[1].TrimStart('"').TrimEnd('"').Trim();
                    //}

                    ///* Lote */
                    //if (arrayQr[i].Contains("\"Lote\"".Replace(@"\", "")))
                    //{
                    //    string[] delimiterValue = { @"/@\" };

                    //    var arrayValue = arrayQr[i].Split(delimiterValue, StringSplitOptions.RemoveEmptyEntries);

                    //    if (arrayValue.Count() > 0)
                    //        mdlEtiqueta.lote = arrayValue[1].TrimStart('"').TrimEnd('"').Trim();
                    //}

                    ///* DtValiLote */
                    //if (arrayQr[i].Contains("\"DtValiLote\"".Replace(@"\", "")))
                    //{
                    //    string[] delimiterValue = { @"/@\" };

                    //    var arrayValue = arrayQr[i].Split(delimiterValue, StringSplitOptions.RemoveEmptyEntries);

                    //    if (arrayValue.Count() > 0)
                    //    {
                    //        mdlEtiqueta.dtValiLote = arrayValue[1].TrimStart('"').TrimEnd('"').Trim();
                    //    }
                    //}

                    ///* TipoConEst */
                    //if (arrayQr[i].Contains("\"TipoConEst\"".Replace(@"\", "")))
                    //{
                    //    string[] delimiterValue = { @"/@\" };

                    //    var arrayValue = arrayQr[i].Split(delimiterValue, StringSplitOptions.RemoveEmptyEntries);

                    //    if (arrayValue.Count() > 0)
                    //        mdlEtiqueta.tipoConEst = arrayValue[1].TrimStart('"').TrimEnd('"').Trim();
                    //}

                    ///* Un */
                    //if (arrayQr[i].Contains("\"Un\"".Replace(@"\", "")))
                    //{
                    //    string[] delimiterValue = { @"/@\" };

                    //    var arrayValue = arrayQr[i].Split(delimiterValue, StringSplitOptions.RemoveEmptyEntries);

                    //    if (arrayValue.Count() > 0)
                    //        mdlEtiqueta.un = arrayValue[1].TrimStart('"').TrimEnd('"').Trim();
                    //}

                }
            }

            if (!String.IsNullOrEmpty(pStrQr) && String.IsNullOrEmpty(mdlEtiqueta.itCodigo))
                mdlEtiqueta.itCodigo = pStrQr;

            var item = ItemDB.GetItem(mdlEtiqueta.itCodigo);

            if (item != null)
            {
                mdlEtiqueta.descItem   = item.DescItem;
                mdlEtiqueta.un         = item.Un;
                mdlEtiqueta.tipoConEst = item.__TipoConEst__;
            }

            return mdlEtiqueta;
        
        } 
    }


    public class ModelEtiqueta
    {
        public string itCodigo { get; set; } = string.Empty;
        public string descItem { get; set; } = string.Empty;
        public string itOrigem { get; set; } = string.Empty;
        public string itNF { get; set; } = string.Empty;
        public string itDataRecebimento { get; set; } = string.Empty;
        public string itRIQ { get; set; } = string.Empty;
        public string itVol { get; set; } = string.Empty;
        public string itExtra { get; set; } = string.Empty;



        public string lote { get; set; } = string.Empty;
        public string dtValiLote { get; set; } = string.Empty;
        public string tipoConEst { get; set; } = string.Empty;
        public string un { get; set; } = string.Empty;
        public string codRefer { get; set; } = string.Empty;

    }

}
