namespace CollectorQi.Models.ESCL018
{
    public class InventarioItemBarra
    {
        public int IdInventario { get; set; }
        public string Lote { get; set; }
        public string CodEstabel { get; set; }
        public string Localizacao { get; set; }
        public string CodItem { get; set; }
        public int Contagem { get; set; }
        public string Serie { get; set; }
        public int IVL { get; set; }
        public string CodEmp { get; set; }
        public string CodDepos { get; set; }
        public decimal QuantidadeDigitada { get; set; }
        public string CodigoBarras { get; set; }
    }

    public class InventarioItem
    {
        public int IdInventario { get; set; }
        public string Lote { get; set; }
        public string CodEstabel { get; set; }
        public string Localizacao { get; set; }
        public string CodItem { get; set; }
        public int Contagem { get; set; }
        public string Serie { get; set; }
        public int IVL { get; set; }
        public string CodEmp { get; set; }
        public string CodDepos { get; set; }
        public decimal Quantidade { get; set; }
    }
}