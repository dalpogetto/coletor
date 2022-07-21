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
    public static class InventarioLocalizacaoDB
    {
        public static List<InventarioLocalizacaoVO> GetInventarioLocalizacaoByInventario(int inventarioById)
        {
            List<InventarioLocalizacaoVO> lstInventarioLocalizacao = new List<InventarioLocalizacaoVO>();
            var dbAsync = new BaseOperations();
            try
            {
                //lstInventarioLocalizacao = dbAsync.Connection.QueryAsync<InventarioLocalizacaoVO>("SELECT * FROM InventarioLocalizacaoVO WHERE inventarioId = ?", inventarioById).Result;

               // lstInventarioLocalizacao = dbAsync.Connection.Table<InventarioLocalizacaoVO>().Where(p => p.InventarioId == inventarioById).ToListAsync().Result;
                lstInventarioLocalizacao = dbAsync.Connection.Table<InventarioLocalizacaoVO>().ToListAsync().Result;
                return lstInventarioLocalizacao;

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

        public async static Task<List<InventarioLocalizacaoVO>> GetInventarioLocalizacaoByInventarioAsync(int inventarioById)
        {
            List<InventarioLocalizacaoVO> lstInventarioLocalizacao = new List<InventarioLocalizacaoVO>();
            var dbAsync = new BaseOperations();
            try
            {
                lstInventarioLocalizacao = await dbAsync.Connection.QueryAsync<InventarioLocalizacaoVO>("SELECT * FROM InventarioLocalizacaoVO WHERE inventarioId = ?", inventarioById);
                return lstInventarioLocalizacao;

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

        public static List<InventarioLocalizacaoVO> GetInventarioLocalizacaoDigitadoByInventarioId(int inventarioById)
        {
            List<InventarioLocalizacaoVO> lstInventarioLocalizacao = new List<InventarioLocalizacaoVO>();
            var dbAsync = new BaseOperations();
            try
            {
                lstInventarioLocalizacao = dbAsync.Connection.QueryAsync<InventarioLocalizacaoVO>("SELECT * FROM InventarioLocalizacaoVO WHERE inventarioId = ? " +
                                                                                                   "AND qtdDigitada"
                                                                                                    , inventarioById).Result;

             

                return lstInventarioLocalizacao;

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

        public static InventarioLocalizacaoVO GetInventarioLocalizacao(int byInventarioLocalizacaoId)
        {
            var dbAsync = new BaseOperations();
            try
            {
                var inventario = dbAsync.Connection.Table<InventarioLocalizacaoVO>().Where(p => p.InventarioLocalizacaoId == byInventarioLocalizacaoId).FirstOrDefaultAsync();

                return inventario.Result;
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

        public static InventarioLocalizacaoVO InserirInventarioLocalizacao(InventarioLocalizacaoVO byInventarioLocalizacao)
        {

            var dbAsync = new BaseOperations();
            try
            {
                _ = dbAsync.InsertAsync(byInventarioLocalizacao);

                var InventarioLocalizacao = dbAsync.Connection.Table<InventarioLocalizacaoVO>().ToListAsync().Result.LastOrDefault();

                return InventarioLocalizacao;

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

        public static bool ConfirmaQuantidadeDigitada(int byInventarioLocalizacaoId, bool byQtdDigitada)
        {
            var dbAsync = new BaseOperations();
            try
            {
                dbAsync.Connection.QueryAsync<InventarioLocalizacaoVO>("update InventarioLocalizacaoVO set   qtdDigitada      = ?" +
                                                                                       " where InventarioLocalizacaoId = ? ",
                                                                                        byQtdDigitada,
                                                                                        byInventarioLocalizacaoId);

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

        public static bool AtualizaQuantidadeInventario(InventarioLocalizacaoVO InventarioLocalizacao)
        {
            var dbAsync = new BaseOperations();
            try
            {

                dbAsync.Connection.QueryAsync<InventarioLocalizacaoVO>("update InventarioLocalizacaoVO set   valApurado         = ?" +
                                                                              " where InventarioLocalizacaoId = ? ",
                                                                              InventarioLocalizacao.ValApurado,
                                                                              InventarioLocalizacao.InventarioLocalizacaoId);

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

        public static List<InventarioLocalizacaoVO> GetInventarioLocalizacaoByQr(int inventarioById, string byItCodigo, string byCodLote)
        {
            List<InventarioLocalizacaoVO> lstInventarioLocalizacao = new List<InventarioLocalizacaoVO>();
            var dbAsync = new BaseOperations();
            try
            {
               
                lstInventarioLocalizacao = dbAsync.Connection.QueryAsync<InventarioLocalizacaoVO>("SELECT * FROM InventarioLocalizacaoVO WHERE inventarioId = ? " +
                                                                                                   "AND itCodigo     = ? " +
                                                                                                   "AND codLote      = ? ", inventarioById, byItCodigo, byCodLote).Result;


                return lstInventarioLocalizacao;

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


        public async static Task<bool> DeletarInventarioByInventarioId(int byInventarioId)
        {
            var dbAsync = new BaseOperations();
            try
            {
                await dbAsync.Connection.QueryAsync<InventarioLocalizacaoVO>("delete from InventarioLocalizacaoVO where inventarioId = ?", byInventarioId);
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
        public async static Task<bool> AtualizarInventarioLocalizacao(List<InventarioLocalizacaoVO> byLstInventarioLocalizacao)
        {
            var dbAsync = new BaseOperations();
            try
            {
                    for (int i = 0; i < byLstInventarioLocalizacao.Count; i++)
                    {
                        var InventarioLocalizacaoVO = await
                            dbAsync.Connection.QueryAsync<InventarioLocalizacaoVO>( 
                            "select * from InventarioLocalizacaoVO where inventarioId = ? " +
                                                             " and codLocaliz   = ? " +
                                                             " and codLote      = ? " +
                                                             " and codRefer     = ? " +
                                                             " and itCodigo     = ? ",
                                                         byLstInventarioLocalizacao[i].InventarioId,
                                                         byLstInventarioLocalizacao[i].CodLocaliz,
                                                         byLstInventarioLocalizacao[i].CodLote,
                                                         byLstInventarioLocalizacao[i].CodRefer,
                                                         byLstInventarioLocalizacao[i].ItCodigo);

                        if (InventarioLocalizacaoVO != null && InventarioLocalizacaoVO.Count > 0)
                        {
                            if (InventarioLocalizacaoVO[0].NrFicha != byLstInventarioLocalizacao[i].NrFicha)
                            {
                                InventarioLocalizacaoVO[0].NrFicha = byLstInventarioLocalizacao[i].NrFicha;
                                await dbAsync.UpdateAsync(InventarioLocalizacaoVO[0]);
                            }
                        }
                        else
                        {
                            await dbAsync.InsertAsync(byLstInventarioLocalizacao[i]);
                        }
                    }

                    return true;
                //}
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

        public static int GetCountInventarioLocalizacao()
        {
            var dbAsync = new BaseOperations();
            try
            {
                int intCount = dbAsync.Connection.Table<InventarioLocalizacaoVO>().CountAsync().Result;

                return intCount;
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
