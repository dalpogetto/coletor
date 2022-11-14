using System;

namespace CollectorQi.Models.ESCL028
{
    public class ValidarReparosNotaFiscal
    {
        public string CodEstabel { get; set; } = "";
        public decimal Digito { get; set; } = 0;
        public decimal NumRR { get; set; } = 0;
        public bool Conferido { get; set; } = false;
        public string CodFilial { get; set; } = "";
        public string Mensagem { get; set; } = "";
        public string CodBarras { get; set; } = "";
    }
}