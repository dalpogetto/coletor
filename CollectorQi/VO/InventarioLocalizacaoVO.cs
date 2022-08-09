using CollectorQi.Resources.DataBaseHelper;
using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using CollectorQi.Resources;

namespace CollectorQi.VO
{
    public class InventarioLocalizacaoVO
    {
        private int     inventarioLocalizacaoId = 0;
        private int     inventarioId = 0;
        private string  codLocaliz = "";
        private InventarioVO inventario;
        private string totalFichas;


        [AutoIncrement, PrimaryKey]
        public int    InventarioLocalizacaoId { get => inventarioLocalizacaoId; set => inventarioLocalizacaoId = value; }

        [Indexed(Name = "uInventarioLocalizacaoIndex", Order = 1, Unique = true)]
       // [ForeignKey(typeof(InventarioVO))]
        public int    InventarioId { get => inventarioId; set => inventarioId = value; } 

        [Indexed(Name = "uInventarioItemIndex", Order = 2, Unique = true)]
        public string CodLocaliz { get => codLocaliz; set => codLocaliz = value; }

        public string TotalFichas { get => totalFichas; set => totalFichas = value; }


        [Ignore]
        public InventarioVO __inventario__
        {
            get
            {
                if (inventario == null)
                {
                    inventario = InventarioDB.GetInventario(InventarioId).Result;
                }

                if (inventario == null)
                {
                    inventario = new InventarioVO();                    
                }

                return inventario;
            }
        }
    }
}
