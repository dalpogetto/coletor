using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Resources.DataBaseHelper.Batch;
using CollectorQi.Models.Datasul;
using CollectorQi.Views;
using CollectorQi.Resources;
using Xamarin.Forms;
using CollectorQi.Models.ESCL028;

namespace CollectorQi.Models
{

    public static class ConnectService
    {
        private static string ProgramName                     = "qip/qi0001.p";
        private static string ProgramNameCollector            = "qip/qi0004.p";
        private static string ProcedureNameLogonValidaUsuario = "LogonValidaUsuario";
        private static string ProcedureNameConectColetor      = "ConectColetor";
        private static string ProcedureNameGetDeposito        = "GetDeposito";
        private static string ProcedureNameGetEstabelec       = "GetEstabelec";
        private static string ProcedureNameDepositoTransfere  = "SetDepositoTransfere";
        private static string ProcedureNameGetInventario      = "GetInventario";
        private static string ProcedureNameSetInventario      = "SetInventario";
        private static string ProcedureNameGetRequisicao      = "GetRequisicao";
        private static string ProcedureNameSetRequisicao      = "SetRequisicao";
        private static string ProcedureNameSetDevolucao       = "SetDevolucao";

        private static bool IsExecConectColetor; 

        //private static string UsuarioLogin;public 
        public async static Task<string> ConectColetorAsync(string pStrUsuario, string pStrSenha, ProgressBarPopUp pProgressBarPopUp = null)
        {
            /* Verifica se está sendo feita execução em BackEnd */
            if (pProgressBarPopUp == null)
            {
                if (IsExecConectColetor)
                    return string.Empty; 
            }

            try
            {
                if (IsExecConectColetor)
                    return string.Empty;

                IsExecConectColetor = true;

                var result = String.Empty;
                /*var task = Task.Run(async () =>
                {*/
                    result = await ConectColetorSync(pStrUsuario, pStrSenha, pProgressBarPopUp);
            /*    });
             */
                /*if (pProgressBarPopUp != null)
                {
                    task.Wait();
                }*/

                //task.Wait();

                    Device.BeginInvokeOnMainThread(() =>
                {
                    if (pProgressBarPopUp != null)
                        pProgressBarPopUp.OnAcompanhar("Autenticando...");
                });


                return result.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                IsExecConectColetor = false;
            }
        }

        public async static Task<string> ConectColetorSync(string pStrUsuario, string pStrSenha, ProgressBarPopUp pProgressBarPopUp)
        {
            try
            {
                string strJson = "[{\"dataType\":\"character\"  ,\"name\":\"pcUsuario\"       ,\"value\":\"#usuario\"         ,\"type\":\"input\"}," +
                                  "{\"dataType\":\"character\"  ,\"name\":\"pcSenha\"         ,\"value\":\"#senha\"           ,\"type\":\"input\"}," +
                                  "{\"dataType\":\"integer\"    ,\"name\":\"piVersaoItem\"    ,\"value\":  #iVersaoItem       ,\"type\":\"input\"}," +
                                  "{\"dataType\":\"integer\"    ,\"name\":\"piNrTrans\"       ,\"value\": \"#iNrTrans\"       ,\"type\":\"input\"}," +
                                  "{\"dataType\":\"character\"  ,\"name\":\"pcRetorno\"       ,\"value\":\"\"                 ,\"type\":\"output\"}";

                strJson = strJson + ",{                                                             " +
                                  "       \"name\": \"ttEstabelec\",                              " +
                                  "       \"type\": \"output\",                                   " +
                                  "       \"dataType\": \"temptable\",                            " +
                                  "       \"value\": {                                            " +
                                  "           \"name\": \"ttEstabelec\",                          " +
                                  "           \"fields\": [{                                      " +
                                  "                   \"name\": \"codEstabel\",                   " +
                                  "                   \"label\": \"codEstabel\",                  " +
                                  "                   \"type\": \"character\"                     " +
                                  "               }, {                                            " +
                                  "                   \"name\": \"nome\",                         " +
                                  "                   \"label\": \"nome\",                        " +
                                  "                   \"type\": \"character\"                     " +
                                  "               }                                               " +
                                  "           ],                                                  " +
                                  "           \"records\": [{}                                    " +
                                  "           ]                                                   " +
                                  "       }                                                       " +
                                  "  }                                                            " +
                                  "";

                strJson = strJson + ",{                                                              " +
                                   "       \"name\": \"ttDeposito\",                                 " +
                                   "       \"type\": \"output\",                                     " +
                                   "       \"dataType\": \"temptable\",                              " +
                                   "       \"value\": {                                              " +
                                   "           \"name\": \"ttDeposito\",                             " +
                                   "           \"fields\": [{                                        " +
                                   "                   \"name\": \"codDepos\",                       " +
                                   "                   \"label\": \"codDepos\",                      " +
                                   "                   \"type\": \"character\"                       " +
                                   "               }, {                                              " +
                                   "                   \"name\": \"nome\",                           " +
                                   "                   \"label\": \"nome\",                          " +
                                   "                   \"type\": \"character\"                       " +
                                   "               }                                                 " +
                                   "           ],                                                    " +
                                   "           \"records\": [{}                                      " +
                                   "           ]                                                     " +
                                   "       }                                                         " +
                                    "  }                                                             " +
                                   "";

                strJson = strJson + ",{                                                               " +
                                    "     \"name\": \"ttItem\",                                       " +
                                    "     \"type\": \"output\",                                       " +
                                    "     \"dataType\": \"temptable\",                                " +
                                    "     \"value\": {                                                " +
                                    "         \"name\": \"ttItem\",                                   " +
                                    "         \"fields\": [{                                          " +
                                    "             \"name\":  \"ItCodigo\",                            " +
                                    "             \"label\": \"itCodigo\",                            " +
                                    "             \"type\":  \"character\"                            " +
                                    "             }, {                                                " +
                                    "             \"name\":  \"DescItem\",                            " +
                                    "             \"label\": \"DescItem\",                            " +
                                    "             \"type\":  \"character\"                            " +
                                    "             }, {                                                " +
                                    "             \"name\":  \"PermSaldoNeg\",                        " +
                                    "             \"label\": \"PermSaldoNeg\",                        " +
                                    "             \"type\":  \"integer\"                              " +

                            /*"             }, {                                                " +
                            "             \"name\":   \"DepositoPad\",                        " +
                            "             \"label\":  \"DepositoPad\",                        " +
                            "             \"type\":   \"character\"                           " +
                            "             }, {                                                " +
                            "             \"name\":  \"CodLocaliz\",                          " +
                            "             \"label\": \"CodLocaliz\",                          " +
                            "             \"type\":  \"character\"                            " +
                            "             }, {                                                " +
                            "             \"name\":  \"LocUnica\",                            " +
                            "             \"label\": \"LocUnica\",                            " +
                            "             \"type\":  \"logical\"                              " +
                          */  "             }, {                                                " +
                                    "             \"name\":  \"TipoConEst\",                          " +
                                    "             \"label\": \"TipoConEst\",                          " +
                                    "             \"type\":  \"integer\"                              " +
                               /*  "             }, {                                                " +
                                 "             \"name\":  \"ContrQualid\",                         " +
                                 "             \"label\": \"ContrQualid\",                         " +
                                 "             \"type\":  \"logical\"                              " +
                                 "             }, {                                                " +
                                 "             \"name\":  \"LogOrigExt\",                          " +
                                 "             \"label\": \"LogOrigExt\",                          " +
                                 "             \"type\":  \"logical\"                              " +
                                 "             }, {                                                " +
                                 "             \"name\":  \"Fraciona\",                            " +
                                 "             \"label\": \"Fraciona\",                            " +
                                 "             \"type\":  \"logical\"                              " +
                              */ "             }, {                                                " +
                                    "             \"name\":  \"Un\",                                  " +
                                    "             \"label\": \"Un\",                                  " +
                                    "             \"type\":  \"character\"                            " +
                                    "             }, {                                                " +
                                    "             \"name\":  \"TpInteg\",                             " +
                                    "             \"label\": \"TpInteg\",                             " +
                                    "             \"type\":  \"integer\"                              " +
                                    "             } , {                                               " +
                                    "             \"name\":  \"SeqInteg\",                            " +
                                    "             \"label\": \"SeqInteg\",                            " +
                                    "             \"type\":  \"integer\"                              " +
                                    "             }                                                   " +
                                    "             ],                                                  " +
                                    "         \"records\": [{}                                        " +
                                    "         ]                                                       " +
                                    "     }                                                           " +
                                    " }                                                               " +
                                    "                                                                 ";

                strJson = strJson + ",{                                                           " +
                                    "     \"name\": \"ttSaldoEstoq\",                             " +
                                    "     \"type\": \"output\",                                   " +
                                    "     \"dataType\": \"temptable\",                            " +
                                    "     \"value\": {                                            " +
                                    "         \"name\": \"ttSaldoEstoq\",                         " +
                                    "         \"fields\": [{                                      " +
                                    "             \"name\":  \"codDepos\",                        " +
                                    "             \"label\": \"codDepos\",                        " +
                                    "             \"type\":  \"character\"                        " +
                                    "             }, {                                            " +
                                    "             \"name\":  \"codEstabel\",                      " +
                                    "             \"label\": \"codEstabel\",                      " +
                                    "             \"type\":  \"character\"                        " +
                                    "             }, {                                            " +
                                    "             \"name\":   \"codLocaliz\",                     " +
                                    "             \"label\":  \"codLocaliz\",                     " +
                                    "             \"type\":   \"character\"                       " +
                                    "             }, {                                            " +
                                    "             \"name\":  \"codLote\",                         " +
                                    "             \"label\": \"codLote\",                         " +
                                    "             \"type\":  \"character\"                        " +
                                    "             }, {                                            " +
                                    "             \"name\":  \"itCodigo\",                        " +
                                    "             \"label\": \"itCodigo\",                        " +
                                    "             \"type\":  \"character\"                        " +
                                    "             }, {                                            " +
                                    "             \"name\":  \"codRefer\",                        " +
                                    "             \"label\": \"codRefer\",                        " +
                                    "             \"type\":  \"character\"                        " +
                                    "             }, {                                            " +
                                    "             \"name\":  \"dtValiLote\",                      " +
                                    "             \"label\": \"dtValiLote\",                      " +
                                    "             \"type\":  \"character\"                        " +
                                    "             }, {                                            " +
                                    "             \"name\":  \"qtidadeAtu\",                      " +
                                    "             \"label\": \"qtidadeAtu\",                      " +
                                    "             \"type\":  \"decimal\"                          " +
                                    "             }, {                                            " +
                                    "             \"name\":  \"nrTrans\",                         " +
                                    "             \"label\": \"nrTrans\",                         " +
                                    "             \"type\":  \"integer\"                          " +
                                    "             }                                               " +
                                    "         ],                                                  " +
                                    "         \"records\": [{}                                    " +
                                    "         ]                                                   " +
                                    "     }                                                       " +
                                    " }                                                           " +
                                    "]                                                            ";

                strJson = strJson.Replace("#usuario", pStrUsuario);
                strJson = strJson.Replace("#senha", pStrSenha);

                strJson = strJson.Replace("#iVersaoItem", ItemDB.GetMaxVersaoItem().ToString());

                strJson = strJson.Replace("#iNrTrans", SaldoEstoqDB.GetMaxVersaoNrTrans().ToString());

                //throw new Exception(strJson);

                string strToken = App.CallProcedureWithToken.userLogin(pStrUsuario);

                strJson = strJson.Trim().Replace("\t", "");

                string strResponse = App.CallProcedureWithToken.callProcedureWithToken(strToken, ProgramNameCollector, ProcedureNameConectColetor, strJson);

                var jsonTempTable = JsonConvert.DeserializeObject<List<TempTable>>(strResponse);

                string strLogin = String.Empty;

                Device.BeginInvokeOnMainThread(() =>
                {

                    if (pProgressBarPopUp != null)
                        pProgressBarPopUp.OnAcompanhar("Salvando Cadastros Banco de Dados...");
                });

                for (int i = 0; i < jsonTempTable.Count(); i++)
                {
                    if (jsonTempTable[i].name == "pcRetorno")
                    {
                        strLogin = jsonTempTable[i].value;
                    }
                    else if (jsonTempTable[i].name == "ttEstabelec")
                    {
                        ConnectService.CriaEstabelecimento(JsonConvert.DeserializeObject<TempTableValues>(jsonTempTable[i].value).records.ToObject<List<ModelEstabelec>>());
                    }
                    else if (jsonTempTable[i].name == "ttDeposito")
                    {
                        ConnectService.CriaDeposito(JsonConvert.DeserializeObject<TempTableValues>(jsonTempTable[i].value).records.ToObject<List<ModelDeposito>>());
                    }
                    else if (jsonTempTable[i].name == "ttItem")
                    {
                           Device.BeginInvokeOnMainThread(() =>
                        {

                            if (pProgressBarPopUp != null)
                                pProgressBarPopUp.OnAcompanhar("Salvando Cadastros Banco de Dados (Item)...");
                        });

                        await ConnectService.CriaItem(JsonConvert.DeserializeObject<TempTableValues>(jsonTempTable[i].value).records.ToObject<List<ModelItem>>());
                    }
                    else if (jsonTempTable[i].name == "ttSaldoEstoq")
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {

                            if (pProgressBarPopUp != null)
                                pProgressBarPopUp.OnAcompanhar("Salvando Cadastros Banco de Dados (Saldo Estoque)...");
                        });

                         await ConnectService.CriaSaldoEstoq(JsonConvert.DeserializeObject<TempTableValues>(jsonTempTable[i].value).records.ToObject<List<ModelSaldoEstoq>>());
                    }
                }

