using System;

namespace CollectorQi.Models.ESCL029
{
    public class LeituraMovimentoReparo
    {
        public string RowId { get; set; }
        public string CodEstabel { get; set; }
        public int Digito { get; set; }
        public string Localiza { get; set; }
        public string CodItem { get; set; }
        public int NumRR { get; set; }
        public string Situacao { get; set; }
        public string CodFilial { get; set; }
        public string DescItem { get; set; }
        public string Mensagem { get; set; }
        public int Opcao { get; set; }
        public string CodBarras { get; set; }
    }
}