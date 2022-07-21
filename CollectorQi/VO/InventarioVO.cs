using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CollectorQi.Resources.DataBaseHelper;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Globalization;
using System.Resources;
using Xamarin.Forms;

namespace CollectorQi.VO
{
    public class InventarioVO
    {
        private int      inventarioId = 0;
        private string   codEstabel   = "";
        private string   descEstabel = "";
        private string   codDepos     = "";
        private string   descDepos = "";
        //private string codigoBarras = "";
        //private DateTime dtInventario = DateTime.Now;
        private int      contagem     = 0;
        private DateTime dtInventario;


        private int origemInventario = 0;
        private eStatusInventario statusInventario = eStatusInventario.NaoIniciado;
        public bool inventarioAtivo = false;

        private DepositoVO deposito = null;

        //[AutoIncrement, PrimaryKey]
        [PrimaryKey]
        public virtual int  InventarioId { get => inventarioId; set => inventarioId = value; }

        [Indexed(Name = "uInventarioIndex", Order = 1, Unique = true)]
        [MaxLength(40)]
        public string CodEstabel { get => codEstabel; set => codEstabel = value; }

        [Indexed(Name = "uInventarioIndex", Order = 2, Unique = true)]
        public string DescEstabel { get => descEstabel; set => descEstabel = value; }

        [Indexed(Name = "uInventarioIndex", Order = 3, Unique = true)]
        [MaxLength(40)]        
        public string CodDepos { get => codDepos; set => codDepos = value; } 

        [Indexed(Name = "uInventarioIndex", Order = 4, Unique = true)]
        public string DescDepos { get => descDepos; set => descDepos = value; }

        [Indexed(Name = "uInventarioIndex", Order = 5, Unique = true)]
        public int Contagem { get => contagem; set => contagem = value; }

        [Indexed(Name = "uInventarioIndex", Order = 6, Unique = true)]
        public DateTime DtInventario { get => dtInventario; set => dtInventario = value; }

        public int               OrigemInventario { get => origemInventario; set => origemInventario = value; }
        public eStatusInventario StatusInventario { get => statusInventario; set => statusInventario = value; }
        public bool              InventarioAtivo  { get => inventarioAtivo;  set => inventarioAtivo  = value; }

        [Ignore]
        public DepositoVO __deposito__
        {
            get
            {
                if (deposito == null)
                {
                    deposito = DepositoDB.GetDeposito(codDepos);
                }

                if (deposito == null)
                {
                    deposito = new DepositoVO();
                }
                return deposito;
            }
        }
    }
}
