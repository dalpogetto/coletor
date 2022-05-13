using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using CollectorQi.Resources.DataBaseHelper;
using System.Linq;
using System.Threading.Tasks;

namespace CollectorQi.VO.Batch
{
    public class BatchRequisicaoItem : RequisicaoItemSaldoEstoqVO
    {
        /* Integração */
        private DateTime dtEfetivacao;
        private DateTime dtIntegracao;
        private string   msgIntegracao;
        private eStatusIntegracao      statusIntegracao = eStatusIntegracao.PendenteIntegracao ;

        /*[PrimaryKey]
        public override string RequisicaoItemId { get; set; }*/

        /* Integração */
        public DateTime DtEfetivacao { get => dtEfetivacao; set => dtEfetivacao = value; }
        public DateTime DtIntegracao { get => dtIntegracao; set => dtIntegracao = value; }
        public string MsgIntegracao { get => msgIntegracao; set => msgIntegracao = value; }
        public eStatusIntegracao StatusIntegracao { get => statusIntegracao; set => statusIntegracao = value; }

    }
}
