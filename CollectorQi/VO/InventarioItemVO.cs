using CollectorQi.Resources.DataBaseHelper;
using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using CollectorQi.Resources;

namespace CollectorQi.VO
{
    public class InventarioItemVO
    {
        private int     inventarioItemId = 0;
        private int     inventarioId = 0;
        private string  codLocaliz = "";
        private string  codLote    = "";
        private string  codRefer   = "";
        private string  itCodigo   = "";
        private int     nrFicha    = 0;
        private decimal valApurado = 0;
        private DateTime? dtUltEntr;
        private bool qtdDigitada = false;
        private decimal quantidade = 0;
        private ItemVO  item;
        private InventarioVO inventario;

        private string codigoBarras = "";

        private decimal? qtidadeInventarioItem = null;

        private string ItemItCodigoDescItem = null;
               

        [AutoIncrement, PrimaryKey]
        public int    InventarioItemId { get => inventarioItemId; set => inventarioItemId = value; }

        [Indexed(Name = "uInventarioItemIndex", Order = 1, Unique = true)]
       // [ForeignKey(typeof(InventarioVO))]
        public int    InventarioId { get => inventarioId; set => inventarioId = value; } 

        [Indexed(Name = "uInventarioItemIndex", Order = 2, Unique = true)]
        public string CodLocaliz { get => codLocaliz; set => codLocaliz = value; }

        [Indexed(Name = "uInventarioItemIndex", Order = 3, Unique = true)]
        public string CodLote { get => codLote; set => codLote = value; }

        [Indexed(Name = "uInventarioItemIndex", Order = 4, Unique = true)]
        public string CodRefer { get => codRefer; set => codRefer = value; }

        [Indexed(Name = "uInventarioItemIndex", Order = 5, Unique = true)]
        public string ItCodigo { get => itCodigo; set => itCodigo = value; }

        public int     NrFicha { get => nrFicha; set => nrFicha = value; }

        public decimal ValApurado { get => valApurado; set => valApurado = value; }

        public DateTime? DtUltEntr { get => dtUltEntr; set => dtUltEntr = value; }

        public bool QtdDigitada { get => qtdDigitada; set => qtdDigitada = value; }

        public decimal Quantidade { get => quantidade; set => quantidade = value; }

        [Ignore]
        public string CodigoBarras { get => codigoBarras; set => codigoBarras = value; }

        /*
        [Ignore]
        public string __itemDesc__
        {
            get
            {
                try
                {
                    if (ItemItCodigoDescItem == null)
                    {
                        item = SecurityAuxiliar.ItemAll.Find(p => p.ItCodigo == itCodigo);
                        ItemItCodigoDescItem = itCodigo + item.DescItem;
                    }

                    return ItemItCodigoDescItem == null ? itCodigo : ItemItCodigoDescItem;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Write(ex);
                    return String.Empty;                    
                }
            }
            set
            {
                ItemItCodigoDescItem = value;
            }
        }
        */
        /*
        [Ignore]
        public ItemVO __item__
        {
            get
            {
                if (item == null)
                {
                    item = SecurityAuxiliar.ItemAll.Find(p => p.ItCodigo == itCodigo);
                    ItemItCodigoDescItem = itCodigo + item.DescItem;
                }

                if (item == null)
                {
                    item = ItemDB.GetItem(itCodigo);
                    ItemItCodigoDescItem = itCodigo + item.DescItem;
                }
                
                if (item == null)
                {
                    item = new ItemVO();
                    item.DescItem = "Item não encontrado";
                    ItemItCodigoDescItem = itCodigo + item.DescItem;
                }
                
                return item;
            }
        }

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
        } */
    }
}