                /*
                Device.BeginInvokeOnMainThread(() =>
                {

                    if (pProgressBarPopUp != null)
                        pProgressBarPopUp.OnAcompanhar("Salvando Cadastros Banco de Dados (Requisição)...");
                });


                var tplLstRequisicaoERP = Models.Controller.GetRequisicao();

                Models.Controller.CriaRequisicao(tplLstRequisicaoERP.Item1, tplLstRequisicaoERP.Item2); */


                /*
                Device.BeginInvokeOnMainThread(() =>
                {

                    if (pProgressBarPopUp != null)
                        pProgressBarPopUp.OnAcompanhar("Efetuando autenticação...");
                });
                */

                return strLogin;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool MovtoEstoqMobile(bool pBlnSaldoMobile, List<ModelDepositoTransfere> lstDepositoTransfere, List<string> lstDeposConsidera = null)
        {
            for (int i = 0; i < lstDepositoTransfere.Count; i++)
            {

                var saldoEstoqSaidaAtu = SaldoEstoqDB.GetSaldoByKey(
                    lstDepositoTransfere[i].codEstabel,
                    lstDepositoTransfere[i].itCodigo,
                    lstDepositoTransfere[i].codDeposSaida,
                    lstDepositoTransfere[i].codRefer,
                    lstDepositoTransfere[i].codLocaliz,
                    lstDepositoTransfere[i].codLote);

                var saldoEstoqEntradaAtu = SaldoEstoqDB.GetSaldoByKey(
                    lstDepositoTransfere[i].codEstabel,
                    lstDepositoTransfere[i].itCodigo,
                    lstDepositoTransfere[i].codDeposEntrada,
                    lstDepositoTransfere[i].codRefer,
                    lstDepositoTransfere[i].codLocaliz,
                    lstDepositoTransfere[i].codLote);

                VO.SaldoEstoqVO movtoEstoqSaida   = null;
                VO.SaldoEstoqVO movtoEstoqEntrada = null;

                if (saldoEstoqSaidaAtu != null)
                {
                    saldoEstoqSaidaAtu.QtidadeMobile = saldoEstoqSaidaAtu.QtidadeMobile - lstDepositoTransfere[i].qtidadeTransf;
                    movtoEstoqSaida = saldoEstoqSaidaAtu;
                }
                else
                {
                    movtoEstoqSaida = new VO.SaldoEstoqVO
                    {
                        CodEstabel    = lstDepositoTransfere[i].codEstabel,
                        ItCodigo      = lstDepositoTransfere[i].itCodigo,
                        CodDepos      = lstDepositoTransfere[i].codDeposSaida,
                        CodRefer      = lstDepositoTransfere[i].codRefer,
                        CodLocaliz    = lstDepositoTransfere[i].codLocaliz,
                        CodLote       = lstDepositoTransfere[i].codLote,
                        DtValiLote    = lstDepositoTransfere[i].dtValiLote,
                        /* Faz a transferencia de saldo, mas tira a quantidade do saldo que nesse caso já está 0 */
                        QtidadeMobile = (lstDepositoTransfere[i].qtidadeTransf * -1)
                    };
                }

                if (saldoEstoqEntradaAtu != null)
                {
                    saldoEstoqEntradaAtu.QtidadeMobile = saldoEstoqEntradaAtu.QtidadeMobile + lstDepositoTransfere[i].qtidadeTransf;
                    movtoEstoqEntrada = saldoEstoqEntradaAtu;
                }
                else
                {
                    movtoEstoqEntrada = new VO.SaldoEstoqVO
                    {
                        CodEstabel    = lstDepositoTransfere[i].codEstabel,
                        ItCodigo      = lstDepositoTransfere[i].itCodigo,
                        CodDepos      = lstDepositoTransfere[i].codDeposEntrada,
                        CodRefer      = lstDepositoTransfere[i].codRefer,
                        CodLocaliz    = lstDepositoTransfere[i].codLocaliz,
                        CodLote       = lstDepositoTransfere[i].codLote,
                        DtValiLote    = lstDepositoTransfere[i].dtValiLote,
                        /* Faz a transferencia de saldo, mas soma a quantidade do saldo que nesse caso já está 0 */
                        QtidadeMobile = lstDepositoTransfere[i].qtidadeTransf
                    };
                }

                List<VO.SaldoEstoqVO> lstSaldoEntradaSaida = new List<VO.SaldoEstoqVO>();

                if (lstDeposConsidera != null)
                {
                    if (lstDeposConsidera.Exists(p => p.Equals(movtoEstoqSaida.CodDepos)))
                        lstSaldoEntradaSaida.Add(movtoEstoqSaida);

                    if (lstDeposConsidera.Exists(p => p.Equals(movtoEstoqEntrada.CodDepos)))
                        lstSaldoEntradaSaida.Add(movtoEstoqEntrada);
                }
                else
                {
                    lstSaldoEntradaSaida.Add(movtoEstoqSaida);
                    lstSaldoEntradaSaida.Add(movtoEstoqEntrada);
                }

                // Só atualiza se a movimentação for de origem Mobile
                // Se a movimentação é pelo TOTVS não efetua a validação
                if (pBlnSaldoMobile)
                {
                    for (int iBatch = 0; iBatch < lstSaldoEntradaSaida.Count; iBatch++)
                    {
                        if (lstSaldoEntradaSaida[iBatch].QtidadeMobile < 0)
                        {
                            throw new Exception("Movimentação não atualizada, saldo do depósito " + lstSaldoEntradaSaida[iBatch].CodDepos + " não pode ser negativo. ");
                        }
                    }
                }

                SaldoEstoqDB.AtualizarSaldoEstoq(lstSaldoEntradaSaida);
            }

            return true;
        }

        public async static void CriaEstabelecimento(List<ModelEstabelec> lstEstabelec)
        {
            #region Estabelecimento

            var dbAsync = new BaseOperations();

            var lstDbEstabelec = await dbAsync.Connection.Table<VO.EstabelecVO>().ToListAsync();

            await dbAsync.DeleteAllAsync<VO.EstabelecVO>();

            if (lstEstabelec != null)
            {

                for (int i = 0; i < lstEstabelec.Count; i++)
                {
                    await dbAsync.InsertAsync(new VO.EstabelecVO
                    {
                        CodEstabel = lstEstabelec[i].codEstabel,
                        Nome = lstEstabelec[i].nome
                    });
                } 
            }

            #endregion
        }

        public static string LogonValidaUsuario(string pStrUsuario, string pStrSenha)
        {
            string result = "#result";

            try
            {
                string strJsonLogin = "[{\"dataType\":\"character\",\"name\":\"pcUsuario\",\"value\":\"#usuario\",\"type\":\"input\"},{\"dataType\":\"character\",\"name\":\"pcSenha\",\"value\":\"#senha\",\"type\":\"input\"},{\"dataType\":\"character\",\"name\":\"pcRetorno\",\"value\":\"\",\"type\":\"output\"}]";
                strJsonLogin = strJsonLogin.Replace("#usuario", pStrUsuario);
                strJsonLogin = strJsonLogin.Replace("#senha", pStrSenha);

                string strToken = App.CallProcedureWithToken.userLogin(pStrUsuario);

                string strJson = strJsonLogin.Trim().Replace("\t", "");


                string strResponse = App.CallProcedureWithToken.callProcedureWithToken(strToken, ProgramName, ProcedureNameLogonValidaUsuario, strJson);
                if (strResponse.IndexOf("OK") >= 0) result = "OK";

                //UsuarioLogin = pStrUsuario;
                return result;

            }
            catch (Exception ex)
            {
                return result + " " + ex.Message;
            }
        }

