using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using CollectorQi.Resources.DataBaseHelper;
using System.Linq;

namespace CollectorQi.VO.Batch
{
    public class BatchDepositoTransfereVO
    {
        private int     intTransferenciaId = 0;
        private string  codEstabel = string.Empty;
        private string  itCodigo = string.Empty;
        private string nroDocto = string.Empty;
        private string  codDeposSaida = string.Empty;
        private string  codLocaliz = string.Empty;
        private string  codRefer = string.Empty;
        private string  codLote = string.Empty;
        private DateTime? dtValiLote;
        private string  codDeposEntrada = string.Empty;
        private decimal qtidadeTransf;
        private string  codUsuario = string.Empty;

        /* Integração */
        private DateTime dtTransferencia;
        private DateTime dtIntegracao;
        private string msgIntegracao;
        private eStatusIntegracao statusIntegracao = eStatusIntegracao.PendenteIntegracao;

        public EstabelecVO estabelec;
        public DepositoVO depositoSaida;
        public DepositoVO depositoEntrada;
        public ItemVO item;

        [AutoIncrement, PrimaryKey]
        public int IntTransferenciaId { get => intTransferenciaId; set => intTransferenciaId = value; }



        public string NroDocto { get => nroDocto; set => nroDocto = value; }

        [Indexed(Name = "idxTransferencia", Order = 1, Unique = false)]
        public string   CodEstabel      { get => codEstabel;      set => codEstabel      = value; }
        [Indexed(Name = "idxTransferencia", Order = 2, Unique = false)]
        public string   ItCodigo        { get => itCodigo;        set => itCodigo        = value; }
        [Indexed(Name = "idxTransferencia", Order = 3, Unique = false)]
        public string   CodDeposSaida   { get => codDeposSaida;   set => codDeposSaida   = value; }
        [Indexed(Name = "idxTransferencia", Order = 4, Unique = false)]
        public string   CodLocaliz      { get => codLocaliz;      set => codLocaliz      = value; }
        [Indexed(Name = "idxTransferencia", Order = 5, Unique = false)]
        public string   CodRefer        { get => codRefer;        set => codRefer        = value; }
        [Indexed(Name = "idxTransferencia", Order = 6, Unique = false)]
        public string   CodLote         { get => codLote;         set => codLote         = value; }

        public DateTime? DtValiLote      { get => dtValiLote;      set => dtValiLote      = value; }

        [Indexed(Name = "idxTransferencia", Order = 7, Unique = false)]
        public string   CodDeposEntrada { get => codDeposEntrada; set => codDeposEntrada = value; }
        public decimal  QtidadeTransf   { get => qtidadeTransf;   set => qtidadeTransf   = value; }
        public string   CodUsuario      { get => codUsuario;      set => codUsuario      = value; }

        /* Integração */
        public DateTime DtTransferencia  { get => dtTransferencia; set => dtTransferencia = value; }
        public DateTime DtIntegracao { get => dtIntegracao; set => dtIntegracao = value; }
        public string MsgIntegracao { get => msgIntegracao; set => msgIntegracao = value; }
        public eStatusIntegracao StatusIntegracao { get => statusIntegracao; set => statusIntegracao = value; }

        [Ignore]
        public  EstabelecVO __estabelec__
        {
            get
            {
                if (estabelec == null)
                {
                    estabelec = EstabelecDB.GetEstabelec(CodEstabel);
                    //estabelec = await EstabelecDB.GetEstabelec(CodEstabel);
                }
                return estabelec;
            }
        } 
        

        [Ignore]
        public DepositoVO __depositoSaida__
        {
            get
            {
                if (depositoSaida == null)
                {
                    depositoSaida = DepositoDB.GetDeposito(CodDeposSaida);
                }
                return depositoSaida;
            }
        }

        public DepositoVO __depositoEntrada__
        {
            get
            {
                if (depositoEntrada == null)
                {
                    depositoEntrada = DepositoDB.GetDeposito(CodDeposEntrada);
                }
                return depositoEntrada;
            }
        }

        public ItemVO __item__
        {
            get
            {
                if (item == null)
                {
                    item = ItemDB.GetItem(ItCodigo);
                }
                return item;
            }
        }

        /*

        public BatchDepositoTransfereVO GetBase
        {
            get
            {
                return (BatchDepositoTransfereVO)this;
            }
        }
        */
    }
}
