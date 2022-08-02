using SQLite;

namespace CollectorQi.VO
{
    public class NotaFiscalVO
    {
        private string  rowId = "";
        private string  codEstabel  = "";
        private string  codItem = "";
        private string  localizacao = "";
        private string  descricaoItem = "";
        private string  nroDocto = "";
        private int     numRR = 0;
        private bool    conferido;
        private int     relaciona;
        private string  codFilial = "";       

        //[AutoIncrement, PrimaryKey]
        [PrimaryKey]
        public string RowId { get => rowId; set => rowId = value; }

        [Indexed(Name = "uNotaFiscalIndex", Order = 1, Unique = true)]        
        public string CodEstabel { get => codEstabel; set => codEstabel = value; }

        [Indexed(Name = "uNotaFiscalIndex", Order = 2, Unique = true)]
        public string CodItem { get => codItem; set => codItem = value; }

        [Indexed(Name = "uNotaFiscalIndex", Order = 3, Unique = true)]
        public string Localizacao { get => localizacao; set => localizacao = value; }

        [Indexed(Name = "uNotaFiscalIndex", Order = 4, Unique = true)]
        public string DescricaoItem { get => descricaoItem; set => descricaoItem = value; }

        [Indexed(Name = "uNotaFiscalIndex", Order = 5, Unique = true)]
        public string NroDocto { get => nroDocto; set => nroDocto = value; }

        [Indexed(Name = "uNotaFiscalIndex", Order = 6, Unique = true)]
        public int NumRR { get => numRR; set => numRR = value; }

        [Indexed(Name = "uNotaFiscalIndex", Order = 7, Unique = true)]
        public bool Conferido { get => conferido; set => conferido = value; }

        [Indexed(Name = "uNotaFiscalIndex", Order = 8, Unique = true)]
        public int Relaciona { get => relaciona; set => relaciona = value; }

        [Indexed(Name = "uNotaFiscalIndex", Order = 9, Unique = true)]
        public string CodFilial { get => codFilial; set => codFilial = value; }  
    }
}
