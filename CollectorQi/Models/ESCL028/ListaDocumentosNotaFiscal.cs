namespace CollectorQi.Models.ESCL028
{
    public class ListaDocumentosNotaFiscal
    {   
        public int CodEmitente { get; set; }
        public string Docto { get; set; }
        public string CodEstabel { get; set; }
        public string Usuario { get; set; }
        public bool Atualizar { get; set; }
        public string Marca { get; set; }
        public string NatOperacao { get; set; }
        public string SerieDocto { get; set; }
        public int Relaciona { get; set; }
        public bool Bloqueado { get; set; }
        public int NrProcesso { get; set; } 
        public bool ItensRestantes { get; set; }
    }
}