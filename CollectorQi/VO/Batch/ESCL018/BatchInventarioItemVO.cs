using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using CollectorQi.Resources.DataBaseHelper;
using System.Linq;
using System.Threading.Tasks;
using CollectorQi.VO.ESCL018;

namespace CollectorQi.VO.Batch.ESCL018
{
    public class BatchInventarioItemVO : InventarioItemVO
    {
        /* Integração */
        private DateTime dtEfetivacao;
        private DateTime dtIntegracao;
        private string msgIntegracao;
        private eStatusIntegracao statusIntegracao = eStatusIntegracao.PendenteIntegracao;


        //private EstabelecVO estabelec;

        /*
        [PrimaryKey]
        public override int InventarioItemId { get; set; }
        */

        /* Integração */
        public DateTime DtEfetivacao { get => dtEfetivacao; set => dtEfetivacao = value; }
        public DateTime DtIntegracao { get => dtIntegracao; set => dtIntegracao = value; }
        public string MsgIntegracao { get => msgIntegracao; set => msgIntegracao = value; }
        public eStatusIntegracao StatusIntegracao { get => statusIntegracao; set => statusIntegracao = value; }

        /*
        [Ignore]
        public EstabelecVO __estabelec__
        {
            get
            {
                if (estabelec == null)
                {
                    estabelec = EstabelecDB.GetEstabelec(CodEstabel);
                }
                return estabelec;
            }
        }*/

    }
}
