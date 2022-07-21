using System;

namespace CollectorQi.Models.ESCL018
{
    public class ImpressaoReparo
    {
        public string CodigoBarras { get; set; }
        public string CodEstabel { get; set; }
        public string CodFilial { get; set; }
        public string CodItem { get; set; }
        public int NumRR { get; set; }
        public int Digito { get; set; }       
    }
}