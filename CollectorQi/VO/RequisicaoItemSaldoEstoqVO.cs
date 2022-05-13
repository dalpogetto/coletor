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
    public class RequisicaoItemSaldoEstoqVO
    {
        private string requisicaoSaldoKey = string.Empty;
        //private int requisicaoItemSaldoEstoqId = 0;
        private int nrRequisicao = 0;
        private int sequencia = 0;
        private string itCodigo = String.Empty;
        private string codEstabel = String.Empty;
        private string codDepos = String.Empty;
        private string codRefer = String.Empty;
        private string codLocaliz = String.Empty;
        private string codLote = String.Empty;
        private bool isDevolucao = false;
        private decimal qtdAtender = 0;
        private decimal qtdDevolver = 0;
        private decimal qtdaDevolver = 0;
        //private bool isDigitadaMobile = false; // Se foi digitada Mobile

        /*[AutoIncrement, PrimaryKey]
        public int RequisicaoItemSaldoEstoqId { get => requisicaoItemSaldoEstoqId; set => requisicaoItemSaldoEstoqId = value; } */
        [PrimaryKey]
        public string RequisicaoSaldoKey
        {
            get
            {
                return nrRequisicao.ToString() + "|" +
                       sequencia.ToString() + "|" +
                       itCodigo.Trim().ToString() + "|" +
                       codEstabel.Trim().ToString() + "|" +
                       codDepos.Trim().ToString() + "|" +
                       codRefer.Trim().ToString() + "|" +
                       codLocaliz.Trim().ToString() + "|" +
                       codLote.Trim().ToString() + "|" +
                       isDevolucao.ToString();

            }

            set { requisicaoSaldoKey = value; }
        }

        [Indexed(Name = "uRequisicaoItemSaldoEstoq", Order = 1, Unique = true)]
        public int NrRequisicao { get => nrRequisicao; set => nrRequisicao = value; }

        [Indexed(Name = "uRequisicaoItemSaldoEstoq", Order = 2, Unique = true)]
        public int Sequencia { get => sequencia; set => sequencia = value; }

        [Indexed(Name = "uRequisicaoItemSaldoEstoq", Order = 3, Unique = true)]
        public string ItCodigo { get => itCodigo; set => itCodigo = value; }

        [Indexed(Name = "uRequisicaoItemSaldoEstoq", Order = 4, Unique = true)]
        public string CodEstabel { get => codEstabel; set => codEstabel = value; }

        [Indexed(Name = "uRequisicaoItemSaldoEstoq", Order = 5, Unique = true)]
        public string CodDepos { get => codDepos; set => codDepos = value; }

        [Indexed(Name = "uRequisicaoItemSaldoEstoq", Order = 6, Unique = true)]
        public string CodRefer { get => codRefer; set => codRefer = value; }

        [Indexed(Name = "uRequisicaoItemSaldoEstoq", Order = 7, Unique = true)]
        public string CodLocaliz { get => codLocaliz; set => codLocaliz = value; }

        [Indexed(Name = "uRequisicaoItemSaldoEstoq", Order = 8, Unique = true)]
        public string CodLote { get => codLote; set => codLote = value; }

        [Indexed(Name = "uRequisicaoItemSaldoEstoq", Order = 9, Unique = true)]
        public bool IsDevolucao { get => isDevolucao; set => isDevolucao = value; }

       /* [Indexed(Name = "uRequisicaoItemSaldoEstoq", Order = 10, Unique = true)]
        public bool IsDigitadaMobile { get => isDigitadaMobile; set => isDigitadaMobile = value; } */

        public decimal QtdAtender  { get => qtdAtender; set => qtdAtender = value; }

        public decimal QtdDevolver { get => qtdDevolver; set => qtdDevolver = value; }
        public decimal QtdaDevolver { get => qtdaDevolver; set => qtdaDevolver = value; }

    }
}
