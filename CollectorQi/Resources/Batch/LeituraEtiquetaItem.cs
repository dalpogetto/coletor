using System;
using System.Collections.Generic;
using System.Text;

namespace CollectorQi.Resources.Batch
{
    // Substitui processo de leitura de etiqueta Progress (integracao/api/
    public static class LeituraEtiqueta
    {
        public static DadosLeituraEtiqueta Item(string pCodBarras)
        {
            string strEstabEmUso;
            string strDepOrigem;
            string strLocOrigem;
            string strCodigoEmUso;
            string strNFEemUso;
            string strSerieEmUso;
            string strQuantidade;

            DadosLeituraEtiqueta dadosLeitura = null;
            //splCodigoBarrasItem;
            // 0 = Código da etiqueta
            // 1 = Código do item
            // 2 = Desc Etiqueta
            // 3 = Quantidade etiqueta
            // 4 = avaliando
            // 5 = avaliando

            // Padroniza tipo de código de barras dependendo do Leitor
            pCodBarras = pCodBarras.Replace("‡", "[");
            pCodBarras = pCodBarras.Replace(";", "[");

            if (!String.IsNullOrEmpty(pCodBarras))
            {
                var splCodigoBarras = pCodBarras.Split('[');

                // Digitou código do item
                if (splCodigoBarras == null)
                {
                    strCodigoEmUso = pCodBarras;
                }
                else if (splCodigoBarras.Length ==  1)
                {
                    dadosLeitura = new DadosLeituraEtiqueta();
                    dadosLeitura.Codigo = splCodigoBarras[0];
                    
                }
                else
                {
                    string strTipoEtiqueta = splCodigoBarras[0];

                    switch (strTipoEtiqueta)
                    {
                        case "01":
                        case "02":
                        case "03":
                        case "07":
                        case "08":
                            dadosLeitura = TratarLeitura(pCodBarras, "Item", splCodigoBarras);

                            if (!String.IsNullOrEmpty(dadosLeitura.Erro))
                            {
                                throw new Exception(dadosLeitura.Erro);
                            }
                            else {
                                strCodigoEmUso = dadosLeitura.Codigo;

                                if (strTipoEtiqueta == "07" || strTipoEtiqueta == "08")
                                {
                                    strNFEemUso = dadosLeitura.NotaFiscal;
                                    strSerieEmUso = dadosLeitura.NumeroSerie;
                                }
                                else
                                {
                                    strNFEemUso = "";
                                    strSerieEmUso = "";
                                }

                                strQuantidade = dadosLeitura.QtdeItem;
                                
                            }

                            break;

                        default:
                            throw new Exception("Layout de etiqueta nao encontrado");

                    }
                }

                if (dadosLeitura != null)
                {
                    dadosLeitura.CodItem = dadosLeitura.Codigo;
                }
            }
            
            return dadosLeitura;
        }
        private static DadosLeituraEtiqueta TratarLeitura(string pCodBarras, string pTipoEtiqueta, string[] pSplCodigoBarras)
        {
            var DadosLeituraEtiqueta = new DadosLeituraEtiqueta();

            DadosLeituraEtiqueta.TpReg = pTipoEtiqueta;
            DadosLeituraEtiqueta.TpEtiqueta = pSplCodigoBarras[0];

            switch(DadosLeituraEtiqueta.TpEtiqueta)
            {
                case "01":
                    DadosLeituraEtiqueta.Codigo      = pSplCodigoBarras[1];
                    DadosLeituraEtiqueta.Descricao   = pSplCodigoBarras[2];
                    DadosLeituraEtiqueta.QtdeItem    = "1";
                    DadosLeituraEtiqueta.NumeroSerie = "NF";
                    break;

                case "02":
                    DadosLeituraEtiqueta.Codigo = pSplCodigoBarras[1];
                    DadosLeituraEtiqueta.Descricao = pSplCodigoBarras[2];
                    DadosLeituraEtiqueta.QtdeItem = pSplCodigoBarras[5];
                    DadosLeituraEtiqueta.NumeroSerie = "NF";
                    break;



                case "03":
                    DadosLeituraEtiqueta.Codigo = pSplCodigoBarras[1];
                    DadosLeituraEtiqueta.Descricao = pSplCodigoBarras[2];
                    DadosLeituraEtiqueta.QtdeItem = pSplCodigoBarras[3];
                    DadosLeituraEtiqueta.DataEmissao = pSplCodigoBarras[4];
                    DadosLeituraEtiqueta.NumeroSerie = "NF";
                    break;

                case "07":
                case "08":
                    DadosLeituraEtiqueta.Codigo = pSplCodigoBarras[1];
                    DadosLeituraEtiqueta.Descricao = pSplCodigoBarras[2];
                    DadosLeituraEtiqueta.NotaFiscal = pSplCodigoBarras[4];
                    DadosLeituraEtiqueta.NumeroSerie = "NF";

                    string strVolume = pSplCodigoBarras[7];

                    if (strVolume.IndexOf("/") > 0){

                        var splQtde = strVolume.Split('/');

                        DadosLeituraEtiqueta.Volume = splQtde[0].Trim();
                        DadosLeituraEtiqueta.QtdeItem = splQtde[1].Trim();

                        /*
                        DadosLeituraEtiqueta.Volume   = strVolume.Substring(0, strVolume.IndexOf("/") - 1).Trim();
                        DadosLeituraEtiqueta.QtdeItem = strVolume.Substring(strVolume.IndexOf("/")).Trim();
                        */

                        /*
                        ASSIGN ttDadosDaLeitura.volume = TRIM(ENTRY(1, v_volqtde, "/"))
                                 ttDadosDaLeitura.qtdaItem = TRIM(ENTRY(2, v_volqtde, "/")).
                        */
                    }
                    else
                    {
                        DadosLeituraEtiqueta.Volume = strVolume.Substring(0,6).Trim();  
                        DadosLeituraEtiqueta.QtdeItem = strVolume.Substring(6,6).Trim();

                        /*
                         * ASSIGN ttDadosDaLeitura.volume   = TRIM(SUBSTRING(v_volqtde,1,6))
                                 ttDadosDaLeitura.qtdaItem = TRIM(SUBSTRING(v_volqtde,7,6)).
                        */
                    }
                    break;
                case "10":
                    DadosLeituraEtiqueta.Endereco = pSplCodigoBarras[1];
                    break;

                default:
                    DadosLeituraEtiqueta.Erro = "Tipo de Layout de etiqueta nao encontrado";
                    break;
            }


            return DadosLeituraEtiqueta;
        }
    }


    public class EtiquetaItem
    {
    }

    public class DadosLeituraEtiqueta
    {
        public string TpReg { get; set; }
        public string TpEtiqueta { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public string QtdeItem { get; set; }
        public string NumeroSerie { get; set; }
        public string Volume { get; set; }
        public string Endereco { get; set; }
        public string Erro { get; set; }
        public string DataEmissao { get; set; }
        public string NotaFiscal { get; set; }

        // Etiqueta Item
        public string CodItem { get; set; }
        public string DescItem { get; set; }
    }
}
