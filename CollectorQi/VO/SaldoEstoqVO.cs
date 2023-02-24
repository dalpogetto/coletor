using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using CollectorQi.Resources.DataBaseHelper;

namespace CollectorQi.VO
{
    public class SaldoEstoqVO
    {
        private int       saldoEstoqId = 0;
        private string    codEstabel   = String.Empty;
        private string    itCodigo     = String.Empty;
        private string    codDepos     = String.Empty;
        private string    codRefer     = String.Empty;  
        private string    codLocaliz   = String.Empty;
        private string    codLote      = String.Empty;
        private DateTime?  dtValiLote;   
        private decimal   qtidadeAtu;
        private decimal   qtidadeMobile;
        //private DateTime  dtVersaoSaldo;
        private int nrTransVersao;

        private DepositoVO deposito;

        [AutoIncrement, PrimaryKey]
        public int SaldoEstoqId { get => saldoEstoqId; set => saldoEstoqId = value; }

        [Indexed(Name = "idxPk", Order = 1, Unique = true)]
        public string CodEstabel  { get => codEstabel; set => codEstabel  = value; }
        [Indexed(Name = "idxPk", Order = 2, Unique = true)]
        public string ItCodigo    { get => itCodigo;   set => itCodigo    = value; }
        [Indexed(Name = "idxPk", Order = 3, Unique = true)]
        public string CodDepos    { get => codDepos;   set => codDepos    = value; }
        [Indexed(Name = "idxPk", Order = 4, Unique = true)]
        public string CodRefer    { get => codRefer;   set => codRefer    = value; }
        [Indexed(Name = "idxPk", Order = 5, Unique = true)]
        public string CodLocaliz  { get => codLocaliz; set => codLocaliz  = value; }
        [Indexed(Name = "idxPk", Order = 6, Unique = true)]
        public string CodLote     { get => codLote;    set => codLote     = value; }
        
        public DateTime?   DtValiLote  { get => dtValiLote;    set => dtValiLote    = value; }
        public decimal   QtidadeAtu    { get => qtidadeAtu;    set => qtidadeAtu    = value; }
        public decimal   QtidadeMobile { get => qtidadeMobile; set => qtidadeMobile = value; }
        //public DateTime  DtVersaoSaldo { get => dtVersaoSaldo; set => dtVersaoSaldo = value; }

        public int NrTransVersao { get => nrTransVersao; set => nrTransVersao = value; }

        [Ignore]
        public DepositoVO __deposito__
        {
            get
            {
                if (deposito == null)
                {
                    deposito = DepositoDB.GetDeposito(CodDepos);
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
