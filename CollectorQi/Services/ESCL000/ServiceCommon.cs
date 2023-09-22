using CsvHelper;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace CollectorQi.Services.ESCL000
{
    public static class ServiceCommon
    {
        private static CultureInfo culturaBR = new CultureInfo("pt-BR");
        private static CultureInfo culturaUSA = new CultureInfo("en-US");

        public static CultureInfo ObterCulturaBrasil
        {
            get { return culturaBR; }
        }

        public static CultureInfo ObterCulturaUSA
        {
            get { return culturaUSA; }
        }

        public static string SystemUrl
        {
            get
            {
                //return string.Empty;
                //return "https://totvsapptst.dieboldnixdorf.com.br:8543"; //Desenv
                // return "https://brspupapl01.ad.diebold.com:8543";         // Desenv
                // return "https://brspupapl01.ad.diebold.com:8143";         // -> Projetos
                //  return "https://brspupapl01.ad.diebold.com:8243";      // -> Homolog
                 return "https://totvsapptst.dieboldnixdorf.com.br:8243"; // -> Homolog
                // return "https://totvsapp.dieboldnixdorf.com.br:8143";  // -> Produção 
            }
        }

        public static string SystemAliasApp
        {
            get
            {
                return "interfcol";
            }
        }

        public static void SetarAmbienteCulturaBrasil()
        {
            CultureInfo.DefaultThreadCurrentCulture = culturaBR;
        }

        public static void SetarAmbienteCulturaUSA()
        {
            CultureInfo.DefaultThreadCurrentCulture = culturaUSA;
        }

    }
}
