namespace CollectorQi.Models.ESCL018
{
    public class Impressao
    {
        public string OpcaoImpressao { get; set; }
        public ImpressaoItem Item { get; set; }
    }
    public class ImpressaoItem
    {
        public string CodEstabel { get; set; }
        public string CodDeposito { get; set; }
        public string CodItem { get; set; }
        public int Quantidade { get; set; }
        public int QtdeEtiqueta { get; set; }

    }
}