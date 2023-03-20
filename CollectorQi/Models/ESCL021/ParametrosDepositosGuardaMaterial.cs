using System;

namespace CollectorQi.Models.ESCL021
{
    public class ParametrosDepositosGuardaMaterial
    {
        public string DepositoEntrada { get; set; }
        public string DepositoSaida { get; set; }
        public string LocalizacaoEntrada { get; set; }
        public string LocalizacaoSaida { get; set; }
        public string TipoTransferencia { get; set; }
        public bool LocalizacaoStatus { get; set; } = true;
        public string CodItem { get; set; }
        public string DescItem { get; set; }
        public string Un { get; set; }
        public string Conta { get; set; }
        public string NF { get; set; }
        public string Serie { get; set; }
        public string Lote { get; set; }
        public string Saldo { get; set; }
        public string Quantidade { get; set; }
    }
}
