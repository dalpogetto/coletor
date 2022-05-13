using System;
using System.Collections.Generic;
using System.Text;
using CollectorQi.VO.Batch;
using SQLite;
using Xamarin.Forms.Internals;
using System.Linq;

namespace CollectorQi.Resources.DataBaseHelper.Batch
{
    public static class BatchDepositoTransfereDB
    {

        public static void InserirBatchDepositoTransfere(BatchDepositoTransfereVO byBatchDepositoTransfereVO)
        {
            var dbAsync = new BaseOperations();
            try
            {
                dbAsync.InsertAsync(byBatchDepositoTransfereVO);
            }
            catch (SQLiteException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //dbAsync.Connection.CloseAsync();
            }
        }

        public static void InserirBatchDepositoTransfere(List<BatchDepositoTransfereVO> byBatchDepositoTransfereVO)
        {
            var dbAsync = new BaseOperations();
            try
            {
                dbAsync.InsertAllAsync(byBatchDepositoTransfereVO);
            }
            catch (SQLiteException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //dbAsync.Connection.CloseAsync();
            }
        }

        public static void AtualizarBatchDepositoTransfereIntegracaoById(BatchDepositoTransfereVO byBatchDepositoTransfereVO)
        {
            var dbAsync = new BaseOperations();
            try
            {

                dbAsync.Connection.QueryAsync<BatchDepositoTransfereVO>("UPDATE BatchDepositoTransfereVO set dtIntegracao        =?, " +
                                                                                             "msgIntegracao       =?, " +
                                                                                             "statusIntegracao    =?  " +
                                                                                       "Where intTransferenciaId  =?  ",
                                                                            byBatchDepositoTransfereVO.DtIntegracao,
                                                                            byBatchDepositoTransfereVO.MsgIntegracao,
                                                                            byBatchDepositoTransfereVO.StatusIntegracao,
                                                                            byBatchDepositoTransfereVO.IntTransferenciaId);
            }
            catch (SQLiteException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //dbAsync.Connection.CloseAsync();
            }
        }

        public static bool DeletarBatchDepositoTransfere(int byBatchDepositoTransfereId)
        {
            var dbAsync = new BaseOperations();
            try
            {

                dbAsync.Connection.QueryAsync<BatchDepositoTransfereVO>("DELETE FROM BatchDepositoTransfereVO Where intTransferenciaId  =?  ",
                                                                                                       byBatchDepositoTransfereId);
                return true;

            }
            catch (SQLiteException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //dbAsync.Connection.CloseAsync();
            }
        }


        public static List<BatchDepositoTransfereVO> GetBatchDepositoTransfereByStatus(eStatusIntegracao pStatusIntegracao)
        {
            List<BatchDepositoTransfereVO> result = new List<BatchDepositoTransfereVO>();
            var dbAsync = new BaseOperations();
            try
            {
                result = dbAsync.Connection.QueryAsync<BatchDepositoTransfereVO>("select * from BatchDepositoTransfereVO where statusIntegracao = ?", pStatusIntegracao).Result.ToList();
            }
            catch (SQLiteException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //dbAsync.Connection.CloseAsync();
            }
            return result;
        }

        public static List<BatchDepositoTransfereVO> GetBatchDepositoTransfere()
        {
            List<BatchDepositoTransfereVO> result = new List<BatchDepositoTransfereVO>();
            var dbAsync = new BaseOperations();
            try
            {
                return dbAsync.Connection.Table<BatchDepositoTransfereVO>().ToListAsync().Result;
            }
            catch (SQLiteException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //dbAsync.Connection.CloseAsync();
            }
        }


        public static Tuple<int, int, int> GetBatchDepositoTransfereCount()
        {
            var dbAsync = new BaseOperations();
            try
            {
                int intPendente = 0;
                int intSucesso = 0;
                int intErro = 0;

                var intAsyncPendente = dbAsync.Connection.Table<BatchDepositoTransfereVO>().CountAsync(p => p.StatusIntegracao == eStatusIntegracao.PendenteIntegracao);
                var intAsyncSucesso  = dbAsync.Connection.Table<BatchDepositoTransfereVO>().CountAsync(p => p.StatusIntegracao == eStatusIntegracao.EnviadoIntegracao);
                var intAsyncErro     = dbAsync.Connection.Table<BatchDepositoTransfereVO>().CountAsync(p => p.StatusIntegracao == eStatusIntegracao.ErroIntegracao);

                if (intAsyncPendente != null)
                    intPendente = intAsyncPendente.Result;

                if (intAsyncSucesso != null)
                    intSucesso = intAsyncSucesso.Result;

                if (intAsyncErro != null)
                    intErro = intAsyncErro.Result;

                var tpl = Tuple.Create<int, int, int>(intPendente, intSucesso, intErro);

                return tpl;
            }
            catch (SQLiteException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //dbAsync.Connection.CloseAsync();
            }
        }

