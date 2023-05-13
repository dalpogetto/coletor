namespace CollectorQi.Models.ESCL018
{
    public class Inventario
    {
        public int IdInventario { get; set; }  
        public string CodEstabel { get; set; }
        public string CodDepos { get; set; }
        public string Localizacao { get; set; }
        public string Lote { get; set; }
        public decimal QuantidadeDigitada { get; set; }
        public string CodigoBarras { get; set; }
    }
}