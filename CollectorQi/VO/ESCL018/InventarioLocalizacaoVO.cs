using SQLite;

namespace CollectorQi.VO.ESCL018
{
    public class InventarioLocalizacaoVO
    {
        private int    inventarioLocalizacaoId = 0;
        private int    inventarioId            = 0;
        private string localizacao             = "";
        private int    totalFichas             = 0;

        [AutoIncrement, PrimaryKey]
        public int InventarioLocalizacaoId { get => inventarioLocalizacaoId; set => inventarioLocalizacaoId = value; }
        [Indexed(Name = "uInventarioLocalizacaoIndex", Order = 1, Unique = true)]
        public int InventarioId { get => inventarioId; set => inventarioId = value; }
        [Indexed(Name = "uInventarioLocalizacaoIndex", Order = 2, Unique = true)]
        public string Localizacao { get => localizacao; set => localizacao = value; }
        public int TotalFichas { get => totalFichas; set => totalFichas = value; }
    
    }
}
