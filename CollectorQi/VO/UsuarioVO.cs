using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CollectorQi.VO
{
    public class UsuarioVO
    {
        private int usuarioId = 0;
        [AutoIncrement, PrimaryKey]
        public int UsuarioId { get => usuarioId; set => usuarioId = value; }

        [Unique, MaxLength(30)]
        public string CodUsuario { get; set; }

        [MaxLength(30)]
        public string CodSenha { get; set; }

        public int Ativo { get; set; }

        [MaxLength(1)]
        public string Tipo { get; set; }

        [MaxLength(1)]
        public string Administrador { get; set; }

        private int versao = 0;
        public int Versao { get => versao; set => versao = value; }

        public DateTime LogDataUsuario { get; set; }
    }
}
