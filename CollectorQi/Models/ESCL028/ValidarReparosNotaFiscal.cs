using System;

namespace CollectorQi.Models.ESCL028
{
    public class ValidarReparosNotaFiscal
    {
        public string CodEstabel { get; set; } = "";
        public int Digito { get; set; } = 0;
        public int NumRR { get; set; } = 0;
        public bool Conferido { get; set; } = false;
        public string CodFilial { get; set; } = "";
        public string Mensagem { get; set; } = "";
        public string CodBarras { get; set; } = "";
    }
}