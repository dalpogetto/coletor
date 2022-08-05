using System.Collections.Generic;

namespace CollectorQi.Resources
{
    public static class SecurityAuxiliar
    {
        public static string CodUsuario { get; set; }
        public static string CodSenha { get; set; }

        public static bool Autenticado  { get; set; }
        public static string Estabelecimento { get; set; }

        public static List<VO.ItemVO> ItemAll { get; set; }

        public static string GetCodEstabel()
        {
            //return "101";
            if (Estabelecimento.IndexOf('(') > 0)
                return Estabelecimento.Remove(Estabelecimento.IndexOf('(')).Trim();
            else
                return Estabelecimento.Trim();            
        }

        public static string GetDescEstabel()
        {
            if (Estabelecimento.IndexOf('(') > 0)
                return Estabelecimento.Remove(0, Estabelecimento.IndexOf('(')).Replace("(","").Replace(")","").Trim();
            else
                return string.Empty;
        }
        public static string GetUsuarioNetwork()
        {
            return CodUsuario + "@" + "DIEBOLD_MASTER";
        }
    }
}