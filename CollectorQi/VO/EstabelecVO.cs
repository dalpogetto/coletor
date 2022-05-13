using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace CollectorQi.VO
{
    public class EstabelecVO
    {
        public static readonly string TableName = "EstabelecVO";

        private int estabelecId = 0;
        private string codEstabel;
        private string nome;
 
        [AutoIncrement, PrimaryKey]
        public int EstabelecId { get => estabelecId; set => estabelecId = value; }

        [Unique, MaxLength(30)]
        public string CodEstabel { get => codEstabel; set => codEstabel = value; }

        [MaxLength(200)]
        public string Nome { get => nome; set => nome = value; }

    }
}
