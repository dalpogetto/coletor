using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace CollectorQi.VO
{
    public class ItemVO
    {
        private int itemId = 0;
        private string itCodigo;
        private string descItem = string.Empty;
        private string depositoPad = string.Empty;
        private string codLocaliz = string.Empty;
        private bool locUnica;
        private int tipoConEst = 0;
        private bool contrQualid;
        private bool logOrigExt;
        private bool fraciona;
        private string un = string.Empty;
        private DateTime logDataItem = DateTime.Now;
        private ePermSaldoNeg permSaldoNeg;
        //private int versao = 0;
        private string produtoNaoEncontrado = "N";

        /* Integração */
        private int versaoIntegracao;

        [AutoIncrement, PrimaryKey]
        public int ItemId { get => itemId; set => itemId = value; }

        [Unique, MaxLength(40)]
        public string ItCodigo { get => itCodigo; set => itCodigo = value; }

        [MaxLength(200)]
        public string DescItem { get => descItem; set => descItem = value; }

        [MaxLength(200)]
        public string DepositoPad { get => depositoPad; set => depositoPad = value; }

        [MaxLength(200)]
        public string CodLocaliz { get => codLocaliz; set => codLocaliz = value; }

        public bool LocUnica { get => locUnica; set => locUnica = value; }

        public int TipoConEst { get => tipoConEst; set => tipoConEst = value; }

        public bool ContrQualid { get => contrQualid; set => contrQualid = value; }

        public bool LogOrigExt { get => logOrigExt; set => logOrigExt = value; }

        public bool Fraciona { get => fraciona; set => fraciona = value; }

        [MaxLength(10)]
        public string Un { get => un; set => un = value; }

        public DateTime LogDataItem { get => logDataItem; set => logDataItem = value; }

        public int VersaoIntegracao { get => versaoIntegracao; set => versaoIntegracao = value; }

        [MaxLength(1)]
        public string ProdutoNaoEncontrado { get => produtoNaoEncontrado; set => produtoNaoEncontrado = value; }

        public string __TipoConEst__
        {
            get
            {
                switch (tipoConEst)
                {
                    case 1:
                        return "Serial";
                    case 2:
                        return "Número Série";
                    case 3:
                        return "Lote";
                    case 4:
                        return "Referência";
                    default:
                        return string.Empty;

                }
            }
        }
    }
}
