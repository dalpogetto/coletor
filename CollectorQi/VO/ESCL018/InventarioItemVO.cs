using CollectorQi.Resources.DataBaseHelper;
using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using CollectorQi.Resources;
using CollectorQi.Resources.DataBaseHelper.ESCL018;

namespace CollectorQi.VO.ESCL018
{
    public class InventarioItemVO
    {
        private string inventarioItemKey = "";

        // Itens relacionados

        // Chave
        private int invantarioId      = 0;
        private InventarioVO inventario = null;
        private string localizacao    = "";
        private string lote           = "";
        private string codItem        = "";

        // valores
        private int contagem = 0;
        private string serie = "";
        private int ivl = 0;
        private string codEmp = "";
        private decimal quantidade = 0;
        //private decimal quantidadeAcum = 0;
        private string codigoBarras = "";

        private eStatusInventarioItem statusIntegracao = eStatusInventarioItem.NaoIniciado;

        [PrimaryKey]
        public string InventarioItemKey { get => InventarioId.ToString() + "|" +
                                                 Localizacao             + "|" +
                                                 Lote         + "|" + 
                                                 CodItem      + "|"; set => inventarioItemKey = value; }

        [Indexed(Name = "uInventarioItemIndex", Order = 1, Unique = true),]
        public int InventarioId { get => invantarioId; set => invantarioId = value; }

        [Indexed(Name = "uInventarioItemIndex", Order = 4, Unique = true)]
        public string Localizacao { get => localizacao; set => localizacao = value; }

        [Indexed(Name = "uInventarioItemIndex", Order = 5, Unique = true)]
        public string Lote { get => lote; set => lote = value; }

        [Indexed(Name = "uInventarioItemIndex", Order = 6, Unique = true)]
        public string CodItem { get => codItem; set => codItem = value; }

        public int Contagem { get => contagem; set => contagem = value; }
        public string Serie { get => serie; set => serie = value; }
        public int IVL { get => ivl; set => ivl = value; }
        public string CodEmp { get => codEmp; set => codEmp = value; }
        public decimal Quantidade { get => quantidade; set => quantidade = value; }

        [Indexed(Name = "uInventarioBarras", Order = 1)]
        public string CodigoBarras { get => codigoBarras; set => codigoBarras = value; }

        //public decimal QuantidadeAcum { get => quantidadeAcum; set => quantidadeAcum = value; }

        public eStatusInventarioItem StatusIntegracao { get => statusIntegracao; set => statusIntegracao = value; }
        
        
        [Ignore]
        public InventarioVO __inventario__
        {
            get
            {
                try
                {
                    if (InventarioId != null && InventarioId != 0)
                    {
                        if (inventario == null)
                        {
                            inventario = InventarioDB.GetInventario(InventarioId);
                        }

                        if (inventario == null)
                        {
                            inventario = new InventarioVO();
                        }

                        return inventario;
                    }
                    return new InventarioVO();
                }
                catch
                {
                    return new InventarioVO();
                }
            }
        }
        public void SetInventarioItemKey(string byInventarioItemKey)
        {
            inventarioItemKey = byInventarioItemKey;
        }
    }
}


