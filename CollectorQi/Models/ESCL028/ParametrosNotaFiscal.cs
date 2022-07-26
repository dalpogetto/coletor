using System;

namespace CollectorQi.Models.ESCL028
{
    public class ParametrosNotaFiscal
    {   
        public string RowId { get; set; }
        public string CodEstabel { get; set; }
        public string CodItem { get; set; }
        public string Localizacao { get; set; }
        public string DescricaoItem { get; set; }
        public string NroDocto { get; set; }
        public int NumRR { get; set; }
        public bool Conferido { get; set; }
        public int Relaciona { get; set; }
        public string CodFilial { get; set; }
    }
}