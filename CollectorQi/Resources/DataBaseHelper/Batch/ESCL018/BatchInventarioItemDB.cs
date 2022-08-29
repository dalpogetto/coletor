using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using Xamarin.Forms.Internals;
using System.Linq;
using System.Threading.Tasks;
using CollectorQi.VO.Batch.ESCL018;

namespace CollectorQi.Resources.DataBaseHelper.Batch.ESCL018
{
    public static class BatchInventarioItemDB
    {

        public static bool AtualizaStatusIntegracao(int byInventarioId, eStatusIntegracao byStatusIntegracao)
        {

            var dbAsync = new BaseOperations();
            try
            {
                dbAsync.Connection.QueryAsync<BatchInventarioItemVO>("UPDATE BatchInventarioVO SET statusIntegracao = ? " +
                                                                                             "WHERE inventarioId    = ? ",
                                                                          byStatusIntegracao,
                                                                          byInventarioId);

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

        /*
        public async static Task<bool> InserirBatchInventario(BatchInventarioItemVO byBatchInventarioVO)
        {
            var dbAsync = new BaseOperations();
            try
            {
                await dbAsync.InsertAsync(byBatchInventarioVO);

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

        */
        public async static Task<bool> AtualizaBatchInventario(BatchInventarioItemVO byBatchInventarioVO)
        {
            var dbAsync = new BaseOperations();
            try
            {
                await dbAsync.Connection.Table<BatchInventarioItemVO>().Where(p => p.InventarioItemId == byBatchInventarioVO.InventarioItemId).DeleteAsync();

                await dbAsync.InsertAsync(byBatchInventarioVO);

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

        public static bool DeletarBatchInventario(BatchInventarioItemVO byBatchInventarioVO)
        {
            var dbAsync = new BaseOperations();
            try
            {
                dbAsync.DeleteAsync(byBatchInventarioVO);

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

        public static List<BatchInventarioItemVO> GetBatchInventarioByStatus(eStatusIntegracao pStatusIntegracao)
        {
            var dbAsync = new BaseOperations();

            try
            {
                var result = dbAsync.Connection.QueryAsync<BatchInventarioItemVO>("select * from BatchInventarioItemVO where statusIntegracao = ?", pStatusIntegracao);

                return result.Result;

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

        public static BatchInventarioItemVO GetBatchInventario(int byInventarioId)
        {
            var dbAsync = new BaseOperations();

            try
            {
                //    var batchInventario = dbAsync.Connection.Table<BatchInventarioVO>().Where(p => p.IdInventario == byInventarioId).FirstOrDefaultAsync().Result;

                // return batchInventario;
                return null;
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

        public static List<BatchInventarioItemVO> GetBatchInventario()
        {
            List<BatchInventarioItemVO> result = new List<BatchInventarioItemVO>();
            var dbAsync = new BaseOperations();

            try
            {
                return dbAsync.Connection.Table<BatchInventarioItemVO>().ToListAsync().Result;
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

        public static Tuple<int, int, int> GetBatchInventarioCount()
        {
            var dbAsync = new BaseOperations();

            try
            {

                int intPendente = 0;
                int intSucesso = 0;
                int intErro = 0;

                var intAsyncPendente = dbAsync.Connection.Table<BatchInventarioItemVO>().CountAsync(p => p.StatusIntegracao == eStatusIntegracao.PendenteIntegracao);
                var intAsyncSucesso = dbAsync.Connection.Table<BatchInventarioItemVO>().CountAsync(p => p.StatusIntegracao == eStatusIntegracao.EnviadoIntegracao);
                var intAsyncErro = dbAsync.Connection.Table<BatchInventarioItemVO>().CountAsync(p => p.StatusIntegracao == eStatusIntegracao.ErroIntegracao);


                if (intAsyncPendente != null)
                    intPendente = intAsyncPendente.Result;

                if (intAsyncSucesso != null)
                    intSucesso = intAsyncSucesso.Result;

                if (intAsyncErro != null)
                    intErro = intAsyncErro.Result;

                var tpl = Tuple.Create(intPendente, intSucesso, intErro);

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
    }
}
