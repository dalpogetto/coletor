using System;
using SQLite;

namespace CollectorQi.VO.ESCL018
{
    public class InventarioVO
    {
        private int idInventario                    = 0;
        private string codEstabel                   = "";
        private string descEstabel                  = "";
        private string codDepos                     = "";
        private string descDepos                    = "";
        private DateTime dtSaldo                    = DateTime.MinValue;
        private eStatusInventario statusInventario  = eStatusInventario.NaoIniciado;

        [PrimaryKey]
        public virtual int IdInventario { get => idInventario; set => idInventario = value; }

        [Indexed(Name = "uInventarioIndex", Order = 1, Unique = true)]
        public string CodEstabel { get => codEstabel; set => codEstabel = value; }
        [Indexed(Name = "uInventarioIndex", Order = 2, Unique = true)]
        public string CodDepos { get => codDepos; set => codDepos = value; }
        [Indexed(Name = "uInventarioIndex", Order = 3, Unique = true)]
        public DateTime DtSaldo { get => dtSaldo; set => dtSaldo = value; }
        public string DescDepos { get => descDepos; set => descDepos = value; }
        public string DescEstabel { get => descEstabel; set => descEstabel = value; }
        public eStatusInventario StatusInventario { get => statusInventario; set => statusInventario = value; }

    }
}
