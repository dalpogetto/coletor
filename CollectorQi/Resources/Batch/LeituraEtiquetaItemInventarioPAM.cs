using System;
using System.Collections.Generic;
using System.Text;

namespace CollectorQi.Resources.Batch
{
    public class DadosLeituraEtiquetaPAM
    {
        public string CodItem { get; set; }
        public decimal QtdUnitaria { get; set; }
        public string LeitorCodigoBarras { get; set; }
        public string CodEtiqueta24 { get; set; }
        public string CodEtiqueta { get; set; }
    }

    public static class LeituraEtiquetaInventarioPAM
    {
        
        public static DadosLeituraEtiquetaPAM ConvertePAM(string piLeitor, string byCodItem)
        {
            string cItCodigo = "";
            int iAuxItem = 0;
            int iDigito = 0;
            decimal iQtdUnitaria = 0;
            string piCodEtiq24 = "";

            var cCodEtiq24 = ConverteBase36(piLeitor);

            cItCodigo = cCodEtiq24.Substring(0, 2) + "." +
                        cCodEtiq24.Substring(2, 3) + "." +
                        cCodEtiq24.Substring(5, 5) + "-";

            iAuxItem = (int.Parse(cItCodigo.Substring(0, 1)) * 1) +
                (int.Parse(cItCodigo.Substring(1, 1)) * 3) +
                (int.Parse(cItCodigo.Substring(3, 1)) * 9) +
                (int.Parse(cItCodigo.Substring(4, 1)) * 7) +
                (int.Parse(cItCodigo.Substring(5, 1)) * 1) +
                (int.Parse(cItCodigo.Substring(7, 1)) * 3) +
                (int.Parse(cItCodigo.Substring(8, 1)) * 9) +
                (int.Parse(cItCodigo.Substring(9, 1)) * 7) +
                (int.Parse(cItCodigo.Substring(10, 1)) * 1) +
                (int.Parse(cItCodigo.Substring(11, 1)) * 3);

            // assign iDigito = (10 - (iAuxItem MOD 10));
            iDigito = (10 - (iAuxItem % 10));

            if (iDigito == 10) iDigito = 0;

            cItCodigo = cItCodigo + iDigito.ToString();
            iQtdUnitaria = decimal.Parse(cCodEtiq24.Substring(10, 5));

            // Se calcula digito errado, considera o do item;
            if (cItCodigo != byCodItem)
            {
                if (byCodItem.Contains(cItCodigo.Substring(0, 13)))
                {
                    cItCodigo = byCodItem;
                }
            }

            piCodEtiq24 = "02[" + cItCodigo + "[ETQPAM[8[5[" + iQtdUnitaria.ToString() + "[7[8[" + piLeitor;

            DadosLeituraEtiquetaPAM pam = new DadosLeituraEtiquetaPAM
            {
                CodItem = cItCodigo,
                QtdUnitaria = iQtdUnitaria,
                LeitorCodigoBarras = piLeitor,
                CodEtiqueta = piCodEtiq24,
                CodEtiqueta24 = cCodEtiq24
            };

            return pam;
        }

        static string ConverteBase36(string pCodEtiq16)
        {
            string[] cCodEtiq4 = new string[4];

            int[] iCodEtiq = new int[4];

            cCodEtiq4[0] = pCodEtiq16.Substring(0, 4);
            cCodEtiq4[1] = pCodEtiq16.Substring(4, 4);
            cCodEtiq4[2] = pCodEtiq16.Substring(8, 4);
            cCodEtiq4[3] = pCodEtiq16.Substring(12, 4);

            int iFator = 0;

            string lookup = "123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0";

            try
            {
                for (int iHelp = 0; iHelp < 4; iHelp++)
                {
                    int iCont = 3;
                    int iExp = 0;

                    while (true)
                    {

                        // Substitui lookup progress, e soma valor 1 devido lookup progress iniciar com "1";
                        char charString = char.Parse(cCodEtiq4[iHelp].Substring(iCont, 1));
                        iFator = lookup.IndexOf(charString) + 1;

                        if (iFator == 36)
                        {
                            iFator = 0;
                        }

                        iCodEtiq[iHelp] = iCodEtiq[iHelp] + (iFator * IntPow(36, iExp));

                        iExp = iExp + 1;
                        iCont = iCont - 1;

                        if (iCont < 0)
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                iCodEtiq = null;
            }

            if (iCodEtiq != null)
            {
                string cCodEtiq24 = iCodEtiq[0].ToString("000000") +
                    iCodEtiq[1].ToString("000000") +
                    iCodEtiq[2].ToString("000000") +
                    iCodEtiq[3].ToString("000000");

                return cCodEtiq24;

            }
            else
            {
                return "";
            }
        }

        static int IntPow(int x, int pow)
        {
            int ret = 1;
            while (pow != 0)
            {
                if ((pow & 1) == 1)
                    ret *= x;
                x *= x;
                pow >>= 1;
            }
            return ret;
        }
    }
}
