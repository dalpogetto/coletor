using CollectorQi.VO;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Internals;
using System.Linq;
using System.Threading.Tasks;

namespace CollectorQi.Resources.DataBaseHelper
{
    public static class RequisicaoItemDB
    {

        public async static Task<int> GetMaxVersaoRequisicaoItem()
        {
            var dbAsync = new BaseOperations();
            try
            {
                int intVersaoInteg = 0;

                try
                {
                    var result = await dbAsync.Connection.Table<RequisicaoItemVO>().OrderByDescending(p => p.VersaoIntegracao).FirstOrDefaultAsync();

                    if (result != null)
                        intVersaoInteg = result.VersaoIntegracao;

                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return intVersaoInteg;

            }
            catch (SQLiteException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async static Task<List<RequisicaoItemVO>> GetRequisicaoItem()
        {
            var dbAsync = new BaseOperations();
            try
            {
                var requisicaoItem =  await dbAsync.Connection.Table<RequisicaoItemVO>().ToListAsync();

                return requisicaoItem;
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

        public async static Task<List<RequisicaoItemVO>> GetRequisicaoItemByRequisicao(int pNrRequisicao, bool pIsDevolucao)
        {
            var dbAsync = new BaseOperations();
            try
            {
                List<RequisicaoItemVO> requisicaoItem;

                if (pIsDevolucao)
                    requisicaoItem = await dbAsync.Connection.Table<RequisicaoItemVO>().Where(p => p.NrRequisicao == pNrRequisicao && p.QtaDevolver > 0).ToListAsync();
                else
                    requisicaoItem = await dbAsync.Connection.Table<RequisicaoItemVO>().Where(p => p.NrRequisicao == pNrRequisicao && p.QtaAtender > 0).ToListAsync();

                return requisicaoItem;
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


        public async static void AtualizaRequisicaoItem(List<RequisicaoItemVO> pLstRequisicaoItemVO)
        {
            var dbAsync = new BaseOperations();
            try
            {
                for (int i = 0; i < pLstRequisicaoItemVO.Count; i++)
                {
                    await dbAsync.Connection.InsertOrReplaceAsync(pLstRequisicaoItemVO[i]);
                }

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

        public static async Task<RequisicaoItemVO> GetRequisicaoItemAsync(RequisicaoItemVO pRequisicaoItemVO)
        {
            var dbAsync = new BaseOperations();
            try
            {
                return await dbAsync.Connection.GetAsync<RequisicaoItemVO>(pRequisicaoItemVO.RequisicaoItemKey);
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

        public async static Task AtualizaRequisicaoItem(RequisicaoItemVO pRequisicaoItemVO)
        {
            var dbAsync = new BaseOperations();
            try
            {
                await dbAsync.Connection.InsertOrReplaceAsync(pRequisicaoItemVO);
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

        public async static void DeletarRequisicaoItem(RequisicaoItemVO pRequisicaoItem)
        {
            var dbAsync = new BaseOperations();
            try
            {
                await dbAsync.DeleteAsync(pRequisicaoItem);
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
