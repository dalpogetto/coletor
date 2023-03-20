namespace CollectorQi.Services.ESCL000
{
    public static class ServiceCommon
    {
        public static string SystemUrl
        {
            get
            {
                //return string.Empty;

                // return "https://brspupapl01.ad.diebold.com:8543";         // Desenv
                // return "https://brspupapl01.ad.diebold.com:8143";         // -> Projetos
                // return "https://brspupapl01.ad.diebold.com:8243";      // -> Homolog
                // return "https://totvsapptst.dieboldnixdorf.com.br:8243; // -> Homolog
                return "https://totvsapp.dieboldnixdorf.com.br:8143";  // -> Produção 
            }
        }

        public static string SystemAliasApp
        {
            get
            {
                return "interfcol";
            }
        }
    }
}
