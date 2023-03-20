using CollectorQi.Services.ESCL000;
using System.Collections.Generic;

namespace CollectorQi.Resources
{
    public static class SecurityAuxiliar
    {
        public static string CodUsuario { get; set; }
        public static string CodSenha { get; set; }
        public static bool Autenticado  { get; set; }
        public static string Estabelecimento { get; set; }
        public static List<ListaEmpresas> EmpresaAll { get; set; }
        public static string CodEmpresa { get; set; }
        public static List<VO.ItemVO> ItemAll { get; set; }
        public static string GetCodEstabel()
        {
            if (Estabelecimento.IndexOf('(') > 0)
                return Estabelecimento.Remove(Estabelecimento.IndexOf('(')).Trim();
            else
                return Estabelecimento.Trim();            
        }
        public static string GetDescEstabel()
        {
            if (Estabelecimento != null && Estabelecimento.Length > 0)
            {
                if (Estabelecimento.IndexOf('(') > 0)
                    return Estabelecimento.Remove(0, Estabelecimento.IndexOf('(')).Replace("(", "").Replace(")", "").Trim();
                else
                    return string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }
        public static string GetUsuarioNetwork()
        {
            return CodUsuario /*+ "@" + "DIEBOLD_MASTER"*/ ;
        }
        public static string GetCodEmpresa ()
        {
            if (CodEmpresa != null && CodEmpresa.Length > 0)
            {
                if (CodEmpresa.IndexOf('(') > 0)
                    return CodEmpresa.Remove(CodEmpresa.IndexOf('(')).Trim();
                else
                    return CodEmpresa.Trim();
            }
            else
            {
                return string.Empty;
            }
        }
    }
}