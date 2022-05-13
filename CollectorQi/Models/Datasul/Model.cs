using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

namespace CollectorQi
{
    public class TempTable
    {
        public string dataType { get; set; }
        public string name     { get; set; }
        public string value    { get; set; }
        public string type     { get; set; }
    }

    public class TempTableValues
    {
        public JArray records { get; set; }
        public JArray fields  { get; set; }
    }

    public class ModelDeposito
    {
        public string codDepos { get; set; } = string.Empty;
        public string nome     { get; set; } = string.Empty;

    } 

    public class ModelEstabelec
    {
        public string codEstabel { get; set; } = string.Empty;
        public string nome       { get; set; } = string.Empty;
    }


    public class ModelDepositoTransfere
    {
        public string   codEstabel      { get; set; } = string.Empty;
        public string   itCodigo        { get; set; } = string.Empty;
        public string   nroDocto        { get; set; } = string.Empty;
        public string   codDeposSaida   { get; set; } = string.Empty;
        public string   codLocaliz      { get; set; } = string.Empty;
        public string   codRefer        { get; set; } = string.Empty;
        public string   codLote         { get; set; } = string.Empty;
        public DateTime? dtValiLote     { get; set; } 
        public string   codDeposEntrada { get; set; } = string.Empty;
        public decimal  qtidadeTransf   { get; set; } = 0;
        public string   codUsuario      { get; set; } = string.Empty;
    }

    public class ModelItem
    {
        public string itCodigo    { get; set; } = string.Empty;
        public string descItem    { get; set; } = string.Empty;
        public string depositoPad { get; set; } = string.Empty;
        public string codLocaliz  { get; set; } = string.Empty;
        public bool   locUnica    { get; set; } = false;
        public int    tipoConEst  { get; set; } = 0;
        public bool   contrQualid { get; set; } = false;
        public bool   logOrigExt  { get; set; } = false;
        public bool   fraciona    { get; set; } = false;
        public string un          { get; set; } = string.Empty;
        public int    seqInteg    { get; set; } = 0;
        public int    tpInteg     { get; set; } = 0;
        public int permSaldoNeg { get; set; } = 0;
    }

    public class ModelSaldoEstoq
    {
        public string   codEstabel       { get; set; } = String.Empty;
        public string   itCodigo         { get; set; } = String.Empty;
        public string   codDepos         { get; set; } = String.Empty;
        public string   codRefer         { get; set; } = String.Empty;
        public string   codLocaliz       { get; set; } = String.Empty;
        public string   codLote          { get; set; } = String.Empty;
        public string   dtValiLote       { get; set; } = String.Empty;
        public decimal  qtidadeAtu       { get; set; } = 0;
        //public string   dtVersaoSaldo    { get; set; } = String.Empty;
        public int nrTrans { get; set; } = 0;
    }

    public class ModelInventario
    {
        public string  dtSaldo      { get; set; } = String.Empty;
        public string  itCodigo     { get; set; } = String.Empty;
        public string  codEstabel   { get; set; } = String.Empty;
        public string  codDepos     { get; set; } = String.Empty;
        public string  codLocaliz   { get; set; } = String.Empty;
        public string  lote         { get; set; } = String.Empty;
        public string  dtUltSaida   { get; set; } = String.Empty;
        public string  dtUltEntr    { get; set; } = String.Empty;
        public string  situacao     { get; set; } = String.Empty;
        public int     nrFicha      { get; set; } = 0;
        public decimal? valApurado  { get; set; } = 0;
        public string  codRefer     { get; set; } = String.Empty;
        public int     NumContagem  { get; set; } = 0;
    }

    public class ModelInventarioErro
    {
        public string dtSaldo { get; set; } = String.Empty;
        public string codEstabel { get; set; } = String.Empty;
        public string codDepos { get; set; } = String.Empty;
        public string codLocaliz { get; set; } = String.Empty;
        public string lote { get; set; } = String.Empty;
        public string codRefer { get; set; } = String.Empty;
        public string itCodigo { get; set; } = String.Empty;
        public string msgErro { get; set; } = String.Empty;


    }

    public class ModelRequisicao
    {
        public int       nrRequisicao      { get; set; } = 0;
        public string    nomeAbrev         { get; set; } = string.Empty;
        public string    dtRequisicao      { get; set; } = String.Empty;
        public int       situacao          { get; set; } = 0;
        public string    codEstabel        { get; set; } = String.Empty;
        public string    permAtenderPadrao { get; set; } = string.Empty;
        public string    permAtenderDevol  { get; set; }  = string.Empty;
    }

    public class ModelRequisicaoItem
    {
        public int     nrRequisicao    { get; set; } = 0;
        public int     sequencia       { get; set; } = 0;
        public string  itCodigo        { get; set; } = String.Empty;
        public decimal qtRequisitada   { get; set; } = 0;
        public decimal qtAtendida      { get; set; } = 0;
        public decimal qtaAtender      { get; set; } = 0;
        public decimal qtDevolvida     { get; set; } = 0;
        public decimal qtaDevolver     { get; set; } = 0;
        public decimal seqInteg        { get; set; } = 0;
        public decimal tpInteg         { get; set; } = 0;
    }

    public class ModelRequisicaoSaldoEstoqDev 
    {
        public int     nrRequisicao   { get; set; } = 0;
        public int     sequencia      { get; set; } = 0;
        public string  itCodigo       { get; set; } = string.Empty;
        public string  codEstabel     { get; set; } = string.Empty;
        public string  codDepos       { get; set; } = string.Empty;
        public string  lote	          { get; set; } = string.Empty;
        public string  codRefer       { get; set; } = string.Empty;
        public string  codLocaliz     { get; set; } = string.Empty;
        public decimal quantidade     { get; set; } = 0;
    }

    public class ModelRequisicaoErro
    {
        public int nrRequisicao { get; set; } = 0;
        public int sequencia { get; set; } = 0;
        public string itCodigo { get; set; } = String.Empty;
        public int seqErro { get; set; } = 0;
        public string descErro { get; set; } = String.Empty;

    }
}


