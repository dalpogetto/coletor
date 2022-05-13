using System;
using System.Collections.Generic;
using System.Text;
using CollectorQi.VO.Batch;
using SQLite;
using Xamarin.Forms.Internals;
using System.Linq;
using System.Threading.Tasks;

namespace CollectorQi.Resources.DataBaseHelper.Batch
{
    public static class BatchInventarioDB
    {

        public static bool AtualizaStatusIntegracao(int byInventarioId, eStatusIntegracao byStatusIntegracao)
        {

            var dbAsync = new BaseOperations();
            try
            {
                dbAsync.Connection.QueryAsync<BatchInventarioVO>("UPDATE BatchInventarioVO SET statusIntegracao = ? " +
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

        public async static Task<bool> InserirBatchInventario(BatchInventarioVO byBatchInventarioVO)
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

        public static bool DeletarBatchInventario(BatchInventarioVO byBatchInventarioVO)
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

        public static List<BatchInventarioVO> GetBatchInventarioByStatus(eStatusIntegracao pStatusIntegracao)
        {
            var dbAsync = new BaseOperations();

            try
            {
                var result = dbAsync.Connection.QueryAsync<BatchInventarioVO>("select * from BatchInventarioVO where statusIntegracao = ?", pStatusIntegracao);

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

        public static BatchInventarioVO GetBatchInventario(int byInventarioId)
        {
            var dbAsync = new BaseOperations();

            try
            {
                var batchInventario = dbAsync.Connection.Table<BatchInventarioVO>().Where(p => p.InventarioId == byInventarioId).FirstOrDefaultAsync().Result;

                return batchInventario;
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

        public static List<BatchInventarioVO> GetBatchInventario()
        {
            List<BatchInventarioVO> result = new List<BatchInventarioVO>();
            var dbAsync = new BaseOperations();

            try
            {
                return dbAsync.Connection.Table<BatchInventarioVO>().ToListAsync().Result;
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

                var intAsyncPendente = dbAsync.Connection.Table<BatchInventarioVO>().CountAsync(p => p.StatusIntegracao == eStatusIntegracao.PendenteIntegracao);
                var intAsyncSucesso  = dbAsync.Connection.Table<BatchInventarioVO>().CountAsync(p => p.StatusIntegracao == eStatusIntegracao.EnviadoIntegracao);
                var intAsyncErro     = dbAsync.Connection.Table<BatchInventarioVO>().CountAsync(p => p.StatusIntegracao == eStatusIntegracao.ErroIntegracao);


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
    } 
}
