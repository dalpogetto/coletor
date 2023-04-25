namespace CollectorQi.Models.ESCL028
{
    public class FinalizarConferenciaNotaFiscal
    {
        public string CodBarras { get; set; }
        public string CodEstabel { get; set; }
        public string CodFilial { get; set; }
        public decimal NumRR { get; set; }
        public decimal Digito { get; set; }
    }
       
    public class FinalizarConferenciaDocumentos
    {
        public int CodEmitente { get; set; }
        public string Docto { get; set; }
        public string CodEstabel { get; set; }
        public string Usuario { get; set; }
        public bool Atualizar { get; set; }
        public string NatOperacao { get; set; }
        public string SerieDocto { get; set; }
        public int Relaciona { get; set; }
    }

    public class FinalizarConferenciaReparosConferidos
    {
        public string RowId { get; set; }
    }
}