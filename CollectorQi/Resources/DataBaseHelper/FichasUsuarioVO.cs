using SQLite;

namespace CollectorQi.VO
{
    public class FichasUsuarioVO
    {
        private string localizacao = "";               

        [Indexed(Name = "uFichasUsuarioIndex", Order = 1, Unique = true)]
        public string Localizacao { get => localizacao; set => localizacao = value; }         
    }
}
