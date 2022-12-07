using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Internals;
using System.Linq;
using System.Threading.Tasks;
using CollectorQi.VO.ESCL018;

namespace CollectorQi.Resources.DataBaseHelper.ESCL018
{
    public static class InventarioItemDB
    {

        public async static Task<List<InventarioItemVO>> AtualizaInventarioItem(int byInventarioId, string byLocalizacao, List<InventarioItemVO> byLstInventarioItem)
        {
            var dbAsync = new BaseOperations();
            try
            {
                for (int i = 0; i < byLstInventarioItem.Count; i++)
                {
                    var inventarioItemVO = (await
                        dbAsync.Connection.QueryAsync<InventarioItemVO>(
                        "select * from InventarioItemVO where inventarioId = ? " +
                                                         " and codEstabel   = ? " +
                                                         " and codDepos      = ? " +
                                                         " and localizacao     = ? " +
                                                         " and lote     = ? " +
                                                         " and codItem     = ? ",
                                                     byLstInventarioItem[i].InventarioId,
                                                     byLstInventarioItem[i].CodEstabel,
                                                     byLstInventarioItem[i].CodDepos,
                                                     byLstInventarioItem[i].Localizacao,
                                                     byLstInventarioItem[i].Lote,
                                                     byLstInventarioItem[i].CodItem));

                    if (inventarioItemVO != null && inventarioItemVO.Count > 0)
                    {
                        byLstInventarioItem[i] = inventarioItemVO.FirstOrDefault();
                    }
                    else
                    {
                        await dbAsync.InsertAsync(byLstInventarioItem[i]);
                    }
                }

                return byLstInventarioItem;
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

        public async static void AtualizaInventarioItemBatch(InventarioItemVO byInventarioItem, eStatusInventarioItem byInventarioStatus)
        {
            var dbAsync = new BaseOperations();
            try
            {
                await dbAsync.Connection.QueryAsync<InventarioItemVO>("UPDATE InventarioItemVO SET statusIntegracao  = ?, " +
                                                                                                   "quantidade       = ?, " +
                                                                                                   "codigoBarras     = ? " +
                                                                       "WHERE inventarioItemId                       = ? ",
                                                                       byInventarioStatus,
                                                                       byInventarioItem.Quantidade,
                                                                       byInventarioItem.CodigoBarras,
                                                                       byInventarioItem.InventarioItemId);

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

        public static List<InventarioItemVO> GetInventarioItemByItemCx(int byInventarioId, string byCodItem)
        {
            var dbAsync = new BaseOperations();
            try
            {
                return dbAsync.Connection.Table<InventarioItemVO>().Where(p => p.InventarioId == byInventarioId
                && p.CodItem != byCodItem 
                && p.StatusIntegracao == eStatusInventarioItem.IntegracaoCX).ToListAsync().Result;
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


        public static List<InventarioItemVO> GetInventarioItemByConfirmadoCx(int byInventarioId)
        {
            var dbAsync = new BaseOperations();
            try
            {
                return dbAsync.Connection.Table<InventarioItemVO>().Where(p => p.InventarioId == byInventarioId
                && p.StatusIntegracao == eStatusInventarioItem.IntegracaoCX).ToListAsync().Result;
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

        public static List<InventarioItemVO> GetInventarioItemByInventarioLocalizacao(int byInventarioId, string byLocalizacao)
        {
            var dbAsync = new BaseOperations();
            try
            {
                return dbAsync.Connection.Table<InventarioItemVO>().Where(p => p.InventarioId == byInventarioId && p.Localizacao == byLocalizacao).ToListAsync().Result;
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

        //   public static List<InventarioItemVO> GetInventarioItemByInventario(int inventarioById)
        //   {
        //       List<InventarioItemVO> lstInventarioItem = new List<InventarioItemVO>();
        //       var dbAsync = new BaseOperations();
        //       try
        //       {
        //           //lstInventarioItem = dbAsync.Connection.QueryAsync<InventarioItemVO>("SELECT * FROM InventarioItemVO WHERE inventarioId = ?", inventarioById).Result;
        //
        //           // lstInventarioItem = dbAsync.Connection.Table<InventarioItemVO>().Where(p => p.InventarioId == inventarioById).ToListAsync().Result;
        //           lstInventarioItem = dbAsync.Connection.Table<InventarioItemVO>().ToListAsync().Result;
        //           return lstInventarioItem;
        //
        //       }
        //       catch (SQLiteException ex)
        //       {
        //           throw ex;
        //       }
        //       catch (Exception ex)
        //       {
        //           throw ex;
        //       }
        //       finally
        //       {
        //           //dbAsync.Connection.CloseAsync();
        //       }
        //   }
        //
        //   public async static Task<List<InventarioItemVO>> GetInventarioItemByInventarioAsync(int inventarioById)
        //   {
        //       List<InventarioItemVO> lstInventarioItem = new List<InventarioItemVO>();
        //       var dbAsync = new BaseOperations();
        //       try
        //       {
        //           lstInventarioItem = await dbAsync.Connection.QueryAsync<InventarioItemVO>("SELECT * FROM InventarioItemVO WHERE inventarioId = ?", inventarioById);
        //           return lstInventarioItem;
        //
        //       }
        //       catch (SQLiteException ex)
        //       {
        //           throw ex;
        //       }
        //       catch (Exception ex)
        //       {
        //           throw ex;
        //       }
        //       finally
        //       {
        //           //dbAsync.Connection.CloseAsync();
        //       }
        //   }
        //
        //   public static List<InventarioItemVO> GetInventarioItemDigitadoByInventarioId(int inventarioById)
        //   {
        //       List<InventarioItemVO> lstInventarioItem = new List<InventarioItemVO>();
        //       var dbAsync = new BaseOperations();
        //       try
        //       {
        //           lstInventarioItem = dbAsync.Connection.QueryAsync<InventarioItemVO>("SELECT * FROM InventarioItemVO WHERE inventarioId = ? " +
        //                                                                                              "AND qtdDigitada"
        //                                                                                               , inventarioById).Result;
        //
        //
        //
        //           return lstInventarioItem;
        //
        //       }
        //       catch (SQLiteException ex)
        //       {
        //           throw ex;
        //       }
        //       catch (Exception ex)
        //       {
        //           throw ex;
        //       }
        //       finally
        //       {
        //           //dbAsync.Connection.CloseAsync();
        //       }
        //   }
        //
        //   public static InventarioItemVO GetInventarioItem(int byInventarioItemId)
        //   {
        //       var dbAsync = new BaseOperations();
        //       try
        //       {
        //           var inventario = dbAsync.Connection.Table<InventarioItemVO>().Where(p => p.InventarioItemId == byInventarioItemId).FirstOrDefaultAsync();
        //
        //           return inventario.Result;
        //       }
        //       catch (SQLiteException ex)
        //       {
        //           throw ex;
        //       }
        //       catch (Exception ex)
        //       {
        //           throw ex;
        //       }
        //       finally
        //       {
        //           //dbAsync.Connection.CloseAsync();
        //       }
        //   }
        //
        //   public static InventarioItemVO InserirInventarioItem(InventarioItemVO byInventarioItem)
        //   {
        //
        //       var dbAsync = new BaseOperations();
        //       try
        //       {
        //           _ = dbAsync.InsertAsync(byInventarioItem);
        //
        //           var inventarioItem = dbAsync.Connection.Table<InventarioItemVO>().ToListAsync().Result.LastOrDefault();
        //
        //           return inventarioItem;
        //
        //       }
        //       catch (SQLiteException ex)
        //       {
        //           throw ex;
        //       }
        //       catch (Exception ex)
        //       {
        //           throw ex;
        //       }
        //       finally
        //       {
        //           //dbAsync.Connection.CloseAsync();
        //       }
        //   }
        //
        //   public static bool ConfirmaQuantidadeDigitada(int byInventarioItemId, bool byQtdDigitada)
        //   {
        //       var dbAsync = new BaseOperations();
        //       try
        //       {
        //           dbAsync.Connection.QueryAsync<InventarioItemVO>("update InventarioItemVO set   qtdDigitada      = ?" +
        //                                                                                  " where inventarioItemId = ? ",
        //                                                                                   byQtdDigitada,
        //                                                                                   byInventarioItemId);
        //
        //           return true;
        //       }
        //       catch (SQLiteException ex)
        //       {
        //           throw ex;
        //       }
        //       catch (Exception ex)
        //       {
        //           throw ex;
        //       }
        //       finally
        //       {
        //           //dbAsync.Connection.CloseAsync();
        //       }
        //   }
        //
        //   public static bool AtualizaQuantidadeInventario(InventarioItemVO inventarioItem)
        //   {
        //       var dbAsync = new BaseOperations();
        //       try
        //       {
        //
        //           dbAsync.Connection.QueryAsync<InventarioItemVO>("update InventarioItemVO set   valApurado         = ?" +
        //                                                                         " where inventarioItemId = ? ",
        //                                                                         inventarioItem.ValApurado,
        //                                                                         inventarioItem.InventarioItemId);
        //
        //           return true;
        //       }
        //       catch (SQLiteException ex)
        //       {
        //           throw ex;
        //       }
        //       catch (Exception ex)
        //       {
        //           throw ex;
        //       }
        //       finally
        //       {
        //           //dbAsync.Connection.CloseAsync();
        //       }
        //   }
        //
        //   public static List<InventarioItemVO> GetInventarioItemByQr(int inventarioById, string byItCodigo, string byCodLote)
        //   {
        //       List<InventarioItemVO> lstInventarioItem = new List<InventarioItemVO>();
        //       var dbAsync = new BaseOperations();
        //       try
        //       {
        //
        //           lstInventarioItem = dbAsync.Connection.QueryAsync<InventarioItemVO>("SELECT * FROM InventarioItemVO WHERE inventarioId = ? " +
        //                                                                                              "AND itCodigo     = ? " +
        //                                                                                              "AND codLote      = ? ", inventarioById, byItCodigo, byCodLote).Result;
        //
        //
        //           return lstInventarioItem;
        //
        //       }
        //       catch (SQLiteException ex)
        //       {
        //           throw ex;
        //       }
        //       catch (Exception ex)
        //       {
        //           throw ex;
        //       }
        //       finally
        //       {
        //           //dbAsync.Connection.CloseAsync();
        //       }
        //   }
        //
        //
        //   public async static Task<bool> DeletarInventarioByInventarioId(int byInventarioId)
        //   {
        //       var dbAsync = new BaseOperations();
        //       try
        //       {
        //           await dbAsync.Connection.QueryAsync<InventarioItemVO>("delete from InventarioItemVO where inventarioId = ?", byInventarioId);
        //           return true;
        //       }
        //       catch (SQLiteException ex)
        //       {
        //           throw ex;
        //       }
        //       catch (Exception ex)
        //       {
        //           throw ex;
        //       }
        //       finally
        //       {
        //           //dbAsync.Connection.CloseAsync();
        //       }
        //   }
        //
        //   public async static Task<bool> AtualizarInventarioItem(List<InventarioItemVO> byLstInventarioItem)
        //   {
        //       var dbAsync = new BaseOperations();
        //       try
        //       {
        //           /*int intCount = GetCountInventarioItem();
        //
        //
        //           if (intCount <= 0)
        //           {
        //               await dbAsync.InsertAllAsync(byLstInventarioItem);
        //               return true;
        //           }
        //           else
        //           {*/
        //
        //           for (int i = 0; i < byLstInventarioItem.Count; i++)
        //           {
        //               var inventarioItemVO = await
        //                   dbAsync.Connection.QueryAsync<InventarioItemVO>(
        //                   "select * from InventarioItemVO where inventarioId = ? " +
        //                                                    " and codLocaliz   = ? " +
        //                                                    " and codLote      = ? " +
        //                                                    " and codRefer     = ? " +
        //                                                    " and itCodigo     = ? ",
        //                                                byLstInventarioItem[i].InventarioId,
        //                                                byLstInventarioItem[i].CodLocaliz,
        //                                                byLstInventarioItem[i].CodLote,
        //                                                byLstInventarioItem[i].CodRefer,
        //                                                byLstInventarioItem[i].ItCodigo);
        //
        //               if (inventarioItemVO != null && inventarioItemVO.Count > 0)
        //               {
        //                   if (inventarioItemVO[0].NrFicha != byLstInventarioItem[i].NrFicha)
        //                   {
        //                       inventarioItemVO[0].NrFicha = byLstInventarioItem[i].NrFicha;
        //                       await dbAsync.UpdateAsync(inventarioItemVO[0]);
        //                   }
        //               }
        //               else
        //               {
        //                   await dbAsync.InsertAsync(byLstInventarioItem[i]);
        //               }
        //           }
        //
        //           return true;
        //           //}
        //       }
        //       catch (SQLiteException ex)
        //       {
        //           throw ex;
        //       }
        //       catch (Exception ex)
        //       {
        //           throw ex;
        //       }
        //       finally
        //       {
        //           //dbAsync.Connection.CloseAsync();
        //       }
        //   }
        //
        //   public static int GetCountInventarioItem()
        //   {
        //       var dbAsync = new BaseOperations();
        //       try
        //       {
        //           int intCount = dbAsync.Connection.Table<InventarioItemVO>().CountAsync().Result;
        //
        //           return intCount;
        //       }
        //       catch (SQLiteException ex)
        //       {
        //           throw ex;
        //       }
        //       catch (Exception ex)
        //       {
        //           throw ex;
        //       }
        //       finally
        //       {
        //           //dbAsync.Connection.CloseAsync();
        //       }
        //   }

        public static bool DeletarInventarioByInventarioId(InventarioItemVO byInventarioItem)
        {
            var dbAsync = new BaseOperations();
            try
            {
                //await dbAsync.Connection.QueryAsync<InventarioItemVO>("delete from InventarioItemVO where inventarioId = ?", byInventarioId);
                //dbAsync.DeleteAsync(inventarioItem);
                dbAsync.Connection.QueryAsync<InventarioItemVO>("delete from InventarioItemVO where inventarioItemId = ? ",
                                                                              byInventarioItem.InventarioItemId);
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
    }
}
