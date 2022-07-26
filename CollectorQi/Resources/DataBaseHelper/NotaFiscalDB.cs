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
    public static class NotaFiscalDB
    {

        public async static Task<NotaFiscalVO> GetNotaFiscal(string byNotaFiscalId)
        {
            var dbAsync = new BaseOperations();
            try
            {
                var notaFiscal = dbAsync.Connection.Table<NotaFiscalVO>().Where(p => p.RowId == byNotaFiscalId).FirstOrDefaultAsync().Result;

                return notaFiscal;
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

        public async static void InserirNotaFiscal(NotaFiscalVO byNotaFiscal)
        {
            var dbAsync = new BaseOperations();
            try
            {
                _ = dbAsync.InsertAsync(byNotaFiscal);
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

        public static List<NotaFiscalVO> GetNotaFiscalAtivoByEstab(string byCodEstabel)
        {
            var dbAsync = new BaseOperations();
            try
            {
                return dbAsync.Connection.QueryAsync<NotaFiscalVO>("SELECT * FROM NotaFiscalVO where codEstabel = ? ", byCodEstabel).Result;
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

        //public static InventarioVO EfetivaInventarioMobile(int byInventarioId, eStatusInventario byStatusInventario)
        //{
        //    var dbAsync = new BaseOperations();
        //    try
        //    {
        //        dbAsync.Connection.QueryAsync<InventarioVO>("update InventarioVO set statusInventario  = ? " +
        //                                                       "where inventarioId      = ? ",
        //                                                        byStatusInventario,
        //                                                        byInventarioId);

        //        var inventario = dbAsync.Connection.Table<InventarioVO>().Where(p => p.InventarioId == byInventarioId).FirstOrDefaultAsync().Result;

        //        return inventario;
        //    }
        //    catch (SQLiteException ex)
        //    {
        //        throw ex;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        //dbAsync.Connection.CloseAsync();
        //    }
        //}

        //public static List<InventarioVO> GetInventarioByEstab(string byCodEstabel)
        //{
        //    var dbAsync = new BaseOperations();
        //    try
        //    {
        //        return dbAsync.Connection.QueryAsync<InventarioVO>("SELECT * FROM InventarioVO where codEstabel = ? and inventarioAtivo = 1", byCodEstabel).Result;
        //    }
        //    catch (SQLiteException ex)
        //    {
        //        throw ex;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        //dbAsync.Connection.CloseAsync();
        //    }
        //}

        //public static List<InventarioVO> GetInventarioAtivoByEstab(string byCodEstabel)
        //{
        //    var dbAsync = new BaseOperations();
        //    try
        //    {
        //        return dbAsync.Connection.QueryAsync<InventarioVO>("SELECT * FROM InventarioVO where codEstabel = ? and inventarioAtivo = ?", byCodEstabel, true).Result;
        //    }
        //    catch (SQLiteException ex)
        //    {
        //        throw ex;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        //dbAsync.Connection.CloseAsync();
        //    }
        //}

        //public async static Task<int> AtualizaInventarioGetId(InventarioVO byInventarioVO)
        //{
        //    var dbAsync = new BaseOperations();
        //    try
        //    {
        //        var inventarioVO =
        //            await dbAsync.Connection.QueryAsync<InventarioVO>
        //            ("select * from InventarioVO where codEstabel   = ? " +
        //                                         " and codDepos     = ? " +
        //                                         " and dtInventario = ? " +
        //                                         " and contagem     = ? " +
        //                                         " and descEstabel  = ? " +
        //                                         " and descDepos    = ? ",
        //                byInventarioVO.CodEstabel,
        //                byInventarioVO.CodDepos,
        //                byInventarioVO.DtInventario,
        //                byInventarioVO.Contagem,
        //                byInventarioVO.DescEstabel,
        //                byInventarioVO.DescDepos);

        //        if (inventarioVO != null && inventarioVO.Count > 0)
        //        {
        //            /* Victor Alves - 26/11/2019 - Se for diferente ativo, então passa para ativo */
        //            if (!inventarioVO[0].inventarioAtivo)
        //            {
        //                inventarioVO[0].inventarioAtivo = true;

        //                await dbAsync.UpdateAsync(inventarioVO[0]);
        //            }
        //            return inventarioVO[0].InventarioId;
        //        }
        //        else
        //        {

        //            await dbAsync.InsertAsync(byInventarioVO);
        //            var inventario = dbAsync.Connection.Table<InventarioVO>().ToListAsync().Result.LastOrDefault();

        //            return inventario.InventarioId;
        //        }
        //    }
        //    catch (SQLiteException ex)
        //    {
        //        throw ex;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        //dbAsync.Connection.CloseAsync();
        //    }
        //}

        //public async static Task DesativaInventarioVO(InventarioVO byInventarioVO)
        //{
        //    var dbAsync = new BaseOperations();
        //    try
        //    {
        //        var inventarioVO =
        //            await dbAsync.Connection.QueryAsync<InventarioVO>
        //            ("select * from InventarioVO where codEstabel   = ? " +
        //                                         " and codDepos     = ? " +
        //                                         " and dtInventario = ? " +
        //                                         " and contagem     = ? ",
        //                byInventarioVO.CodEstabel,
        //                byInventarioVO.CodDepos,
        //                byInventarioVO.DtInventario,
        //                byInventarioVO.Contagem
        //                );

        //        if (inventarioVO != null && inventarioVO.Count > 0)
        //        {
        //            /* Victor Alves - 26/11/2019 - Se estiver ativo, então desativa */
        //            if (inventarioVO[0].inventarioAtivo)
        //            {
        //                inventarioVO[0].inventarioAtivo = false;

        //                await dbAsync.UpdateAsync(inventarioVO[0]);
        //            }
        //        }
        //    }
        //    catch (SQLiteException ex)
        //    {
        //        throw ex;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        //dbAsync.Connection.CloseAsync();
        //    }
        //}

        //public static List<InventarioVO> GetInventario()
        //{
        //    //InventarioVO inventario = new InventarioVO();

        //    var dbAsync = new BaseOperations();
        //    try
        //    {
        //        var inventario = dbAsync.Connection.Table<InventarioVO>().ToListAsync().Result;

        //        return inventario;
        //    }
        //    catch (SQLiteException ex)
        //    {
        //        throw ex;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        //dbAsync.Connection.CloseAsync();
        //    }
        //}

        //public async static Task<bool> DeletarInventario(InventarioVO byInventarioVO)
        //{
        //    var dbAsync = new BaseOperations();
        //    try
        //    {
        //        await dbAsync.DeleteAsync(byInventarioVO);
        //        return true;
        //    }
        //    catch (SQLiteException ex)
        //    {
        //        throw ex;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        //dbAsync.Connection.CloseAsync();
        //    }
        //}
    }
}
