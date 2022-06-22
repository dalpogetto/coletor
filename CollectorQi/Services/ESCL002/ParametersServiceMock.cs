using System;
using System.Collections.Generic;
using System.Text;
using CollectorQi.Services.ESCL002;

namespace CollectorQi.Services.ESCL002
{
    public static class ParametersServiceMock
    {
        public static ParametersService.ResultJson GetParametersResult() => new ParametersService.ResultJson
        {
            param = new ParametersService.ParametrosResult
            {
                CodEmitente = 0,
                UsuarioTotvs = "super",
                DiasXML = 0,
                CodEstabel = "101",
                NFRet = "",
                DtEntrada = "15/06/22",
                ValorTotal = 0,
                Serie = "",
                QtdeItem = 0
            }
        };

        public static ParametersService.ResultSend GetRepairs() => new ParametersService.ResultSend
        {
            //Mensagem = "OK: Parametros enviados com sucesso",
            ListaReparos = GetDefaultListRepair()
        };

        public static List<ParametersService.ResultRepair> GetDefaultListRepair() => new List<ParametersService.ResultRepair> { GetDefaultRepair() };

        public static ParametersService.ResultRepair GetDefaultRepair() => new ParametersService.ResultRepair
        { 
            CodEmitente = 10098,
            RowId = "0x0000000000d9040c",
            Qtde = 1,
            CodEstabel = "101",
            Digito = 4,
            Localiza = "",
            CodItem = "96.150.00004-8B",
            NumRR = "959328.0",
            Situacao = "F",
            CodFilial = "07",
            Mensagem = "OK: Teste...",
            Valor = Decimal.Parse("1201.16"),
            CodBarras = "",
            Origem = "Externo",
            DescItem = "K7 INK STAINNING CASSETE P/ RM4H PATS C/PPB"                
        };
    }
}
