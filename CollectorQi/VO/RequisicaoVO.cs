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
    public class RequisicaoVO   
    {
        private int       nrRequisicao  = 0;
        private string    nomeAbrev = String.Empty;
        private DateTime? dtRequisicao = null;
        private int       situacao = 0;
        private string codEstabel = string.Empty;
        private bool permAtenderPadrao = false;
        private bool permAtenderDevol = false;

        [Unique, PrimaryKey]
        public virtual int  NrRequisicao { get => nrRequisicao; set => nrRequisicao = value; }

        public string    NomeAbrev       { get => nomeAbrev;    set => nomeAbrev = value;    }
        public DateTime? DtRequisicao    { get => dtRequisicao; set => dtRequisicao = value; }
        public int       Situacao        { get => situacao;     set => situacao = value;     }
        public string    CodEstabel      { get => codEstabel; set => codEstabel = value;     }

        public bool PermAtenderPadrao { get => permAtenderPadrao; set => permAtenderPadrao = value; }
        public bool PermAtenderDevol { get => permAtenderDevol; set => permAtenderDevol = value; }

    }
}
