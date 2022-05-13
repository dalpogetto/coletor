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
    public class RequisicaoItemVO   
    {
        //private int       requisicaoItemId = 0;
        private int       nrRequisicao  = 0;
        private int       sequencia = 0;
        private string    itCodigo = string.Empty;
        private decimal qtRequisitada = 0;
        private decimal qtAtendida = 0;
        private decimal qtAtendidaMobile = 0;
        private decimal qtaAtender = 0;
        // Victor Alves - 26/06/219 - campos para devolução
        private decimal qtDevolvida = 0;
        private decimal qtaDevolver = 0;
        private decimal qtDevolvidaMobile = 0;
        // Victor Alves - 26/06/2019 - campos para integração offline
        private int tpInteg  = 0;
        private int versaoIntegracao = 0;

        private string requisicaoItemKey = string.Empty;

        private ItemVO item;
        //private decimal? qtDigitadaMobile = null;

        [PrimaryKey]
        public string RequisicaoItemKey { get { return nrRequisicao.ToString() + "|" + sequencia.ToString() + "|" + itCodigo.Trim().ToString(); } set { requisicaoItemKey = value; } }

        [Indexed(Name = "uRequisicaoItem", Order = 1, Unique = true)]
        public int NrRequisicao { get => nrRequisicao; set => nrRequisicao = value; }

        [Indexed(Name = "uRequisicaoItem", Order = 2, Unique = true)]
        public int Sequencia { get => sequencia; set => sequencia = value; }

        [Indexed(Name = "uRequisicaoItem", Order = 3, Unique = true)]
        public string ItCodigo { get => itCodigo; set => itCodigo = value; }

        public decimal QtRequisitada { get => qtRequisitada; set => qtRequisitada = value; }
        public decimal QtAtendida { get => qtAtendida; set => qtAtendida = value; }
        public decimal QtAtendidaMobile { get => qtAtendidaMobile; set => qtAtendidaMobile = value; }
        public decimal QtaAtender { get => qtaAtender; set => qtaAtender = value; }


        // Victor Alves - 26/06/219 - campos para devolução
        public decimal QtDevolvida { get => qtDevolvida; set => qtDevolvida = value; }
        public decimal QtaDevolver { get => qtaDevolver; set => qtaDevolver = value; }
        public decimal QtDevolvidaMobile { get => qtDevolvidaMobile; set => qtDevolvidaMobile = value; }

        // Victor Alves - 26/06/2019 - campos para integração offline
        public int TpInteg  { get => tpInteg; set => tpInteg = value; }
        public int VersaoIntegracao { get => versaoIntegracao; set => versaoIntegracao = value; }

        [Ignore]
        public ItemVO __item__
        {
            get
            {
                if (item == null)
                {
                    item = ItemDB.GetItem(itCodigo);
                }

                if (item == null)
                {
                    item = new ItemVO();
                    item.DescItem = "Item não encontrado";
                }

                return item;
            }
        }
    }
}
