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
    public static class InventarioDB
    {

        public async static void AtualizaInventarioByCodEstabel(string byCodEstabel, List<InventarioVO> byLstInventarioLocalizacao)
        {
            var dbAsync = new BaseOperations();
            try
            {
                await dbAsync.Connection.Table<InventarioVO>().DeleteAsync(p => p.CodEstabel == byCodEstabel);
                await dbAsync.InsertAllAsync(byLstInventarioLocalizacao);
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

        public static List<InventarioVO> GetInventarioByCodEstabel(string byCodEstabel)
        {
            var dbAsync = new BaseOperations();
            try
            {
                return dbAsync.Connection.Table<InventarioVO>().Where(p => p.CodEstabel == byCodEstabel).ToListAsync().Result;
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

        public async static Task<InventarioVO> GetInventario(int byIdInventario)
        {
            var dbAsync = new BaseOperations();
            try
            {
                return await dbAsync.Connection.Table<InventarioVO>().Where(p => p.IdInventario == byIdInventario).FirstOrDefaultAsync();
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


        //  public async static Task<InventarioVO> GetInventario(int byInventarioId)
        //  {
        //      var dbAsync = new BaseOperations();
        //      try
        //      {
        //          var inventario = dbAsync.Connection.Table<InventarioVO>().Where(p => p.InventarioId == byInventarioId).FirstOrDefaultAsync().Result;
        //
        //          return inventario;
        //      }
        //      catch (SQLiteException ex)
        //      {
        //          throw ex;
        //      }
        //      catch (Exception ex)
        //      {
        //          throw ex;
        //      }
        //      finally
        //      {
        //          //dbAsync.Connection.CloseAsync();
        //      }
        //  }

        //  public static void InserirInventario(InventarioVO byInventario)
        //  {
        //      var dbAsync = new BaseOperations();
        //      try
        //      {
        //          dbAsync.InsertAsync(byInventario);
        //      }
        //      catch (SQLiteException ex)
        //      {
        //          throw ex;
        //      }
        //      catch (Exception ex)
        //      {
        //          throw ex;
        //      }
        //      finally
        //      {
        //          //dbAsync.Connection.CloseAsync();
        //      }
        //  }

        //  public static InventarioVO EfetivaInventarioMobile(int byInventarioId, eStatusInventario byStatusInventario)
        //  {
        //      var dbAsync = new BaseOperations();
        //      try
        //      {
        //          dbAsync.Connection.QueryAsync<InventarioVO>("update InventarioVO set statusInventario  = ? " +
        //                                                         "where inventarioId      = ? ",
        //                                                          byStatusInventario,
        //                                                          byInventarioId);
        //
        //          var inventario = dbAsync.Connection.Table<InventarioVO>().Where(p => p.InventarioId == byInventarioId).FirstOrDefaultAsync().Result;
        //
        //          return inventario;
        //      }
        //      catch (SQLiteException ex)
        //      {
        //          throw ex;
        //      }
        //      catch (Exception ex)
        //      {
        //          throw ex;
        //      }
        //      finally
        //      {
        //          //dbAsync.Connection.CloseAsync();
        //      }
        //  }

        // public static List<InventarioVO> GetInventarioByEstab(string byCodEstabel)
        // {
        //     var dbAsync = new BaseOperations();
        //     try
        //     {
        //         return dbAsync.Connection.QueryAsync<InventarioVO>("SELECT * FROM InventarioVO where codEstabel = ? and inventarioAtivo = 1", byCodEstabel).Result;
        //     }
        //     catch (SQLiteException ex)
        //     {
        //         throw ex;
        //     }
        //     catch (Exception ex)
        //     {
        //         throw ex;
        //     }
        //     finally
        //     {
        //         //dbAsync.Connection.CloseAsync();
        //     }
        // }

        // public static List<InventarioVO> GetInventarioAtivoByEstab(string byCodEstabel)
        // {
        //     var dbAsync = new BaseOperations();
        //     try
        //     {
        //         return dbAsync.Connection.QueryAsync<InventarioVO>("SELECT * FROM InventarioVO where codEstabel = ? and inventarioAtivo = ?", byCodEstabel, true).Result;
        //     }
        //     catch (SQLiteException ex)
        //     {
        //         throw ex;
        //     }
        //     catch (Exception ex)
        //     {
        //         throw ex;
        //     }
        //     finally
        //     {
        //         //dbAsync.Connection.CloseAsync();
        //     }
        // }


        // public async static Task<int> AtualizaInventarioGetId(InventarioVO byInventarioVO)
        // {
        //     var dbAsync = new BaseOperations();
        //     try
        //     {
        //         var inventarioVO =
        //             await dbAsync.Connection.QueryAsync<InventarioVO>
        //             ("select * from InventarioVO where codEstabel   = ? " +
        //                                          " and codDepos     = ? " +
        //                                          " and dtInventario = ? " +
        //                                          " and contagem     = ? " +
        //                                          " and descEstabel  = ? " +
        //                                          " and descDepos    = ? ",
        //                 byInventarioVO.CodEstabel,
        //                 byInventarioVO.CodDepos,
        //                 byInventarioVO.DtInventario,
        //                 byInventarioVO.Contagem,
        //                 byInventarioVO.DescEstabel,
        //                 byInventarioVO.DescDepos);
        //
        //         if (inventarioVO != null && inventarioVO.Count > 0)
        //         {
        //             /* Victor Alves - 26/11/2019 - Se for diferente ativo, então passa para ativo */
        //             if (!inventarioVO[0].inventarioAtivo)
        //             {
        //                 inventarioVO[0].inventarioAtivo = true;
        //
        //                 await dbAsync.UpdateAsync(inventarioVO[0]);
        //             }
        //             return inventarioVO[0].InventarioId;
        //         }
        //         else
        //         {
        //
        //             await dbAsync.InsertAsync(byInventarioVO);
        //             var inventario = dbAsync.Connection.Table<InventarioVO>().ToListAsync().Result.LastOrDefault();
        //
        //             return inventario.InventarioId;
        //         }
        //     }
        //     catch (SQLiteException ex)
        //     {
        //         throw ex;
        //     }
        //     catch (Exception ex)
        //     {
        //         throw ex;
        //     }
        //     finally
        //     {
        //         //dbAsync.Connection.CloseAsync();
        //     }
        // }
        //
        // public async static Task DesativaInventarioVO(InventarioVO byInventarioVO)
        // {
        //     var dbAsync = new BaseOperations();
        //     try
        //     {
        //         var inventarioVO =
        //             await dbAsync.Connection.QueryAsync<InventarioVO>
        //             ("select * from InventarioVO where codEstabel   = ? " +
        //                                          " and codDepos     = ? " +
        //                                          " and dtInventario = ? " +
        //                                          " and contagem     = ? ",
        //                 byInventarioVO.CodEstabel,
        //                 byInventarioVO.CodDepos,
        //                 byInventarioVO.DtInventario,
        //                 byInventarioVO.Contagem
        //                 );
        //
        //         if (inventarioVO != null && inventarioVO.Count > 0)
        //         {
        //             /* Victor Alves - 26/11/2019 - Se estiver ativo, então desativa */
        //             if (inventarioVO[0].inventarioAtivo)
        //             {
        //                 inventarioVO[0].inventarioAtivo = false;
        //
        //                 await dbAsync.UpdateAsync(inventarioVO[0]);
        //             }
        //         }
        //     }
        //     catch (SQLiteException ex)
        //     {
        //         throw ex;
        //     }
        //     catch (Exception ex)
        //     {
        //         throw ex;
        //     }
        //     finally
        //     {
        //         //dbAsync.Connection.CloseAsync();
        //     }
        // }
        //
        // public static List<InventarioVO> GetInventario()
        // {
        //     //InventarioVO inventario = new InventarioVO();
        //
        //     var dbAsync = new BaseOperations();
        //     try
        //     {
        //         var inventario = dbAsync.Connection.Table<InventarioVO>().ToListAsync().Result;
        //
        //         return inventario;
        //     }
        //     catch (SQLiteException ex)
        //     {
        //         throw ex;
        //     }
        //     catch (Exception ex)
        //     {
        //         throw ex;
        //     }
        //     finally
        //     {
        //         //dbAsync.Connection.CloseAsync();
        //     }
        // }
        //
        // public async static Task<bool> DeletarInventario(InventarioVO byInventarioVO)
        // {
        //     var dbAsync = new BaseOperations();
        //     try
        //     {
        //         await dbAsync.DeleteAsync(byInventarioVO);
        //         return true;
        //     }
        //     catch (SQLiteException ex)
        //     {
        //         throw ex;
        //     }
        //     catch (Exception ex)
        //     {
        //         throw ex;
        //     }
        //     finally
        //     {
        //         //dbAsync.Connection.CloseAsync();
        //     }
        // }
    }
}
