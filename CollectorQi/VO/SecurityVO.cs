using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace CollectorQi.VO
{
    public class SecurityVO
    {
        private  int securityId = 0;      
        private  string codUsuario = String.Empty;
        private string codSenha = String.Empty;
        private  bool autenticado = false;
        private  DateTime dtUltIntegracao = DateTime.MinValue;
        private bool cxCompleta = false;

        [AutoIncrement, PrimaryKey]
        public  int SecurityId { get => securityId; set => securityId = value; }

        [MaxLength(40)]
        public  string CodUsuario { get => codUsuario; set => codUsuario = value; }

        [MaxLength(40)]
        public  string CodSenha { get => codSenha; set => codSenha = value; }

        public  bool Autenticado { get => autenticado; set => autenticado = value; }

        public  DateTime DtUltIntegracao { get => dtUltIntegracao; set => dtUltIntegracao = value; }

        public bool CxCompleta { get => cxCompleta; set => cxCompleta = value; }


    }
}
