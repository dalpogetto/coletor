namespace CollectorQi.Models.ESCL029
{
    public class LeituraMovimentoReparo
    {
        public string RowId { get; set; }
        public string CodEstabel { get; set; }
        public decimal Digito { get; set; }
        public string Localiza { get; set; }
        public string CodItem { get; set; }
        public decimal NumRR { get; set; }
        public string Situacao { get; set; }
        public string CodFilial { get; set; }
        public string DescItem { get; set; }
        public string Mensagem { get; set; }
        public int Opcao { get; set; }
        public string CodBarras { get; set; } 
    }

    public class EfetivaReparo
    {
        public int Opcao { get; set; }
        public string RowId { get; set; }

        public int CodTecnico { get; set; }

        public string CodEstabel { get; set; } 
    }
}