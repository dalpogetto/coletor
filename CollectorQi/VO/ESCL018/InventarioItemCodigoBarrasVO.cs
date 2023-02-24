using CollectorQi.Resources.DataBaseHelper;
using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using CollectorQi.Resources;
using CollectorQi.Resources.DataBaseHelper.ESCL018;
using System.Threading.Tasks;

namespace CollectorQi.VO.ESCL018
{
    public class InventarioItemCodigoBarrasVO
    {
        // Chave Unica
        private int inventarioItemCodigoBarrasId = 0;

        // Itens relacionados
        private string inventarioItemKey = "";
        private InventarioItemVO inventarioItem = null;

        // Valores
        private decimal quantidade = 0;
        private string codigoBarras = "";
        private string codEtiquetaDescript= "";
        private string codEtiqueta = "";

        [AutoIncrement, PrimaryKey]
        public int InventarioItemCodigoBarrasId { get => inventarioItemCodigoBarrasId; set => inventarioItemCodigoBarrasId = value; }
        public string InventarioItemKey { get => inventarioItemKey; set => inventarioItemKey = value; }
        public decimal Quantidade { get => quantidade; set => quantidade = value; }
        public string CodigoBarras { get => codigoBarras; set => codigoBarras = value; }
        public string CodEtiquetaDescript { get => codEtiquetaDescript; set => codEtiquetaDescript = value; }
        public string CodEtiqueta { get => codEtiqueta; set => codEtiqueta = value; }

        [Ignore]
        public InventarioItemVO __inventarioItem__
        {
            get
            {
                try
                {
                    if (inventarioItemKey != null)
                    {
                        if (inventarioItem == null)
                        {
                            inventarioItem = InventarioItemDB.GetInventarioItemByIdItem(inventarioItemKey);
                        }

                        if (inventarioItem == null)
                        {
                            inventarioItem = new InventarioItemVO();
                        }

                        return inventarioItem;
                    }

                    return new InventarioItemVO();
                }
                catch (Exception ex)
                {
                    return new InventarioItemVO();
                }
            }
        }
    }
}