        public static BatchDepositoTransfereVO GetBatchDepositoTransfere(int byBatchDepositoTransfereVO)
        {
            BatchDepositoTransfereVO batchDepositoTransfere = new BatchDepositoTransfereVO();
            var dbAsync = new BaseOperations();
            try
            {
                batchDepositoTransfere = dbAsync.Connection.Table<BatchDepositoTransfereVO>().FirstOrDefaultAsync(BatchDepositoTransfereVO => BatchDepositoTransfereVO.IntTransferenciaId == byBatchDepositoTransfereVO).Result;

            }
            catch (SQLiteException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //dbAsync.Connection.CloseAsync();
            }
            return batchDepositoTransfere;
        }


        public static Tuple<List<BatchDepositoTransfereVO>, List<BatchDepositoTransfereVO>> GetBatchDepositoTransfereByPendenteErro(VO.SaldoEstoqVO saldoEstoq)
        {
            Tuple<List<BatchDepositoTransfereVO>, List<BatchDepositoTransfereVO>> result = null;
            var dbAsync = new BaseOperations();
            try
            {

               var lstBatchSaida = dbAsync.Connection.QueryAsync<BatchDepositoTransfereVO>("select * from BatchDepositoTransfereVO WHERE codEstabel        = ? AND " +
                                                                                                                      " itCodigo          = ? AND " +
                                                                                                                      " codDeposSaida     = ? AND " +
                                                                                                                      " codRefer          = ? AND " +
                                                                                                                      " codLocaliz        = ? AND " +
                                                                                                                      " codLote           = ? AND " +
                                                                                                                      " (statusIntegracao = ? OR  " +
                                                                                                                      "  statusIntegracao = ?)    ",
                                                                                                                      saldoEstoq.CodEstabel,
                                                                                                                      saldoEstoq.ItCodigo,
                                                                                                                      saldoEstoq.CodDepos,
                                                                                                                      saldoEstoq.CodRefer,
                                                                                                                      saldoEstoq.CodLocaliz,
                                                                                                                      saldoEstoq.CodLote,
                                                                                                                      eStatusIntegracao.ErroIntegracao,
                                                                                                                      eStatusIntegracao.PendenteIntegracao);

                var lstBatchEntrada = dbAsync.Connection.QueryAsync<BatchDepositoTransfereVO>("select * from BatchDepositoTransfereVO WHERE codEstabel  = ? AND " +
                                                                                                                           " itCodigo              = ? AND " +
                                                                                                                           " codDeposEntrada       = ? AND " +
                                                                                                                           " codRefer              = ? AND " +
                                                                                                                           " codLocaliz            = ? AND " +
                                                                                                                           " codLote               = ? AND " +
                                                                                                                           " (statusIntegracao     = ? OR  " +
                                                                                                                           "  statusIntegracao     = ?)    ",
                                                                                                                           saldoEstoq.CodEstabel,
                                                                                                                           saldoEstoq.ItCodigo,
                                                                                                                           saldoEstoq.CodDepos,
                                                                                                                           saldoEstoq.CodRefer,
                                                                                                                           saldoEstoq.CodLocaliz,
                                                                                                                           saldoEstoq.CodLote,
                                                                                                                           eStatusIntegracao.ErroIntegracao,
                                                                                                                           eStatusIntegracao.PendenteIntegracao);

                result = Tuple.Create<List<BatchDepositoTransfereVO>, List<BatchDepositoTransfereVO>>(lstBatchSaida.Result, lstBatchEntrada.Result);

            }
            catch (SQLiteException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //dbAsync.Connection.CloseAsync();
            }

            return result;
        }     
    }
}
