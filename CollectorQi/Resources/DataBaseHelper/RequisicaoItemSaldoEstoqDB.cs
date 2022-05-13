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
    public static class RequisicaoItemSaldoEstoqDB
    {
        public async static Task AtualizaRequisicaoItemSaldoEstoq(List<RequisicaoItemSaldoEstoqVO> pLstRequisicaoItemSaldoEStoqVO)
        {
            var dbAsync = new BaseOperations();
            try
            {
                for (int i = 0; i < pLstRequisicaoItemSaldoEStoqVO.Count; i++)
                {
                    await dbAsync.Connection.InsertOrReplaceAsync(pLstRequisicaoItemSaldoEStoqVO[i]);
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
        }

        public async static Task AtualizaRequisicaoItemSaldoEstoq(RequisicaoItemSaldoEstoqVO pRequisicaoItemSaldoEStoqVO)
        {
            var dbAsync = new BaseOperations();
            try
            {
                await dbAsync.Connection.InsertOrReplaceAsync(pRequisicaoItemSaldoEStoqVO);
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

      
        public async static Task<List<RequisicaoItemSaldoEstoqVO>> GetRequisicaoItemSaldoEstoq (int pNrRequisicao, int pSequencia, string pItCodigo, bool pDevolucao)
        {
            var dbAsync = new BaseOperations();
            try
            {
                var lstReqAsync = dbAsync.Connection.Table<RequisicaoItemSaldoEstoqVO>().Where(p => p.NrRequisicao == pNrRequisicao &&
                                                                                                    p.Sequencia == p.Sequencia && 
                                                                                                    p.ItCodigo == pItCodigo &&
                                                                                                    p.IsDevolucao == pDevolucao);

                var lstReq = await lstReqAsync.ToListAsync();
                return lstReq;
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

        public async static Task DeletarRequisicaoItemSaldoEstoq(RequisicaoItemSaldoEstoqVO pRequisicaoItemSaldoEstoqVO)
        {
            var dbAsync = new BaseOperations();
            try
            {
                await dbAsync.DeleteAsync(pRequisicaoItemSaldoEstoqVO);
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

        public async static Task<List<RequisicaoItemSaldoEstoqVO>> GetRequisicaoItemSaldoEstoq()
        {
            var dbAsync = new BaseOperations();
            try
            {
                var lstReqAsync = await dbAsync.Connection.Table<RequisicaoItemSaldoEstoqVO>().ToListAsync();

                return lstReqAsync;
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

        public async static Task<List<RequisicaoItemSaldoEstoqVO>> GetRequisicaoItemSaldoEstoq(int pNrRequisicao, bool lDevolucao)
        {
            var dbAsync = new BaseOperations();
            try
            {
                var lstReqAsync = await dbAsync.Connection.Table<RequisicaoItemSaldoEstoqVO>().Where(p => p.NrRequisicao == pNrRequisicao && p.IsDevolucao == lDevolucao).ToListAsync();

                return lstReqAsync;
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
    }
}
