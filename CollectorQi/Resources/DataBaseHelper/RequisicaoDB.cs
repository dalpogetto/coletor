using CollectorQi.VO;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollectorQi.Resources.DataBaseHelper
{
    public static class RequisicaoDB
    {


        public async static Task<List<RequisicaoVO>> GetRequisicao(bool pIsDevolucao)
        {
            var dbAsync = new BaseOperations();
            try
            {
                List<RequisicaoVO> requisicao;

                if (pIsDevolucao)
                    requisicao =  await dbAsync.Connection.Table<RequisicaoVO>().Where(p => p.PermAtenderDevol).ToListAsync();
                else
                    requisicao = await dbAsync.Connection.Table<RequisicaoVO>().Where(p => p.PermAtenderPadrao).ToListAsync();

                return requisicao;
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
        public async static Task<List<RequisicaoVO>> GetRequisicaoDevolucao()
        {
            var dbAsync = new BaseOperations();
            try
            {
                var requisicao = await dbAsync.Connection.Table<RequisicaoVO>().Where(p => p.PermAtenderDevol).ToListAsync();

                return requisicao;
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
        }*/



        public async static Task<List<RequisicaoVO>> GetRequisicaoGeral()
        {
            var dbAsync = new BaseOperations();
            try
            {
                var requisicao = await dbAsync.Connection.Table<RequisicaoVO>().ToListAsync();

                return requisicao;
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


        public async static void AtualizaRequisicao(List<RequisicaoVO> pLstRequisicaoVO)
        {
            var dbAsync = new BaseOperations();
            try
            {
                for (int i = 0; i < pLstRequisicaoVO.Count; i++)
                {
                    /* Victor Alves 29/11/2019 - Corrige sincronização de Requisição */
                    await dbAsync.InsertOrReplaceAsync<RequisicaoVO>(pLstRequisicaoVO[i]);
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

        public async static void AtualizaRequisicao(RequisicaoVO pRequisicaoVO)
        {
            var dbAsync = new BaseOperations();
            try
            {
                /* Victor Alves 29/11/2019 - Corrige sincronização de Requisição */
                await dbAsync.InsertOrReplaceAsync(pRequisicaoVO);
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

        public async static void DeletarRequisicao(RequisicaoVO pRequisicao)
        {
            var dbAsync = new BaseOperations();
            try
            {
                await dbAsync.DeleteAsync(pRequisicao);
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