        public async static Task<bool> CriaItem(List<ModelItem> item)
        {
            try
            {
                /*ItemDB itemDataBase = new ItemDB();*/

                List<VO.ItemVO> lstItemVO = new List<VO.ItemVO>();

                if (item != null)
                {
                    for (int i = 0; i < item.Count; i++)
                    {
                        try
                        {

                            if (item[i].tpInteg == 2)
                            {
                                await ItemDB.DeleteItemAsync(item[i].itCodigo);
                                continue;
                            }

                            lstItemVO.Add(new VO.ItemVO
                            {
                                ItCodigo = item[i].itCodigo.Trim(),
                                DescItem = item[i].descItem.Trim(),
                               // DepositoPad = item[i].depositoPad.Trim(),
                               // CodLocaliz = item[i].codLocaliz.Trim(),
                                LocUnica = item[i].locUnica,
                                TipoConEst = item[i].tipoConEst,
                                ContrQualid = item[i].contrQualid,
                                LogOrigExt = item[i].logOrigExt,
                                Fraciona = item[i].fraciona,
                                Un = item[i].un.Trim(),
                                VersaoIntegracao = item[i].seqInteg
                            });
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("(CriaItem) - " + ex.Message); 
                        }
                    }



                    await ItemDB.AtualizarItemAsync(lstItemVO);
                }

                // Busca todos os itens realmente atualizados;
                var items = await ItemDB.GetItemsAsync();
                // Atualiza Item para busca mais rapida (Backend)
                SecurityAuxiliar.ItemAll = items;

                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async static Task<bool> CriaSaldoEstoq(List<ModelSaldoEstoq> saldoEstoq) 
        {
            try
            {
                //SaldoEstoqDB saldoEstoqDBAsync = new SaldoEstoqDB();

                List<VO.SaldoEstoqVO> lstSaldoEstoqVO = new List<VO.SaldoEstoqVO>();

                if (saldoEstoq != null)
                {
                    bool blnIntegraPendencia = false;
                    // Verifica se tem transferencia pendente para atualizar Saldo Mobile
                    //BatchDepositoTransfereDB batchDepositoDb = new BatchDepositoTransfereDB();

                    var tplReturnPendente = BatchDepositoTransfereDB.GetBatchDepositoTransfereCount();

                    // Verifica se tem integração pendente para integração com sistema (se nao existir melhor a performance no DOWNLOAD do arquivo)
                    if (tplReturnPendente.Item1 > 0 ||
                        tplReturnPendente.Item3 > 0)
                         blnIntegraPendencia = true;

                    List<ModelDepositoTransfere> lstDepositoTransfere = new List<ModelDepositoTransfere>();

                    // Lista de depósito a ser considerado para movimentação de saldo MOBILE 
                    List<string> lstDeposConsidera = new List<string>();

                    for (int i = 0; i < saldoEstoq.Count; i++)
                    {
                        try
                        {
                            //DateTime  dtVersaoSaldo = DateTime.MinValue;
                            DateTime? dtValiLote = null;

                            /*if (!String.IsNullOrEmpty(saldoEstoq[i].dtVersaoSaldo))
                                dtVersaoSaldo = new DateTime(int.Parse(saldoEstoq[i].dtVersaoSaldo.Substring(6, 4)),
                                                             int.Parse(saldoEstoq[i].dtVersaoSaldo.Substring(3, 2)),
                                                             int.Parse(saldoEstoq[i].dtVersaoSaldo.Substring(0, 2)));*/

                             if (!String.IsNullOrEmpty(saldoEstoq[i].dtValiLote))
                                dtValiLote = new DateTime(int.Parse(saldoEstoq[i].dtValiLote.Substring(6)),
                                                          int.Parse(saldoEstoq[i].dtValiLote.Substring(3, 2)),
                                                          int.Parse(saldoEstoq[i].dtValiLote.Substring(0, 2)));


                            var saldoEstoqVO = new VO.SaldoEstoqVO
                            {
                                CodEstabel = saldoEstoq[i].codEstabel.Trim(),
                                ItCodigo = saldoEstoq[i].itCodigo.Trim(),
                                CodDepos = saldoEstoq[i].codDepos.Trim(),
                                CodRefer = saldoEstoq[i].codRefer.Trim(),
                                CodLocaliz = saldoEstoq[i].codLocaliz.Trim(),
                                CodLote = saldoEstoq[i].codLote.Trim(),
                                DtValiLote = dtValiLote,
                                QtidadeAtu = saldoEstoq[i].qtidadeAtu,
                                QtidadeMobile = saldoEstoq[i].qtidadeAtu,
                                NrTransVersao = saldoEstoq[i].nrTrans
                            };

                            lstDeposConsidera.Add(saldoEstoqVO.CodDepos);

                            if (blnIntegraPendencia)
                            {
                                var tupleBatch = BatchDepositoTransfereDB.GetBatchDepositoTransfereByPendenteErro(saldoEstoqVO);

                                var lstBatchSaida = tupleBatch.Item1;
                                var lstBatchEntrada = tupleBatch.Item2;

                                for (int iBatch = 0; iBatch < lstBatchSaida.Count; iBatch++)
                                {
                                    lstDepositoTransfere.Add(new ModelDepositoTransfere
                                    {
                                        codEstabel = lstBatchSaida[iBatch].CodEstabel,
                                        itCodigo = lstBatchSaida[iBatch].ItCodigo,
                                        codDeposSaida = lstBatchSaida[iBatch].CodDeposSaida,
                                        codLocaliz = lstBatchSaida[iBatch].CodLocaliz,
                                        codRefer = lstBatchSaida[iBatch].CodRefer,
                                        codLote = lstBatchSaida[iBatch].CodLote,
                                        dtValiLote = lstBatchSaida[iBatch].DtValiLote,
                                        codDeposEntrada = lstBatchSaida[iBatch].CodDeposEntrada,
                                        qtidadeTransf = lstBatchSaida[iBatch].QtidadeTransf,
                                        codUsuario = lstBatchSaida[iBatch].CodUsuario

                                    });
                                }

                                for (int iBatch = 0; iBatch < lstBatchEntrada.Count; iBatch++)
                                {
                                    lstDepositoTransfere.Add(new ModelDepositoTransfere
                                    {
                                        codEstabel = lstBatchEntrada[iBatch].CodEstabel,
                                        itCodigo = lstBatchEntrada[iBatch].ItCodigo,
                                        codDeposSaida = lstBatchEntrada[iBatch].CodDeposSaida,
                                        codLocaliz = lstBatchEntrada[iBatch].CodLocaliz,
                                        codRefer = lstBatchEntrada[iBatch].CodRefer,
                                        codLote = lstBatchEntrada[iBatch].CodLote,
                                        dtValiLote = lstBatchEntrada[iBatch].DtValiLote,
                                        codDeposEntrada = lstBatchEntrada[iBatch].CodDeposEntrada,
                                        qtidadeTransf = lstBatchEntrada[iBatch].QtidadeTransf,
                                        codUsuario = lstBatchEntrada[iBatch].CodUsuario

                                    });
                                }
                            }
                            lstSaldoEstoqVO.Add(saldoEstoqVO);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("(CriaSaldoEstoq) - " + ex.Message);
                        }
                    }

                    await SaldoEstoqDB.AtualizarSaldoEstoq(lstSaldoEstoqVO);
                    if (lstDepositoTransfere.Count > 0)
                        ConnectService.MovtoEstoqMobile(true, lstDepositoTransfere, lstDeposConsidera);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void CriaDeposito(List<ModelDeposito> deposito)
        {
            try
            {
                #region Deposito
               
                var lstDbDeposito = DepositoDB.GetDeposito();

                if (lstDbDeposito != null)
                {
                    for (int i = 0; i < lstDbDeposito.Count(); i++)
                    {
                        DepositoDB.DeletarDeposito(lstDbDeposito[i]);
                    }
                }

                if (deposito != null)
                {
                    for (int i = 0; i < deposito.Count; i++)
                    {
                        DepositoDB.InserirDeposito(new VO.DepositoVO
                        {
                            CodDepos = deposito[i].codDepos.Trim(),
                            Nome = deposito[i].nome.Trim()
                        });
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static List<ModelDeposito> GetDeposito()
        {

            try
            {

                string strJson = "[{                                                              " +
                                        "       \"name\": \"ttDeposito\",                              " +
                                        "       \"type\": \"output\",                                   " +
                                        "       \"dataType\": \"temptable\",                            " +
                                        "       \"value\": {                                            " +
                                        "           \"name\": \"ttDeposito\",                        " +
                                        "           \"fields\": [{                                      " +
                                        "                   \"name\": \"codDepos\",                   " +
                                        "                   \"label\": \"codDepos\",                  " +
                                        "                   \"type\": \"character\"                     " +
                                        "               }, {                                            " +
                                        "                   \"name\": \"nome\",                     " +
                                        "                   \"label\": \"nome\",                    " +
                                        "                   \"type\": \"character\"                     " +
                                        "               }" +
                                        "           ],                                                  " +
                                        "           \"records\": [{}                                    " +
                                        "           ]                                                   " +
                                        "       }                                                       " +
                                         "  }                                                           " +
                                        "]";

                string strToken = App.CallProcedureWithToken.userLogin(SecurityAuxiliar.CodUsuario);

                strJson = strJson.Trim().Replace("\t", "");

                String strResponse = App.CallProcedureWithToken.callProcedureWithToken(strToken, ProgramNameCollector, ProcedureNameGetDeposito, strJson);

                var lstRecords = DeserializeJsonWS<List<ModelDeposito>>(strResponse.ToString());

                return lstRecords;
            }
            catch (Exception ex)
            {
                Console.WriteLine("### Erro: " + ex.Message);
                return null;
            }

        }

        public static List<ModelEstabelec>  GetEstabelec()
        {

            try
            {

                string strJson = "[{                                                              " +
                                        "       \"name\": \"ttEstabelec\",                              " +
                                        "       \"type\": \"output\",                                   " +
                                        "       \"dataType\": \"temptable\",                            " +
                                        "       \"value\": {                                            " +
                                        "           \"name\": \"ttEstabelec\",                        " +
                                        "           \"fields\": [{                                      " +
                                        "                   \"name\": \"codEstabel\",                   " +
                                        "                   \"label\": \"codEstabel\",                  " +
                                        "                   \"type\": \"character\"                     " +
                                        "               }, {                                            " +
                                        "                   \"name\": \"nome\",                     " +
                                        "                   \"label\": \"nome\",                    " +
                                        "                   \"type\": \"character\"                     " +
                                        "               }" +
                                        "           ],                                                  " +
                                        "           \"records\": [{}                                    " +
                                        "           ]                                                   " +
                                        "       }                                                       " +
                                         "  }                                                           " +
                                        "]";

                string strToken = App.CallProcedureWithToken.userLogin(SecurityAuxiliar.CodUsuario);

                strJson = strJson.Trim().Replace("\t", "");

                String strResponse = App.CallProcedureWithToken.callProcedureWithToken(strToken, ProgramNameCollector, ProcedureNameGetEstabelec, strJson);

                var lstRecords = DeserializeJsonWS<List<ModelEstabelec>>(strResponse.ToString());

                return lstRecords;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static Tuple<TipoIntegracao,string> SetDepositoTransfere(List<ModelDepositoTransfere> lstDepositoTransfere)
        {
            try
            {

                string strJson = "[{                                                                    " +
                                        "       \"name\": \"ttDepositoTransfere\",                      " +
                                        "       \"type\": \"input\",                                    " +
                                        "       \"dataType\": \"temptable\",                            " +
                                        "       \"value\": {                                            " +
                                        "           \"name\": \"ttDepositoTransfere\",                  " +
                                        "           \"fields\": [{                                      " +
                                        "                   \"name\": \"codEstabel\",                   " +
                                        "                   \"label\": \"codEstabel\",                  " +
                                        "                   \"type\": \"character\"                     " +
                                        "               }, {                                            " +
                                        "                   \"name\": \"itCodigo\",                     " +
                                        "                   \"label\": \"itCodigo\",                    " +
                                        "                   \"type\": \"character\"                     " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"nroDocto\",                     " +
                                        "                   \"label\": \"nroDocto\",                    " +
                                        "                   \"type\": \"character\"                     " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"codDeposSaida\",                " +
                                        "                   \"label\": \"codDeposSaida\",               " +
                                        "                   \"type\": \"character\"                     " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"codLocaliz\",                   " +
                                        "                   \"label\": \"codLocaliz\",                  " +
                                        "                   \"type\": \"character\"                     " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"codRefer\",                     " +
                                        "                   \"label\": \"codRefer\",                    " +
                                        "                   \"type\": \"character\"                     " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"codLote\",                      " +
                                        "                   \"label\": \"codLote\",                     " +
                                        "                   \"type\": \"character\"                     " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"dtValiLote\",                   " +
                                        "                   \"label\": \"dtValiLote\",                  " +
                                        "                   \"type\": \"character\"                     " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"codDeposEntrada\",              " +
                                        "                   \"label\": \"codDeposEntrada\",             " +
                                        "                   \"type\": \"character\"                     " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"qtdTransfer\",                  " +
                                        "                   \"label\": \"qtdTransfer\",                 " +
                                        "                   \"type\": \"decimal\"                       " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"codUsuario\",                   " +
                                        "                   \"label\": \" codUsuario\",                 " +
                                        "                   \"type\": \"character\"                     " +
                                        "               }                                               " +
                                        "           ],                                                  " +
                                        "      \"name\": \"ttDepositoTransfere\",                       " +
                                        "      \"records\": [                                           ";


                string strModelo = " {                                                         " +
                                   "              \"codEstabel\": \"#codEstabel\",             " +
                                   "              \"itCodigo\":  \"#itCodigo\",                " +
                                   "              \"nroDocto\":  \"#nroDocto\",                " +
                                   "              \"codDeposSaida\": \"#codDeposSaida\",       " +
                                   "              \"codLocaliz\": \"#codLocaliz\",             " +
                                   "              \"codRefer\": \"#codRefer\",                 " +
                                   "              \"codLote\": \"#codLote\",                   " +
                                   "              \"dtValiLote\": \"#dtValiLote\",             " +
                                   "              \"codDeposEntrada\": \"#codDeposEntrada\",   " +
                                   "              \"qtdTransfer\": #qtdTransfer,               " +
                                   "              \"codUsuario\": \"#codUsuario\",             " +
                                   "          }   ";



                bool blnFirst = true;
                for (int i = 0; i < lstDepositoTransfere.Count(); i++)
                {
                    if (!blnFirst) strJson = strJson + ",";
                    if (blnFirst) blnFirst = false;
                    string strTemp = strModelo;

                    strTemp = strTemp.Replace("#codEstabel", lstDepositoTransfere[i].codEstabel.Trim());
                    strTemp = strTemp.Replace("#itCodigo", lstDepositoTransfere[i].itCodigo.Trim());
                    strTemp = strTemp.Replace("#nroDocto", lstDepositoTransfere[i].nroDocto.Trim());
                    strTemp = strTemp.Replace("#codDeposSaida", lstDepositoTransfere[i].codDeposSaida.Trim());
                    strTemp = strTemp.Replace("#codLocaliz", lstDepositoTransfere[i].codLocaliz.Trim());
                    strTemp = strTemp.Replace("#codRefer", lstDepositoTransfere[i].codRefer.Trim());
                    strTemp = strTemp.Replace("#codLote", lstDepositoTransfere[i].codLote.Trim());
                    strTemp = strTemp.Replace("#dtValiLote", (lstDepositoTransfere[i].dtValiLote.HasValue ? lstDepositoTransfere[i].dtValiLote.Value.ToString("dd/MM/yyyy") : String.Empty));
                    strTemp = strTemp.Replace("#codDeposEntrada", lstDepositoTransfere[i].codDeposEntrada.Trim());
                    strTemp = strTemp.Replace("#qtdTransfer", lstDepositoTransfere[i].qtidadeTransf.ToString());
                    strTemp = strTemp.Replace("#codUsuario", lstDepositoTransfere[i].codUsuario.Trim());

                    strJson = strJson + strTemp;
                }

                strJson = strJson + "     ]                                                             " + /* Records */
                                   "    }                                                              " +
                                   "  },                                                               ";

                strJson = strJson + "{                                                           " +
                                "     \"name\": \"ttSaldoEstoq\",                             " +
                                "     \"type\": \"output\",                                   " +
                                "     \"dataType\": \"temptable\",                            " +
                                "     \"value\": {                                            " +
                                "         \"name\": \"ttSaldoEstoq\",                         " +
                                "         \"fields\": [{                                      " +
                                "             \"name\":  \"codDepos\",                        " +
                                "             \"label\": \"codDepos\",                        " +
                                "             \"type\":  \"character\"                        " +
                                "             }, {                                            " +
                                "             \"name\":  \"codEstabel\",                      " +
                                "             \"label\": \"codEstabel\",                      " +
                                "             \"type\":  \"character\"                        " +
                                "             }, {                                            " +
                                "             \"name\":   \"codLocaliz\",                     " +
                                "             \"label\":  \"codLocaliz\",                     " +
                                "             \"type\":   \"character\"                       " +
                                "             }, {                                            " +
                                "             \"name\":  \"codLote\",                         " +
                                "             \"label\": \"codLote\",                         " +
                                "             \"type\":  \"character\"                        " +
                                "             }, {                                            " +
                                "             \"name\":  \"itCodigo\",                        " +
                                "             \"label\": \"itCodigo\",                        " +
                                "             \"type\":  \"character\"                        " +
                                "             }, {                                            " +
                                "             \"name\":  \"codRefer\",                        " +
                                "             \"label\": \"codRefer\",                        " +
                                "             \"type\":  \"character\"                        " +
                                "             }, {                                            " +
                                "             \"name\":  \"dtValiLote\",                      " +
                                "             \"label\": \"dtValiLote\",                      " +
                                "             \"type\":  \"character\"                        " +
                                "             }, {                                            " +
                                "             \"name\":  \"qtidadeAtu\",                      " +
                                "             \"label\": \"qtidadeAtu\",                      " +
                                "             \"type\":  \"decimal\"                          " +
                                "             }, {                                            " +
                                "             \"name\":  \"nrTrans\",                         " +
                                "             \"label\": \"nrTrans\",                         " +
                                "             \"type\":  \"integer\"                          " +
                                "             }                                               " +
                                "         ],                                                  " +
                                "         \"records\": [{}                                    " +
                                "         ]                                                   " +
                                "     }                                                       " +
                                " },                                                          ";

                strJson = strJson + "  {  \"dataType\": \"character\", \"name\": \"pcMensagem\", \"value\": \"\"         , \"type\": \"output\" }  ,  " +
                                    "  {  \"dataType\": \"integer\"  , \"name\": \"piNrTrans\" , \"value\": \"#iNrTrans\", \"type\": \"input\" }   ,  " +
                                    "  {  \"dataType\": \"character\"  , \"name\": \"pcMobile\"  , \"value\": \"#cMobile\" , \"type\": \"input\" }    " +
                                    " ]                                                                 ";


                strJson = strJson.Replace("#iNrTrans", SaldoEstoqDB.GetMaxVersaoNrTrans().ToString());

                string strToken = App.CallProcedureWithToken.userLogin(SecurityAuxiliar.CodUsuario);

                strJson = strJson.Trim().Replace("\t", "");

                String strResponse = App.CallProcedureWithToken.callProcedureWithToken(strToken, ProgramNameCollector, ProcedureNameDepositoTransfere, strJson);

                var jsonTempTable = JsonConvert.DeserializeObject<List<TempTable>>(strResponse);

                string strOk = String.Empty;

                for (int i = 0; i < jsonTempTable.Count(); i++)
                {
                    if (jsonTempTable[i].name == "pcMensagem")
                    {
                        strOk = jsonTempTable[i].value;
                    }
                    else if (jsonTempTable[i].name == "ttSaldoEstoq")
                    {
                        ConnectService.CriaSaldoEstoq(JsonConvert.DeserializeObject<TempTableValues>(jsonTempTable[i].value).records.ToObject<List<ModelSaldoEstoq>>());
                    }
                }

                /* Situação correta saldo estoq */

                if (strOk.Contains("OK"))
                {
                    return Tuple.Create<TipoIntegracao, string>(TipoIntegracao.IntegracaoOnline, strOk);            
                }
                else
                    return Tuple.Create<TipoIntegracao, string>(TipoIntegracao.IntegracaoOnlineErro, strOk);

            }
            catch (Exception ex)
            {
                return Tuple.Create<TipoIntegracao, string>(TipoIntegracao.IntegracaoOnlineErro, ex.Message);
            }
        }

        public static List<ModelInventario> GetInventario()
        {
    
            try
            {
                string strJson = "[{                                                                    " +
                                        "       \"name\": \"ttInventario\",                             " +
                                        "       \"type\": \"output\",                                   " +
                                        "       \"dataType\": \"temptable\",                            " +
                                        "       \"value\": {                                            " +
                                        "           \"name\": \"ttInventario\",                         " +
                                        "           \"fields\": [{                                      " +
                                        "                   \"name\": \"itCodigo\",                     " +
                                        "                   \"label\": \"itCodigo\",                    " +
                                        "                   \"type\": \"character\"                     " +
                                        "               }, {                                            " +
                                        "                   \"name\": \"dtSaldo\",                      " +
                                        "                   \"label\": \"dtSaldo\",                     " +
                                        "                   \"type\": \"date\"                          " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"codEstabel\",                   " +
                                        "                   \"label\": \"codEstabel\",                  " +
                                        "                   \"type\": \"character\"                     " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"codDepos\",                     " +
                                        "                   \"label\": \"codDepos\",                    " +
                                        "                   \"type\": \"character\"                     " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"codLocaliz\",                   " +
                                        "                   \"label\": \"codLocaliz\",                  " +
                                        "                   \"type\": \"character\"                     " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"lote\",                         " +
                                        "                   \"label\": \"lote\",                        " +
                                        "                   \"type\": \"character\"                     " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"dtUltEntr\",                    " +
                                        "                   \"label\": \"dtUltEntr\",                   " +
                                        "                   \"type\": \"date\"                          " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"dtUltSaida\",                   " +
                                        "                   \"label\": \"dtUltSaida\",                  " +
                                        "                   \"type\": \"date\"                          " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"situacao\",                     " +
                                        "                   \"label\": \"situacao\",                    " +
                                        "                   \"type\": \"integer\"                       " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"nrFicha\",                      " +
                                        "                   \"label\": \"nrFicha\",                     " +
                                        "                   \"type\": \"integer\"                       " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"valApurado\",                   " +
                                        "                   \"label\": \"valApurado\",                  " +
                                        "                   \"type\": \"decimal\"                       " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"codRefer\",                     " +
                                        "                   \"label\": \"codRefer\",                    " +
                                        "                   \"type\": \"character\"                     " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"NumContagem\",                  " +
                                        "                   \"label\": \"NumContagem\",                 " +
                                        "                   \"type\": \"integer\"                       " +
                                        "               }                                               " +
                                        "           ],                                                  " +
                                        "           \"records\": [{}                                    " +
                                        "           ]                                                   " +
                                        "       }                                                       " +
                                         "  }                                                           " +
                                        "]";

                string strToken = App.CallProcedureWithToken.userLogin(SecurityAuxiliar.CodUsuario);

                strJson = strJson.Trim().Replace("\t", "");

                String strResponse = App.CallProcedureWithToken.callProcedureWithToken(strToken, ProgramNameCollector, ProcedureNameGetInventario, strJson);

                //throw new Exception(strResponse);
                var lstRecords = DeserializeJsonWS<List<ModelInventario>>(strResponse.ToString());

                return lstRecords;
            }
            catch (Exception ex)
            {
                throw new Exception("(GetInventario) " + ex.Message);
            }
        }

        public static Tuple<TipoIntegracao,string> SetInventario(List<ModelInventario> lstInventario)
        {
            try
            {
                string strJson = "[{                                                                    " +
                                     "       \"name\": \"ttInventario\",                             " +
                                     "       \"type\": \"input\",                                    " +
                                     "       \"dataType\": \"temptable\",                            " +
                                     "       \"value\": {                                            " +
                                     "           \"name\": \"ttInventario\",                         " +
                                     "           \"fields\": [{                                      " +
                                     "                   \"name\": \"itCodigo\",                     " +
                                     "                   \"label\": \"itCodigo\",                    " +
                                     "                   \"type\": \"character\"                     " +
                                     "               }, {                                            " +
                                     "                   \"name\": \"dtSaldo\",                      " +
                                     "                   \"label\": \"dtSaldo\",                     " +
                                     "                   \"type\": \"date\"                          " +
                                     "               } , {                                           " +
                                     "                   \"name\": \"codEstabel\",                   " +
                                     "                   \"label\": \"codEstabel\",                  " +
                                     "                   \"type\": \"character\"                     " +
                                     "               } , {                                           " +
                                     "                   \"name\": \"codDepos\",                     " +
                                     "                   \"label\": \"codDepos\",                    " +
                                     "                   \"type\": \"character\"                     " +
                                     "               } , {                                           " +
                                     "                   \"name\": \"codLocaliz\",                   " +
                                     "                   \"label\": \"codLocaliz\",                  " +
                                     "                   \"type\": \"character\"                     " +
                                     "               } , {                                           " +
                                     "                   \"name\": \"lote\",                         " +
                                     "                   \"label\": \"lote\",                        " +
                                     "                   \"type\": \"character\"                     " +
                                     "               } , {                                           " +
                                     "                   \"name\": \"dtUltEntr\",                    " +
                                     "                   \"label\": \"dtUltEntr\",                   " +
                                     "                   \"type\": \"date\"                          " +
                                     "               } , {                                           " +
                                     "                   \"name\": \"dtUltSaida\",                   " +
                                     "                   \"label\": \"dtUltSaida\",                  " +
                                     "                   \"type\": \"date\"                          " +
                                     "               } , {                                           " +
                                     "                   \"name\": \"situacao\",                     " +
                                     "                   \"label\": \"situacao\",                    " +
                                     "                   \"type\": \"integer\"                       " +
                                     "               } , {                                           " +
                                     "                   \"name\": \"nrFicha\",                      " +
                                     "                   \"label\": \"nrFicha\",                     " +
                                     "                   \"type\": \"integer\"                       " +
                                     "               } , {                                           " +
                                     "                   \"name\": \"valApurado\",                   " +
                                     "                   \"label\": \"valApurado\",                  " +
                                     "                   \"type\": \"decimal\"                       " +
                                     "               } , {                                           " +
                                     "                   \"name\": \"codRefer\",                     " +
                                     "                   \"label\": \"codRefer\",                    " +
                                     "                   \"type\": \"character\"                     " +
                                     "               } , {                                           " +
                                     "                   \"name\": \"NumContagem\",                  " +
                                     "                   \"label\": \"NumContagem\",                 " +
                                     "                   \"type\": \"integer\"                       " +
                                     "               }                                               " +
                                     "           ],                                                  " +
                                     "           \"records\": [                                      ";

                string strModelo = " {                                                      " +
                                   "              \"itCodigo\"    : \"#itCodigo\",          " +
                                   "              \"dtSaldo\"     : \"#dtSaldo\",           " +
                                   "              \"codEstabel\"  : \"#codEstabel\",        " +
                                   "              \"codDepos\"    : \"#codDepos\",          " +
                                   "              \"codLocaliz\"  : \"#codLocaliz\",        " +
                                   "              \"lote\"        : \"#lote\",              " +
                                   "              \"dtUltEntr\"   : \"#dtUltEntr\",         " +
                                   "              \"dtUltSaida\"  : \"#dtUltSaida\",        " +
                                   "              \"nrFicha\"     :   #nrFicha,             " +
                                   "              \"valApurado\"  :   #valApurado,         " +
                                   "              \"codRefer\"    : \"#codRefer\",          " +
                                   "              \"NumContagem\":    #NumContagem          " +
                                   "          }   ";



                bool blnFirst = true;
                for (int i = 0; i < lstInventario.Count(); i++)
                {
                    if (!blnFirst) strJson = strJson + ",";
                    if (blnFirst) blnFirst = false;
                    string strTemp = strModelo;

                    strTemp = strTemp.Replace("#itCodigo"    , lstInventario[i].itCodigo.Trim());
                    strTemp = strTemp.Replace("#dtSaldo"     , lstInventario[i].dtSaldo.Trim());
                    strTemp = strTemp.Replace("#codEstabel"  , lstInventario[i].codEstabel.Trim());
                    strTemp = strTemp.Replace("#codDepos"    , lstInventario[i].codDepos.Trim());
                    strTemp = strTemp.Replace("#codLocaliz"  , lstInventario[i].codLocaliz.Trim());
                    strTemp = strTemp.Replace("#lote"        , lstInventario[i].lote.Trim());
                    strTemp = strTemp.Replace("#dtUltEntr"   , lstInventario[i].dtUltEntr.Trim());
                    strTemp = strTemp.Replace("#dtUltSaida"  , lstInventario[i].dtUltSaida.Trim());
                    strTemp = strTemp.Replace("#nrFicha"     , lstInventario[i].nrFicha.ToString().Trim());
                    strTemp = strTemp.Replace("#valApurado"  ,lstInventario[i].valApurado.ToString().Trim().Replace(",","."));
                    strTemp = strTemp.Replace("#codRefer"    ,lstInventario[i].codRefer.Trim().Trim());
                    strTemp = strTemp.Replace("#NumContagem" ,lstInventario[i].NumContagem.ToString().Trim());

                    strJson = strJson + strTemp;
                }

                strJson = strJson + "     ]                                                            " + /* Records */
                                   "    }                                                              " +
                                   "  },                                                               ";



                strJson = strJson + "{                                                           " +
                                "     \"name\": \"ttInventarioErro\",                             " +
                                "     \"type\": \"output\",                                   " +
                                "     \"dataType\": \"temptable\",                            " +
                                "     \"value\": {                                            " +
                                "         \"name\": \"ttInventarioErro\",                         " +
                                "         \"fields\": [{                                      " +
                                "             \"name\":  \"dtSaldo\",                        " +
                                "             \"label\": \"dtSaldo\",                        " +
                                "             \"type\":  \"date\"                        " +
                                "             }, {                                            " +
                                "             \"name\":  \"codEstabel\",                      " +
                                "             \"label\": \"codEstabel\",                      " +
                                "             \"type\":  \"character\"                        " +
                                "             }, {                                            " +
                                "             \"name\":   \"codDepos\",                     " +
                                "             \"label\":  \"codDepos\",                     " +
                                "             \"type\":   \"character\"                       " +
                                "             }, {                                            " +
                                "             \"name\":  \"codLocaliz\",                         " +
                                "             \"label\": \"codLocaliz\",                         " +
                                "             \"type\":  \"character\"                        " +
                                "             }, {                                            " +
                                "             \"name\":  \"lote\",                        " +
                                "             \"label\": \"lote\",                        " +
                                "             \"type\":  \"character\"                        " +
                                "             }, {                                            " +
                                "             \"name\":  \"codRefer\",                        " +
                                "             \"label\": \"codRefer\",                        " +
                                "             \"type\":  \"character\"                        " +
                                "             }, {                                            " +
                                "             \"name\":  \"itCodigo\",                      " +
                                "             \"label\": \"itCodigo\",                      " +
                                "             \"type\":  \"character\"                        " +
                                "             }, {                                            " +
                                "             \"name\":  \"msgErro\",                      " +
                                "             \"label\": \"msgErro\",                      " +
                                "             \"type\":  \"character\"                          " +
                                "             }, " +
                                "         ],                                                  " +
                                "         \"records\": [{}                                    " +
                                "         ]                                                   " +
                                "     }                                                       " +
                                " },                                                          " +
                                "  {  \"dataType\": \"character\"  , \"name\": \"pcMobile\"  , \"value\": \"#cMobile\" , \"type\": \"input\" }    " +
                                "]                                                               ";

               
                string strToken = App.CallProcedureWithToken.userLogin(SecurityAuxiliar.CodUsuario);

                strJson = strJson.Trim().Replace("\t", "");

                String strResponse = App.CallProcedureWithToken.callProcedureWithToken(strToken, ProgramNameCollector, ProcedureNameSetInventario, strJson);

                var lstInventarioErro = JsonConvert.DeserializeObject<List<ModelInventarioErro>>(strResponse);


                /* Elimina registro que retorna vazio (erro Progress) */
                var remove = lstInventarioErro.FindIndex(p => p.dtSaldo == String.Empty &&
                                                 p.codEstabel == String.Empty &&
                                                 p.codDepos == String.Empty &&
                                                 p.codLocaliz == String.Empty &&
                                                 p.lote == String.Empty &&
                                                 p.codRefer == String.Empty &&
                                                 p.itCodigo == String.Empty &&
                                                 p.msgErro == String.Empty);

                lstInventarioErro.RemoveAt(remove);

                if (lstInventarioErro != null && lstInventarioErro.Count > 0)
                {
                    return Tuple.Create<TipoIntegracao, string>(TipoIntegracao.IntegracaoOnlineErro, "Inventário integrado erro.");
                }
                {
                    return Tuple.Create<TipoIntegracao, string>(TipoIntegracao.IntegracaoOnline, "Inventário integrado com sucesso.");
                }
            }
            catch (Exception ex)
            {
                return Tuple.Create<TipoIntegracao, string>(TipoIntegracao.IntegracaoOnlineErro, ex.Message);
            }           
        }

        public async static Task<bool> CriaInventario(List<ModelInventario> inventario)
        {
            // try
            // {
            //     List<InventarioVO> lstInventarioVO = new List<InventarioVO>();
            //         
            //     if (inventario != null)
            //     {
            //         for (int i = 0; i < inventario.Count; i++)
            //         {
            //             try
            //             {
            //                 List<InventarioItemVO> lstInventarioItemVO = new List<InventarioItemVO>();
            //
            //                 DateTime? dtUltEntr = null;
            //
            //                 if (!String.IsNullOrEmpty(inventario[i].dtUltEntr))
            //                     dtUltEntr = new DateTime(int.Parse(inventario[i].dtUltEntr.Substring(6, 4)),
            //                                                  int.Parse(inventario[i].dtUltEntr.Substring(3, 2)),
            //                                                  int.Parse(inventario[i].dtUltEntr.Substring(0, 2)));
            //
            //                 DateTime dtSaldoInventario = DateTime.MinValue;
            //
            //                 //if (!String.IsNullOrEmpty(inventario[i].dtSaldo))
            //                 //    dtSaldoInventario = new DateTime(int.Parse(inventario[i].dtSaldo.Substring(6, 4)),
            //                 //                                     int.Parse(inventario[i].dtSaldo.Substring(3, 2)),
            //                 //                                     int.Parse(inventario[i].dtSaldo.Substring(0, 2)));
            //
            //
            //                 //string teste = inventario[i].dtSaldo.Substring(0, 2) + inventario[i].dtSaldo.Substring(3, 2) + inventario[i].dtSaldo.Substring(6, 2);
            //
            //                 if (!String.IsNullOrEmpty(inventario[i].dtSaldo))
            //                     dtSaldoInventario = new DateTime(int.Parse(inventario[i].dtSaldo.Substring(6, 2)),
            //                                                      int.Parse(inventario[i].dtSaldo.Substring(3, 2)),
            //                                                      int.Parse(inventario[i].dtSaldo.Substring(0, 2)));
            //
            //
            //                 var inventarioVO = new VO.ESCL018.InventarioVO
            //                 {
            //                     CodEstabel = inventario[i].codEstabel,
            //                     CodDepos = inventario[i].codDepos,
            //                     DtInventario = dtSaldoInventario,
            //                     Contagem = inventario[i].NumContagem,                                
            //                     inventarioAtivo = true,
            //                     InventarioId  = inventario[i].idInventario,
            //                     DescEstabel = inventario[i].DescEstabel,
            //                     DescDepos = inventario[i].DescDepos
            //                 };
            //
            //                 lstInventarioVO.Add(inventarioVO);
            //                 int idInventario = await InventarioDB.AtualizaInventarioGetId(inventarioVO);
            //
            //                 var inventarioItemVO = new VO.ESCL018.InventarioItemVO
            //                 {
            //                     InventarioId   = idInventario,
            //                     CodLocaliz     = inventario[i].codLocaliz,
            //                     CodLote        = inventario[i].lote,
            //                     CodRefer       = inventario[i].codRefer,
            //                     ItCodigo       = inventario[i].itCodigo,
            //                     NrFicha        = inventario[i].nrFicha,
            //                     ValApurado     = inventario[i].valApurado??0,
            //                     DtUltEntr      = dtUltEntr
            //                 };
            //
            //                 lstInventarioItemVO.Add(inventarioItemVO);
            //
            //                 await InventarioItemDB.AtualizarInventarioItem(lstInventarioItemVO);
            //             }
            //             catch (Exception ex)
            //             {
            //                 throw new Exception("(CriaInventario) - " + ex.Message);
            //             }
            //         }
            //
            //         var lstInventarioAtual = InventarioDB.GetInventario();
            //
            //         for (int i = 0; i < lstInventarioAtual.Count; i++)
            //         {
            //             var inventarioAtual = lstInventarioVO.Find(
            //                 InventarioModel => InventarioModel.DtInventario == lstInventarioAtual[i].DtInventario &&
            //                                    InventarioModel.CodDepos        == lstInventarioAtual[i].CodDepos &&
            //                                    InventarioModel.CodEstabel      == lstInventarioAtual[i].CodEstabel //&&
            //                                    //InventarioModel.Contagem        == lstInventarioAtual[i].Contagem
            //                                    );
            //
            //             if (inventarioAtual == null)
            //             {
            //                 await InventarioDB.DesativaInventarioVO(lstInventarioAtual[i]);
            //                 //await InventarioDB.DeletarInventario(lstInventarioAtual[i]);
            //
            //                 //await InventarioItemDB.DeletarInventarioByInventarioId(lstInventarioAtual[i].InventarioId);
            //             }
            //         }
            //     }
            //
            //     return true;
            // }
            // catch (Exception ex)
            // {
            //     throw ex;
            // }

            return true;
        }

        public static bool CriaNotaFiscal(List<ModelNotaFiscal> notaFiscal)
        {
            try
            {
                List<VO.NotaFiscalVO> lstNotaFiscalVO = new List<VO.NotaFiscalVO>();

                if (notaFiscal != null)
                {
                    for (int i = 0; i < notaFiscal.Count; i++)
                    {
                        try
                        {
                            var notaFiscalVO = new VO.NotaFiscalVO
                            {
                                RowId = notaFiscal[i].rowId,
                                CodEstabel = notaFiscal[i].codEstabel,
                                CodItem = notaFiscal[i].codItem,
                                Localizacao = notaFiscal[i].localizacao,
                                DescricaoItem = notaFiscal[i].descricaoItem,
                                NroDocto = notaFiscal[i].nroDocto,
                                NumRR = notaFiscal[i].numRR,
                                Conferido = notaFiscal[i].conferido,
                                Relaciona = notaFiscal[i].relaciona,
                                CodFilial = notaFiscal[i].codFilial
                                //OrigemNotaFiscal = notaFiscal[i].origemNotaFiscal,
                                //NotaFiscalAtivo = notaFiscal[i].notaFiscalAtivo
                            };

                            lstNotaFiscalVO.Add(notaFiscalVO);

                            NotaFiscalDB.InserirNotaFiscal(notaFiscalVO);

                            //int idInventario = await InventarioDB.AtualizaInventarioGetId(inventarioVO);

                        }
                        catch (Exception ex)
                        {
                            throw new Exception("(CriaNotaFiscal) - " + ex.Message);
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool AtualizaNotaFiscal(ValidarReparosNotaFiscal validarReparosNotaFiscal)
        {
            try
            {
                if (validarReparosNotaFiscal != null)
                {                    
                    try
                    {
                        var notaFiscalVO = new VO.NotaFiscalVO
                        {
                            NumRR = validarReparosNotaFiscal.NumRR,
                            Conferido = validarReparosNotaFiscal.Conferido                           
                        };

                        NotaFiscalDB.AtualizaNotaFiscal(notaFiscalVO);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("(AtualizaNotaFiscal) - " + ex.Message);                        
                    }                    
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Tuple<List<ModelRequisicao>, List<ModelRequisicaoItem>, List<ModelRequisicaoSaldoEstoqDev>> GetRequisicao()
        {
            try
            {

               string strJson = "[        {\"dataType\":\"integer\"    ,\"name\":\"piVersaoRequisicao\"    ,\"value\":  #iVersaoRequisicao       ,\"type\":\"input\" } " +
                                        ",{                                                             " +
                                        "       \"name\": \"ttRequisicao\",                             " +
                                        "       \"type\": \"output\",                                   " +
                                        "       \"dataType\": \"temptable\",                            " +
                                        "       \"value\": {                                            " +
                                        "           \"name\": \"ttRequisicao\",                         " +
                                        "           \"fields\": [{                                      " +
                                        "                   \"name\": \"nrRequisicao\",                 " +
                                        "                   \"label\": \"nrRequisicao\",                " +
                                        "                   \"type\": \"integer\"                       " +
                                        "               }, {                                            " +
                                        "                   \"name\": \"nomeAbrev\",                    " +
                                        "                   \"label\": \"nomeAbrev\",                   " +
                                        "                   \"type\": \"character\"                     " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"dtRequisicao\",                 " +
                                        "                   \"label\": \"dtRequisicao\",                " +
                                        "                   \"type\": \"date\"                          " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"situacao\",                     " +
                                        "                   \"label\": \"situacao\",                    " +
                                        "                   \"type\": \"integer\"                       " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"codEstabel\",                   " +
                                        "                   \"label\": \"codEstabel\",                  " +
                                        "                   \"type\": \"character\"                     " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"permAtenderPadrao\",            " +
                                        "                   \"label\": \"permAtenderPadrao\",           " +
                                        "                   \"type\": \"logical\"                       " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"permAtenderDevol\",             " +
                                        "                   \"label\": \"permAtenderDevol\",            " +
                                        "                   \"type\": \"logical\"                       " +
                                        "               }                                               " +
                                        "           ],                                                  " +
                                        "           \"records\": [{}                                    " +
                                        "           ]                                                   " +
                                        "       }                                                       " +
                                        "  } ,                                                          " +
                                        "  {                                                            " + 
                                        "       \"name\": \"ttRequisicaoItem\",                         " +
                                        "       \"type\": \"output\",                                   " +
                                        "       \"dataType\": \"temptable\",                            " +
                                        "       \"value\": {                                            " +
                                        "           \"name\": \"ttRequisicaoItem\",                     " +
                                        "           \"fields\": [{                                      " +
                                        "                   \"name\": \"nrRequisicao\",                 " +
                                        "                   \"label\": \"nrRequisicao\",                " +
                                        "                   \"type\": \"integer\"                       " +
                                        "               }, {                                            " +
                                        "                   \"name\": \"sequencia\",                    " +
                                        "                   \"label\": \"sequencia\",                   " +
                                        "                   \"type\": \"integer\"                       " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"itCodigo\",                     " +
                                        "                   \"label\": \"itCodigo\",                    " +
                                        "                   \"type\": \"character\"                     " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"qtRequisitada\",                " +
                                        "                   \"label\": \"qtRequisitada\",               " +
                                        "                   \"type\": \"decimal\"                       " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"qtAtendida\",                   " +
                                        "                   \"label\": \"qtAtendida\",                  " +
                                        "                   \"type\": \"decimal\"                       " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"qtaAtender\",                   " +
                                        "                   \"label\": \"qtaAtender\",                  " +
                                        "                   \"type\": \"decimal\"                       " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"qtDevolvida\",                  " +
                                        "                   \"label\": \"qtDevolvida\",                 " +
                                        "                   \"type\": \"decimal\"                       " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"qtaDevolver\",                  " +
                                        "                   \"label\": \"qtaDevolver\",                 " +
                                        "                   \"type\": \"decimal\"                       " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"tpInteg\",                      " +
                                        "                   \"label\": \"tpInteg\",                     " +
                                        "                   \"type\": \"integer\"                       " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"SeqInteg\",                     " +
                                        "                   \"label\": \"SeqInteg\",                    " +
                                        "                   \"type\": \"integer\"                       " +
                                        "               }                                               " +
                                        "           ],                                                  " +
                                        "           \"records\": [{}                                    " +
                                        "           ]                                                   " +
                                        "       }                                                       " +
                                        "  },                                                           " +
                                        "  {                                                            " +
                                        "       \"name\": \"ttRequisicaoSaldoEstoqDev\",                " +
                                        "       \"type\": \"output\",                                   " +
                                        "       \"dataType\": \"temptable\",                            " +
                                        "       \"value\": {                                            " +
                                        "           \"name\": \"ttRequisicaoSaldoEstoqDev\",            " +
                                        "           \"fields\": [{                                      " +
                                        "                   \"name\": \"nrRequisicao\",                 " +
                                        "                   \"label\": \"nrRequisicao\",                " +
                                        "                   \"type\": \"integer\"                       " +
                                        "               }, {                                            " +
                                        "                   \"name\": \"sequencia\",                    " +
                                        "                   \"label\": \"sequencia\",                   " +
                                        "                   \"type\": \"integer\"                       " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"itCodigo\",                     " +
                                        "                   \"label\": \"itCodigo\",                    " +
                                        "                   \"type\": \"character\"                     " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"codEstabel\",                   " +
                                        "                   \"label\": \"codEstabel\",                  " +
                                        "                   \"type\": \"character\"                     " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"codDepos\",                     " +
                                        "                   \"label\": \"codDepos\",                    " +
                                        "                   \"type\": \"character\"                     " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"lote\",                         " +
                                        "                   \"label\": \"lote\",                        " +
                                        "                   \"type\": \"character\"                     " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"codRefer\",                     " +
                                        "                   \"label\": \"codRefer\",                    " +
                                        "                   \"type\": \"character\"                     " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"codLocaliz\",                   " +
                                        "                   \"label\": \"codLocaliz\",                  " +
                                        "                   \"type\": \"character\"                     " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"quantidade\",                   " +
                                        "                   \"label\": \"quantidade\",                  " +
                                        "                   \"type\": \"decimal\"                       " +
                                           "               }                                            " +
                                        "           ],                                                  " +
                                        "           \"records\": [{}                                    " +
                                        "           ]                                                   " +
                                        "       }                                                       " +
                                         "  }                                                           " +
                                        "]";


                strJson = strJson.Replace("#iVersaoRequisicao", "0");

                string strToken = App.CallProcedureWithToken.userLogin(SecurityAuxiliar.CodUsuario);

                strJson = strJson.Trim().Replace("\t", "");


                String strResponse = App.CallProcedureWithToken.callProcedureWithToken(strToken, ProgramNameCollector, ProcedureNameGetRequisicao, strJson);

                //throw new Exception(strResponse);

                var jsonTempTable = JsonConvert.DeserializeObject<List<TempTable>>(strResponse);

                string strOk = String.Empty;

                List<ModelRequisicao> lstRequisicao = new List<ModelRequisicao>();
                List<ModelRequisicaoItem> lstRequisicaoItem = new List<ModelRequisicaoItem>();
                List<ModelRequisicaoSaldoEstoqDev> lstRequisicaoSaldoEstoqDev = new List<ModelRequisicaoSaldoEstoqDev>();

                for (int i = 0; i < jsonTempTable.Count(); i++)
                {

                    if (jsonTempTable[i].name == "ttRequisicao")
                    {
                        lstRequisicao = JsonConvert.DeserializeObject<TempTableValues>(jsonTempTable[i].value).records.ToObject<List<ModelRequisicao>>();

                    }
                    else if (jsonTempTable[i].name == "ttRequisicaoItem")
                    {
                        lstRequisicaoItem = JsonConvert.DeserializeObject<TempTableValues>(jsonTempTable[i].value).records.ToObject<List<ModelRequisicaoItem>>();

                    }
                    else if (jsonTempTable[i].name == "ttRequisicaoSaldoEstoqDev")
                    {
                        lstRequisicaoSaldoEstoqDev = JsonConvert.DeserializeObject<TempTableValues>(jsonTempTable[i].value).records.ToObject<List<ModelRequisicaoSaldoEstoqDev>>();

                    }
                }
                return Tuple.Create<List<ModelRequisicao>, List<ModelRequisicaoItem>, List<ModelRequisicaoSaldoEstoqDev>>(lstRequisicao, lstRequisicaoItem, lstRequisicaoSaldoEstoqDev);

            }

            catch (Exception ex)
            {
                throw new Exception("(GetRequisicao) " + ex.Message);
            }
        }

        public static Tuple<TipoIntegracao,string> SetRequisicao(List<VO.RequisicaoItemSaldoEstoqVO> pLstRequisicaoItemSaldoEstoqVO)
        {


            try
            {
                string strJson = "[{                                                                  " +
                                       "       \"name\": \"ttRequisicaoItemRetorno\",                 " +
                                       "       \"type\": \"input\",                                   " +
                                       "       \"dataType\": \"temptable\",                           " +
                                       "       \"value\": {                                           " +
                                       "           \"name\": \"ttRequisicaoItemRetorno\",             " +
                                       "           \"fields\": [{                                     " +
                                       "                   \"name\": \"nrRequisicao\",                " +
                                       "                   \"label\": \"nrRequisicao\",               " +
                                       "                   \"type\": \"integer\"                      " +
                                       "               }, {                                           " +
                                       "                   \"name\": \"sequencia\",                   " +
                                       "                   \"label\": \"sequencia\",                  " +
                                       "                   \"type\": \"integer\"                      " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"itCodigo\",                    " +
                                       "                   \"label\": \"itCodigo\",                   " +
                                       "                   \"type\": \"character\"                    " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"codEstabel\",                  " +
                                       "                   \"label\": \"codEstabel\",                 " +
                                       "                   \"type\": \"character\"                    " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"codDepos\",                    " +
                                       "                   \"label\": \"codDepos\",                   " +
                                       "                   \"type\": \"character\"                    " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"lote\",                        " +
                                       "                   \"label\": \"lote\",                       " +
                                       "                   \"type\": \"character\"                    " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"dtValiLote\",                  " +
                                       "                   \"label\": \"dtValiLote\",                 " +
                                       "                   \"type\": \"date\"                         " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"codRefer\",                    " +
                                       "                   \"label\": \"codRefer\",                   " +
                                       "                   \"type\": \"character\"                    " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"codLocaliz\",                  " +
                                       "                   \"label\": \"codLocaliz\",                 " +
                                       "                   \"type\": \"character\"                    " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"codUsuario\",                  " +
                                       "                   \"label\": \"codUsuario\",                 " +
                                       "                   \"type\": \"character\"                    " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"dtAtend\",                     " +
                                       "                   \"label\": \"dtAtend\",                    " +
                                       "                   \"type\": \"date\"                         " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"dtDevol\",                     " +
                                       "                   \"label\": \"dtDevol\",                    " +
                                       "                   \"type\": \"date\"                         " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"qtRequisitada\",               " +
                                       "                   \"label\": \"qtRequisitada\",              " +
                                       "                   \"type\": \"decimal\"                      " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"qtAtendida\",                  " +
                                       "                   \"label\": \"qtAtendida\",                 " +
                                       "                   \"type\": \"decimal\"                      " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"qtaAtender\",                  " +
                                       "                   \"label\": \"qtaAtender\",                 " +
                                       "                   \"type\": \"decimal\"                      " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"qtDevolvida\",                 " +
                                       "                   \"label\": \"qtDevolvida\",                " +
                                       "                   \"type\": \"decimal\"                      " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"qtaDevolver\",                 " +
                                       "                   \"label\": \"qtaDevolver\",                " +
                                       "                   \"type\": \"decimal\"                      " +
                                       "               }                                              " +
                                       "           ],                                                 " +
                                       "           \"records\": [ ";

                string strModelo = " {                                                              " +
                                   "              \"nrRequisicao\"  :   #nrRequisicao,              " +
                                   "              \"sequencia\"     :   #sequencia   ,              " +
                                   "              \"itCodigo\"      : \"#itCodigo\"  ,              " +
                                   "              \"codEstabel\"    : \"#codEstabel\"  ,            " +
                                   "              \"codDepos\"      : \"#codDepos\"  ,              " +
                                   "              \"lote\"          : \"#lote\"  ,                  " +
                                   "              \"dtValiLote\"    : \"#dtValiLote\"  ,            " +
                                   "              \"codRefer\"      : \"#codRefer\"  ,              " +
                                   "              \"codLocaliz\"    : \"#codLocaliz\"  ,            " +
                                   "              \"codUsuario\"    : \"#codUsuario\"  ,            " +
                                   "              \"dtAtend\"       : \"#dtAtend\"  ,               " +
                                   "              \"dtDevol\"       : \"#dtDevol\"  ,               " +
                                   "              \"qtRequisitada\" : \"#qtRequisitada\"  ,         " +
                                   "              \"qtAtendida\"    : \"#qtAtendida\"  ,            " +
                                   "              \"qtaAtender\"    : \"#qtaAtender\"  ,            " +
                                   "              \"qtDevolvida\"    : \"#qtDevolvida\"  ,          " +
                                   "              \"qtaDevolver\"    : \"#qtaDevolver\"  ,          " +
                                   "          }   ";





                bool blnFirst = true;
                for (int i = 0; i < pLstRequisicaoItemSaldoEstoqVO.Count(); i++)
                {
                    if (!blnFirst) strJson = strJson + ",";
                    if (blnFirst) blnFirst = false;
                    string strTemp = strModelo;

                    strTemp = strTemp.Replace("#nrRequisicao", pLstRequisicaoItemSaldoEstoqVO[i].NrRequisicao.ToString());
                    strTemp = strTemp.Replace("#sequencia", pLstRequisicaoItemSaldoEstoqVO[i].Sequencia.ToString());
                    strTemp = strTemp.Replace("#itCodigo", pLstRequisicaoItemSaldoEstoqVO[i].ItCodigo.Trim());

                    strTemp = strTemp.Replace("#codEstabel", pLstRequisicaoItemSaldoEstoqVO[i].CodEstabel.Trim());
                    strTemp = strTemp.Replace("#codDepos", pLstRequisicaoItemSaldoEstoqVO[i].CodDepos.Trim());
                    strTemp = strTemp.Replace("#lote", pLstRequisicaoItemSaldoEstoqVO[i].CodLote.Trim());
                    //strTemp = strTemp.Replace("#dtValiLote", pLstRequisicaoItemSaldoEstoqVO[i].
                    strTemp = strTemp.Replace("#codRefer", pLstRequisicaoItemSaldoEstoqVO[i].CodRefer.Trim());
                    strTemp = strTemp.Replace("#codLocaliz", pLstRequisicaoItemSaldoEstoqVO[i].CodLocaliz.Trim());
                    strTemp = strTemp.Replace("#codUsuario", SecurityAuxiliar.CodUsuario.Trim());


                    strTemp = strTemp.Replace("#dtAtend", DateTime.Now.Date.ToString("dd/MM/yyyy"));
                    strTemp = strTemp.Replace("#dtDevol", DateTime.Now.Date.ToString("dd/MM/yyyy"));

                    // strTemp = strTemp.Replace("#dtAtend", pLstRequisicaoItemSaldoEstoqVO[i].dtAtend.Trim());
                    //          strTemp = strTemp.Replace("#qtRequisitada", pLstRequisicaoItemSaldoEstoqVO[i].QtRequisitada.ToString());

                    //          strTemp = strTemp.Replace("#qtAtendida", pLstRequisicaoItemSaldoEstoqVO[i].QtAtendida.ToString());

                    strTemp = strTemp.Replace("#qtAtendida", pLstRequisicaoItemSaldoEstoqVO[i].QtdAtender.ToString().Replace(",", "."));
                    strTemp = strTemp.Replace("#qtDevolvida", pLstRequisicaoItemSaldoEstoqVO[i].QtdDevolver.ToString().Replace(",", "."));

                    //  strTemp = strTemp.Replace("#qtaAtender", pLstRequisicaoItemSaldoEstoqVO[i].QtdAtender.ToString());

                    strJson = strJson + strTemp;
                }

                strJson = strJson + "     ]                                                            " + /* Records */
                                   "    }                                                              " +
                                   "  },                                                               ";


                strJson = strJson + " {                                                                 " +
                                        "       \"name\": \"ttRequisicaoErro\",                       " +
                                        "       \"type\": \"output\",                                   " +
                                        "       \"dataType\": \"temptable\",                            " +
                                        "       \"value\": {                                            " +
                                        "           \"name\": \"ttRequisicaoErro\",                   " +
                                        "           \"fields\": [{                                      " +
                                        "                   \"name\": \"nrRequisicao\",                 " +
                                        "                   \"label\": \"nrRequisicao\",                " +
                                        "                   \"type\": \"integer\"                       " +
                                        "               }, {                                            " +
                                        "                   \"name\": \"sequencia\",                    " +
                                        "                   \"label\": \"sequencia\",                   " +
                                        "                   \"type\": \"integer\"                       " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"itCodigo\",                     " +
                                        "                   \"label\": \"itCodigo\",                    " +
                                        "                   \"type\": \"character\"                     " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"seqErro\",                      " +
                                        "                   \"label\": \"seqErro\",                     " +
                                        "                   \"type\": \"integer\"                       " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"descErro\",                     " +
                                        "                   \"label\": \"descErro\",                    " +
                                        "                   \"type\": \"character\"                     " +
                                        "               }                                               " +
                                        "           ],                                                  " +
                                        "           \"records\": [{}                                    " +
                                        "           ]                                                   " +
                                        "       }                                                       " +
                                         "  } ,                                                         ";
                                  

                strJson = strJson + "{                                                           " +
                                    "     \"name\": \"ttSaldoEstoq\",                             " +
                                    "     \"type\": \"output\",                                   " +
                                    "     \"dataType\": \"temptable\",                            " +
                                    "     \"value\": {                                            " +
                                    "         \"name\": \"ttSaldoEstoq\",                         " +
                                    "         \"fields\": [{                                      " +
                                    "             \"name\":  \"codDepos\",                        " +
                                    "             \"label\": \"codDepos\",                        " +
                                    "             \"type\":  \"character\"                        " +
                                    "             }, {                                            " +
                                    "             \"name\":  \"codEstabel\",                      " +
                                    "             \"label\": \"codEstabel\",                      " +
                                    "             \"type\":  \"character\"                        " +
                                    "             }, {                                            " +
                                    "             \"name\":   \"codLocaliz\",                     " +
                                    "             \"label\":  \"codLocaliz\",                     " +
                                    "             \"type\":   \"character\"                       " +
                                    "             }, {                                            " +
                                    "             \"name\":  \"codLote\",                         " +
                                    "             \"label\": \"codLote\",                         " +
                                    "             \"type\":  \"character\"                        " +
                                    "             }, {                                            " +
                                    "             \"name\":  \"itCodigo\",                        " +
                                    "             \"label\": \"itCodigo\",                        " +
                                    "             \"type\":  \"character\"                        " +
                                    "             }, {                                            " +
                                    "             \"name\":  \"codRefer\",                        " +
                                    "             \"label\": \"codRefer\",                        " +
                                    "             \"type\":  \"character\"                        " +
                                    "             }, {                                            " +
                                    "             \"name\":  \"dtValiLote\",                      " +
                                    "             \"label\": \"dtValiLote\",                      " +
                                    "             \"type\":  \"character\"                        " +
                                    "             }, {                                            " +
                                    "             \"name\":  \"qtidadeAtu\",                      " +
                                    "             \"label\": \"qtidadeAtu\",                      " +
                                    "             \"type\":  \"decimal\"                          " +
                                    "             }, {                                            " +
                                    "             \"name\":  \"nrTrans\",                         " +
                                    "             \"label\": \"nrTrans\",                         " +
                                    "             \"type\":  \"integer\"                          " +
                                    "             }                                               " +
                                    "         ],                                                  " +
                                    "         \"records\": [{}                                    " +
                                    "         ]                                                   " +
                                    "     }                                                       " +
                                    " },                                                          ";

                strJson = strJson + "  {  \"dataType\": \"integer\"    , \"name\": \"piNrTrans\" , \"value\": \"#iNrTrans\", \"type\": \"input\" } ,  " +
                                    "  {  \"dataType\": \"character\"  , \"name\": \"pcMobile\"  , \"value\": \"#cMobile\" , \"type\": \"input\" }    " +
                                      " ]  ";

                strJson = strJson.Replace("#iNrTrans", SaldoEstoqDB.GetMaxVersaoNrTrans().ToString());

                string strToken = App.CallProcedureWithToken.userLogin(SecurityAuxiliar.CodUsuario);

                strJson = strJson.Trim().Replace("\t", "");

                String strResponse = App.CallProcedureWithToken.callProcedureWithToken(strToken, ProgramNameCollector, ProcedureNameSetRequisicao, strJson);

                var jsonTempTable = JsonConvert.DeserializeObject<List<TempTable>>(strResponse);

                List<ModelRequisicaoErro> lstModelReqErro = null;

                for (int i = 0; i < jsonTempTable.Count(); i++)
                {
                    if (jsonTempTable[i].name == "ttRequisicaoErro")
                    {
                        lstModelReqErro = JsonConvert.DeserializeObject<TempTableValues>(jsonTempTable[i].value).records.ToObject<List<ModelRequisicaoErro>>();

                    }
                    else if (jsonTempTable[i].name == "ttSaldoEstoq")
                    {
                        ConnectService.CriaSaldoEstoq(JsonConvert.DeserializeObject<TempTableValues>(jsonTempTable[i].value).records.ToObject<List<ModelSaldoEstoq>>());
                    }
                }

                /*
                var lstRequisicaoErro = JsonConvert.DeserializeObject<List<ModelRequisicaoErro>>(strResponse);

                /* Elimina registro que retorna vazio (erro Progress) 
                var remove = lstRequisicaoErro.FindIndex(p => p.nrRequisicao == 0 &&
                                                         p.sequencia == 0 &&
                                                         p.itCodigo == String.Empty &&
                                                         p.seqErro == 0 &&
                                                         p.descErro == String.Empty);

                lstRequisicaoErro.RemoveAt(remove);*/

                if (lstModelReqErro != null && lstModelReqErro.Count > 0)
                {
                    return Tuple.Create<TipoIntegracao, string>(TipoIntegracao.IntegracaoOnlineErro, lstModelReqErro[0].descErro);
                }
                {
                    return Tuple.Create<TipoIntegracao, string>(TipoIntegracao.IntegracaoOnline, "Requisição integrado com sucesso.");
                }
            }
            catch (Exception ex)
            {
                return Tuple.Create<TipoIntegracao, string>(TipoIntegracao.IntegracaoOnlineErro, ex.Message);
            }           
        }


        public static Tuple<TipoIntegracao, string> SetDevolucao(List<VO.RequisicaoItemSaldoEstoqVO> pLstRequisicaoItemSaldoEstoqVO)
        {


            try
            {
                string strJson = "[{                                                                  " +
                                       "       \"name\": \"ttRequisicaoItemRetorno\",                 " +
                                       "       \"type\": \"input\",                                   " +
                                       "       \"dataType\": \"temptable\",                           " +
                                       "       \"value\": {                                           " +
                                       "           \"name\": \"ttRequisicaoItemRetorno\",             " +
                                       "           \"fields\": [{                                     " +
                                       "                   \"name\": \"nrRequisicao\",                " +
                                       "                   \"label\": \"nrRequisicao\",               " +
                                       "                   \"type\": \"integer\"                      " +
                                       "               }, {                                           " +
                                       "                   \"name\": \"sequencia\",                   " +
                                       "                   \"label\": \"sequencia\",                  " +
                                       "                   \"type\": \"integer\"                      " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"itCodigo\",                    " +
                                       "                   \"label\": \"itCodigo\",                   " +
                                       "                   \"type\": \"character\"                    " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"codEstabel\",                  " +
                                       "                   \"label\": \"codEstabel\",                 " +
                                       "                   \"type\": \"character\"                    " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"codDepos\",                    " +
                                       "                   \"label\": \"codDepos\",                   " +
                                       "                   \"type\": \"character\"                    " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"lote\",                        " +
                                       "                   \"label\": \"lote\",                       " +
                                       "                   \"type\": \"character\"                    " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"dtValiLote\",                  " +
                                       "                   \"label\": \"dtValiLote\",                 " +
                                       "                   \"type\": \"date\"                         " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"codRefer\",                    " +
                                       "                   \"label\": \"codRefer\",                   " +
                                       "                   \"type\": \"character\"                    " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"codLocaliz\",                  " +
                                       "                   \"label\": \"codLocaliz\",                 " +
                                       "                   \"type\": \"character\"                    " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"codUsuario\",                  " +
                                       "                   \"label\": \"codUsuario\",                 " +
                                       "                   \"type\": \"character\"                    " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"dtAtend\",                     " +
                                       "                   \"label\": \"dtAtend\",                    " +
                                       "                   \"type\": \"date\"                         " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"dtDevol\",                     " +
                                       "                   \"label\": \"dtDevol\",                    " +
                                       "                   \"type\": \"date\"                         " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"qtRequisitada\",               " +
                                       "                   \"label\": \"qtRequisitada\",              " +
                                       "                   \"type\": \"decimal\"                      " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"qtAtendida\",                  " +
                                       "                   \"label\": \"qtAtendida\",                 " +
                                       "                   \"type\": \"decimal\"                      " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"qtaAtender\",                  " +
                                       "                   \"label\": \"qtaAtender\",                 " +
                                       "                   \"type\": \"decimal\"                      " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"qtDevolvida\",                 " +
                                       "                   \"label\": \"qtDevolvida\",                " +
                                       "                   \"type\": \"decimal\"                      " +
                                       "               } , {                                          " +
                                       "                   \"name\": \"qtaDevolver\",                 " +
                                       "                   \"label\": \"qtaDevolver\",                " +
                                       "                   \"type\": \"decimal\"                      " +
                                       "               }                                              " +
                                       "           ],                                                 " +
                                       "           \"records\": [ ";

                string strModelo = " {                                                              " +
                                   "              \"nrRequisicao\"  :   #nrRequisicao,              " +
                                   "              \"sequencia\"     :   #sequencia   ,              " +
                                   "              \"itCodigo\"      : \"#itCodigo\"  ,              " +
                                   "              \"codEstabel\"    : \"#codEstabel\"  ,            " +
                                   "              \"codDepos\"      : \"#codDepos\"  ,              " +
                                   "              \"lote\"          : \"#lote\"  ,                  " +
                                   "              \"dtValiLote\"    : \"#dtValiLote\"  ,            " +
                                   "              \"codRefer\"      : \"#codRefer\"  ,              " +
                                   "              \"codLocaliz\"    : \"#codLocaliz\"  ,            " +
                                   "              \"codUsuario\"    : \"#codUsuario\"  ,            " +
                                   "              \"dtAtend\"       : \"#dtAtend\"  ,               " +
                                   "              \"dtDevol\"       : \"#dtDevol\"  ,               " +
                                   "              \"qtRequisitada\" : \"#qtRequisitada\"  ,         " +
                                   "              \"qtAtendida\"    : \"#qtAtendida\"  ,            " +
                                   "              \"qtaAtender\"    : \"#qtaAtender\"  ,            " +
                                   "              \"qtDevolvida\"    : \"#qtDevolvida\"  ,          " +
                                   "              \"qtaDevolver\"    : \"#qtaDevolver\"  ,          " +
                                   "          }   ";




                bool blnFirst = true;
                for (int i = 0; i < pLstRequisicaoItemSaldoEstoqVO.Count(); i++)
                {


                    if (!blnFirst) strJson = strJson + ",";
                    if (blnFirst) blnFirst = false;
                    string strTemp = strModelo;

                    strTemp = strTemp.Replace("#nrRequisicao", pLstRequisicaoItemSaldoEstoqVO[i].NrRequisicao.ToString());
                    strTemp = strTemp.Replace("#sequencia", pLstRequisicaoItemSaldoEstoqVO[i].Sequencia.ToString());
                    strTemp = strTemp.Replace("#itCodigo", pLstRequisicaoItemSaldoEstoqVO[i].ItCodigo.Trim());

                    strTemp = strTemp.Replace("#codEstabel", pLstRequisicaoItemSaldoEstoqVO[i].CodEstabel.Trim());
                    strTemp = strTemp.Replace("#codDepos", pLstRequisicaoItemSaldoEstoqVO[i].CodDepos.Trim());
                    strTemp = strTemp.Replace("#lote", pLstRequisicaoItemSaldoEstoqVO[i].CodLote.Trim());
                    //strTemp = strTemp.Replace("#dtValiLote", pLstRequisicaoItemSaldoEstoqVO[i].
                    strTemp = strTemp.Replace("#codRefer", pLstRequisicaoItemSaldoEstoqVO[i].CodRefer.Trim());
                    strTemp = strTemp.Replace("#codLocaliz", pLstRequisicaoItemSaldoEstoqVO[i].CodLocaliz.Trim());
                    strTemp = strTemp.Replace("#codUsuario", SecurityAuxiliar.CodUsuario.Trim());


                    strTemp = strTemp.Replace("#dtAtend", DateTime.Now.Date.ToString("dd/MM/yyyy"));
                    strTemp = strTemp.Replace("#dtDevol", DateTime.Now.Date.ToString("dd/MM/yyyy"));

                    // strTemp = strTemp.Replace("#dtAtend", pLstRequisicaoItemSaldoEstoqVO[i].dtAtend.Trim());
                    //          strTemp = strTemp.Replace("#qtRequisitada", pLstRequisicaoItemSaldoEstoqVO[i].QtRequisitada.ToString());

                    //          strTemp = strTemp.Replace("#qtAtendida", pLstRequisicaoItemSaldoEstoqVO[i].QtAtendida.ToString());

                    strTemp = strTemp.Replace("#qtAtendida", pLstRequisicaoItemSaldoEstoqVO[i].QtdAtender.ToString().Replace(",","."));
                    strTemp = strTemp.Replace("#qtDevolvida", pLstRequisicaoItemSaldoEstoqVO[i].QtdDevolver.ToString()).Replace(",", ".");


                    strJson = strJson + strTemp;
                }

                strJson = strJson + "     ]                                                            " + /* Records */
                                   "    }                                                              " +
                                   "  },                                                               ";


                strJson = strJson + " {                                                                 " +
                                        "       \"name\": \"ttRequisicaoErro\",                       " +
                                        "       \"type\": \"output\",                                   " +
                                        "       \"dataType\": \"temptable\",                            " +
                                        "       \"value\": {                                            " +
                                        "           \"name\": \"ttRequisicaoErro\",                   " +
                                        "           \"fields\": [{                                      " +
                                        "                   \"name\": \"nrRequisicao\",                 " +
                                        "                   \"label\": \"nrRequisicao\",                " +
                                        "                   \"type\": \"integer\"                       " +
                                        "               }, {                                            " +
                                        "                   \"name\": \"sequencia\",                    " +
                                        "                   \"label\": \"sequencia\",                   " +
                                        "                   \"type\": \"integer\"                       " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"itCodigo\",                     " +
                                        "                   \"label\": \"itCodigo\",                    " +
                                        "                   \"type\": \"character\"                     " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"seqErro\",                      " +
                                        "                   \"label\": \"seqErro\",                     " +
                                        "                   \"type\": \"integer\"                       " +
                                        "               } , {                                           " +
                                        "                   \"name\": \"descErro\",                     " +
                                        "                   \"label\": \"descErro\",                    " +
                                        "                   \"type\": \"character\"                     " +
                                        "               }                                               " +
                                        "           ],                                                  " +
                                        "           \"records\": [{}                                    " +
                                        "           ]                                                   " +
                                        "       }                                                       " +
                                         "  } ,                                                         ";


                strJson = strJson + "{                                                           " +
                                    "     \"name\": \"ttSaldoEstoq\",                             " +
                                    "     \"type\": \"output\",                                   " +
                                    "     \"dataType\": \"temptable\",                            " +
                                    "     \"value\": {                                            " +
                                    "         \"name\": \"ttSaldoEstoq\",                         " +
                                    "         \"fields\": [{                                      " +
                                    "             \"name\":  \"codDepos\",                        " +
                                    "             \"label\": \"codDepos\",                        " +
                                    "             \"type\":  \"character\"                        " +
                                    "             }, {                                            " +
                                    "             \"name\":  \"codEstabel\",                      " +
                                    "             \"label\": \"codEstabel\",                      " +
                                    "             \"type\":  \"character\"                        " +
                                    "             }, {                                            " +
                                    "             \"name\":   \"codLocaliz\",                     " +
                                    "             \"label\":  \"codLocaliz\",                     " +
                                    "             \"type\":   \"character\"                       " +
                                    "             }, {                                            " +
                                    "             \"name\":  \"codLote\",                         " +
                                    "             \"label\": \"codLote\",                         " +
                                    "             \"type\":  \"character\"                        " +
                                    "             }, {                                            " +
                                    "             \"name\":  \"itCodigo\",                        " +
                                    "             \"label\": \"itCodigo\",                        " +
                                    "             \"type\":  \"character\"                        " +
                                    "             }, {                                            " +
                                    "             \"name\":  \"codRefer\",                        " +
                                    "             \"label\": \"codRefer\",                        " +
                                    "             \"type\":  \"character\"                        " +
                                    "             }, {                                            " +
                                    "             \"name\":  \"dtValiLote\",                      " +
                                    "             \"label\": \"dtValiLote\",                      " +
                                    "             \"type\":  \"character\"                        " +
                                    "             }, {                                            " +
                                    "             \"name\":  \"qtidadeAtu\",                      " +
                                    "             \"label\": \"qtidadeAtu\",                      " +
                                    "             \"type\":  \"decimal\"                          " +
                                    "             }, {                                            " +
                                    "             \"name\":  \"nrTrans\",                         " +
                                    "             \"label\": \"nrTrans\",                         " +
                                    "             \"type\":  \"integer\"                          " +
                                    "             }                                               " +
                                    "         ],                                                  " +
                                    "         \"records\": [{}                                    " +
                                    "         ]                                                   " +
                                    "     }                                                       " +
                                    " },                                                          ";

                strJson = strJson + "  {  \"dataType\": \"integer\"    , \"name\": \"piNrTrans\" , \"value\": \"#iNrTrans\", \"type\": \"input\" } ,  " +
                                    "  {  \"dataType\": \"character\"  , \"name\": \"pcMobile\"  , \"value\": \"#cMobile\" , \"type\": \"input\" }    " +
                                    " ]  ";


                strJson = strJson.Replace("#iNrTrans", SaldoEstoqDB.GetMaxVersaoNrTrans().ToString());

                string strToken = App.CallProcedureWithToken.userLogin(SecurityAuxiliar.CodUsuario);

                strJson = strJson.Trim().Replace("\t", "");

                String strResponse = App.CallProcedureWithToken.callProcedureWithToken(strToken, ProgramNameCollector, ProcedureNameSetDevolucao, strJson);

                var jsonTempTable = JsonConvert.DeserializeObject<List<TempTable>>(strResponse);

                List<ModelRequisicaoErro> lstModelReqErro = null;

                for (int i = 0; i < jsonTempTable.Count(); i++)
                {
                    if (jsonTempTable[i].name == "ttRequisicaoErro")
                    {
                        lstModelReqErro = JsonConvert.DeserializeObject<TempTableValues>(jsonTempTable[i].value).records.ToObject<List<ModelRequisicaoErro>>();

                    }
                    else if (jsonTempTable[i].name == "ttSaldoEstoq")
                    {
                        ConnectService.CriaSaldoEstoq(JsonConvert.DeserializeObject<TempTableValues>(jsonTempTable[i].value).records.ToObject<List<ModelSaldoEstoq>>());
                    }
                }

                /*
                var lstRequisicaoErro = JsonConvert.DeserializeObject<List<ModelRequisicaoErro>>(strResponse);

                /* Elimina registro que retorna vazio (erro Progress) 
                var remove = lstRequisicaoErro.FindIndex(p => p.nrRequisicao == 0 &&
                                                         p.sequencia == 0 &&
                                                         p.itCodigo == String.Empty &&
                                                         p.seqErro == 0 &&
                                                         p.descErro == String.Empty);

                lstRequisicaoErro.RemoveAt(remove);*/

                if (lstModelReqErro != null && lstModelReqErro.Count > 0)
                {
                    return Tuple.Create<TipoIntegracao, string>(TipoIntegracao.IntegracaoOnlineErro, lstModelReqErro[0].descErro);
                }
                {
                    return Tuple.Create<TipoIntegracao, string>(TipoIntegracao.IntegracaoOnline, "Requisição integrado com sucesso.");
                }
            }
            catch (Exception ex)
            {
                return Tuple.Create<TipoIntegracao, string>(TipoIntegracao.IntegracaoOnlineErro, ex.Message);
            }
        }


        public async static Task<bool> CriaRequisicao(List<ModelRequisicao> pLstRequisicao, List<ModelRequisicaoItem> pLstRequisicaoItem, List<ModelRequisicaoSaldoEstoqDev> pLstRequisicaoSaldoEstoqDev)
        {

            try
            {
                //List<VO.RequisicaoV> lstInventarioVO = new List<VO.InventarioVO>();
                    
                if (pLstRequisicao != null)
                {
                    List<VO.RequisicaoVO> lstRequisicaoVO = new List<VO.RequisicaoVO>();

                    for (int i = 0; i < pLstRequisicao.Count; i++)
                    {
                        try
                        {
                            DateTime? dtRequisicao = null;

                            if (!String.IsNullOrEmpty(pLstRequisicao[i].dtRequisicao))
                                dtRequisicao = new DateTime(int.Parse(pLstRequisicao[i].dtRequisicao.Substring(6, 4)),
                                                            int.Parse(pLstRequisicao[i].dtRequisicao.Substring(3, 2)),
                                                            int.Parse(pLstRequisicao[i].dtRequisicao.Substring(0, 2)));

                            bool blnPermAtenderDevol  = false;
                            bool blnPermAtenderPadrao = false;

                            if (!String.IsNullOrEmpty(pLstRequisicao[i].permAtenderDevol) &&
                                                      pLstRequisicao[i].permAtenderDevol == "True")
                                blnPermAtenderDevol = true;

                            if (!String.IsNullOrEmpty(pLstRequisicao[i].permAtenderPadrao) &&
                                                      pLstRequisicao[i].permAtenderPadrao == "True")
                                blnPermAtenderPadrao = true;


                            var requisicaoVO = new VO.RequisicaoVO
                            {

                                NrRequisicao = pLstRequisicao[i].nrRequisicao,
                                NomeAbrev = pLstRequisicao[i].nomeAbrev,
                                DtRequisicao = dtRequisicao,
                                Situacao = pLstRequisicao[i].situacao,
                                CodEstabel = pLstRequisicao[i].codEstabel,
                                PermAtenderDevol = blnPermAtenderDevol,
                                PermAtenderPadrao = blnPermAtenderPadrao
                            };

                            lstRequisicaoVO.Add(requisicaoVO);

                            //await InventarioItemDB.AtualizarInventarioItem(lstInventarioItemVO);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("(CriaRequisicao) - " + ex.ToString());
                        }
                    }

                    var lstRequisicaoAtual = await RequisicaoDB.GetRequisicaoGeral();

                    for (int i = 0; i < lstRequisicaoAtual.Count; i++)
                    {
                        var requisicaoAtual = lstRequisicaoVO.Find(
                            RequisicaoModel => RequisicaoModel.NrRequisicao == lstRequisicaoAtual[i].NrRequisicao);

                        if (requisicaoAtual == null)
                        {
                            RequisicaoDB.DeletarRequisicao(lstRequisicaoAtual[i]);

                            //await InventarioItemDB.DeletarInventarioByInventarioId(lstInventarioAtual[i].InventarioId);
                        }
                    }
                    try
                    { 
                    RequisicaoDB.AtualizaRequisicao(lstRequisicaoVO);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("(AtualizaRequisicao) - " + ex.ToString());
                    }
                }

                if (pLstRequisicaoItem != null)
                {
                    List<VO.RequisicaoItemVO> lstRequisicaoItemVO = new List<VO.RequisicaoItemVO>();

                    for (int i = 0; i < pLstRequisicaoItem.Count; i++)
                    {
                        try
                        {
                            var requisicaoItemVO = new VO.RequisicaoItemVO
                            {

                                NrRequisicao = pLstRequisicaoItem[i].nrRequisicao,
                                Sequencia = pLstRequisicaoItem[i].sequencia,
                                ItCodigo = pLstRequisicaoItem[i].itCodigo,

                                QtRequisitada = pLstRequisicaoItem[i].qtRequisitada,
                                QtAtendida = pLstRequisicaoItem[i].qtAtendida,
                                QtaAtender = pLstRequisicaoItem[i].qtaAtender,
                                QtDevolvida = pLstRequisicaoItem[i].qtDevolvida,
                                QtaDevolver = pLstRequisicaoItem[i].qtaDevolver,
                                QtAtendidaMobile = 0,
                                QtDevolvidaMobile = 0

                               // QtAtendidaMobile = pLstRequisicao[i].qtAtendida

                            };

                            lstRequisicaoItemVO.Add(requisicaoItemVO);

                        }
                        catch (Exception ex)
                        {
                            throw new Exception("(CriaItemRequisicao) - " + ex.ToString());
                        }
                    }

                    var lstRequisicaoItemAtual = await RequisicaoItemDB.GetRequisicaoItem();

                    for (int i = 0; i < lstRequisicaoItemAtual.Count; i++)
                    {
                        var requisicaoItemAtual = lstRequisicaoItemVO.Find(
                            RequisicaoItemModel => RequisicaoItemModel.NrRequisicao == lstRequisicaoItemAtual[i].NrRequisicao &&
                                                   RequisicaoItemModel.Sequencia    == lstRequisicaoItemAtual[i].Sequencia &&
                                                   RequisicaoItemModel.ItCodigo     == lstRequisicaoItemAtual[i].ItCodigo);

                        if (requisicaoItemAtual == null)
                        {
                            RequisicaoItemDB.DeletarRequisicaoItem(lstRequisicaoItemAtual[i]);
                        }
                    }

                    RequisicaoItemDB.AtualizaRequisicaoItem(lstRequisicaoItemVO);
                }

                if (pLstRequisicaoSaldoEstoqDev != null)
                {
                    List<VO.RequisicaoItemSaldoEstoqVO> lstRequisicaoItemSaldoEstoqDevVO = new List<VO.RequisicaoItemSaldoEstoqVO>();

                    foreach (var row in pLstRequisicaoSaldoEstoqDev)
                    {
                        try
                        {
                            var requisicaoItemSaldoEstoqVO = new VO.RequisicaoItemSaldoEstoqVO
                            {
                                NrRequisicao = row.nrRequisicao,
                                Sequencia = row.sequencia,
                                ItCodigo = row.itCodigo,
                                CodEstabel = row.codEstabel,
                                CodDepos = row.codDepos,
                                CodRefer = row.codRefer,
                                CodLocaliz = row.codLocaliz,
                                CodLote = row.lote,
                                IsDevolucao = true,
                                QtdAtender = 0,
                                QtdDevolver = row.quantidade,
                                QtdaDevolver = row.quantidade
                            };

                            lstRequisicaoItemSaldoEstoqDevVO.Add(requisicaoItemSaldoEstoqVO); 

                        }
                        catch (Exception ex)
                        {
                            throw new Exception("(CriaItemRequisicao) - " + ex.ToString());
                        }
                    }


                    var lstRequisicaoItemSaldoEstoqDev = await RequisicaoItemSaldoEstoqDB.GetRequisicaoItemSaldoEstoq();

                    foreach (var row in lstRequisicaoItemSaldoEstoqDev)
                    {
                        // Se nao for devolução, desconsidera
                        if (!row.IsDevolucao)
                            continue;

                        //if (!row.Q)
                        var requisicaoItemSaldoEstoqAtual = lstRequisicaoItemSaldoEstoqDevVO.Find(
                            RequisicaoItemSaldoEstoqModel => RequisicaoItemSaldoEstoqModel.NrRequisicao == row.NrRequisicao &&
                            RequisicaoItemSaldoEstoqModel.Sequencia == row.Sequencia &&
                            RequisicaoItemSaldoEstoqModel.ItCodigo == row.ItCodigo &&
                            RequisicaoItemSaldoEstoqModel.CodEstabel == row.CodEstabel &&
                            RequisicaoItemSaldoEstoqModel.CodDepos == row.CodDepos &&
                            RequisicaoItemSaldoEstoqModel.CodRefer == row.CodRefer &&
                            RequisicaoItemSaldoEstoqModel.CodLocaliz == row.CodLocaliz &&
                            RequisicaoItemSaldoEstoqModel.CodLote == row.CodLote);

                        if (requisicaoItemSaldoEstoqAtual == null)
                        {
                            await RequisicaoItemSaldoEstoqDB.DeletarRequisicaoItemSaldoEstoq(row);
                        }
                    }                 

                    await RequisicaoItemSaldoEstoqDB.AtualizaRequisicaoItemSaldoEstoq(lstRequisicaoItemSaldoEstoqDevVO);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
                
            }
        }

        public static T DeserializeJsonWS<T>(string pStrJson)
        {
            var jsonTempTable = JsonConvert.DeserializeObject<List<TempTable>>(pStrJson);

            var jsonTempTableValues = JsonConvert.DeserializeObject<TempTableValues>(jsonTempTable[0].value);

            return jsonTempTableValues.records.ToObject<T>();
        }

    }
}
