using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using Xamarin.Forms;

namespace CollectorQi.Resources
{
    public static class SecurityAuxiliar
    {

        public static string CodUsuario { get; set; }
        public static string CodSenha { get; set; }
        public static bool   Autenticado  { get; set; }
        public static string Estabelecimento { get; set; }

        public static List<VO.ItemVO> ItemAll { get; set; }

        public static string GetCodEstabel()
        {
            //return "2100";
            if (Estabelecimento.IndexOf('(') > 0)
                return Estabelecimento.Remove(Estabelecimento.IndexOf('(')).Trim();
            else
                return Estabelecimento.Trim();
        }
    }
}
