namespace CollectorQi.Models.ESCL017
{
    public class LeituraEtiquetaInventarioReparo
    {
        public string CodProduto { get; set; } = "";
        public string CodEstabel { get; set; } = "";
        public string Localiza { get; set; } = "";
        public decimal Digito { get; set; } = 0;
        public decimal NumRR { get; set; } = 0;
        public string Situacao { get; set; } = "";
        public string DescProduto { get; set; } = "";
        public string CodFilial { get; set; } = "";
        public string Mensagem { get; set; } = "";
        public string CodBarras { get; set; } = "";
    }
}