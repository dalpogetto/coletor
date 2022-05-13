using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace CollectorQi.VO
{
    public class DepositoVO
    {
        /*private int depositoId = 0; */
        private string codDepos;
        private string nome;
 
        /*[AutoIncrement, PrimaryKey]
        public int DepositoId { get => depositoId; set =>depositoId = value; }*/

        [PrimaryKey, Indexed(Name = "idxPk", Order = 1, Unique = true)]
        public string CodDepos { get => codDepos; set => codDepos = value; }

        [MaxLength(200)]
        public string Nome { get => nome; set => nome = value; }

    }
}
