using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CollectorQi.VO
{
    public class LeituraEtiquetaVO
    {
        [AutoIncrement, PrimaryKey]
        public int Id { get; set; }

        [Indexed(Name = "idxItem", Order = 1), MaxLength(20)]
        public string ItCodigo { get; set; }

        [Indexed(Name = "idxSerie", Order = 2), MaxLength(20)]
        public string Serie { get; set; }
    }
}
