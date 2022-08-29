using System;

namespace CollectorQi.Models.ESCL028
{
    public class ValidarReparosNotaFiscal
    {   
        public string CodEstabel { get; set; }
        public int Digito { get; set; }
        public int NumRR { get; set; }
        public bool Conferido { get; set; }
        public string CodFilial { get; set; }
        public string Mensagem { get; set; }
        public string CodBarras { get; set; }
    }
}