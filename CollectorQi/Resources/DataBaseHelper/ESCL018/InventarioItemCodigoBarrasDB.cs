using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollectorQi.VO.ESCL018;

namespace CollectorQi.Resources.DataBaseHelper.ESCL018
{
    public static class InventarioItemCodigoBarrasDB
    {
        public async static Task<InventarioItemCodigoBarrasVO> InserirEtiqueta(InventarioItemCodigoBarrasVO byInventarioItem)
        {
        
            var dbAsync = new BaseOperations();
            try
            {
                await dbAsync.InsertAsync(byInventarioItem);
        
                var inventarioItem = dbAsync.Connection.Table<InventarioItemCodigoBarrasVO>().ToListAsync().Result.LastOrDefault();
        
                return inventarioItem;
        
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

        public static List<InventarioItemCodigoBarrasVO> GetInventarioItemById(string byInventarioKey)
        {
            var dbAsync = new BaseOperations();
            try
            {
                return dbAsync.Connection.Table<InventarioItemCodigoBarrasVO>().Where(p => p.InventarioItemKey == byInventarioKey).ToListAsync().Result;
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

        public static List<InventarioItemCodigoBarrasVO> GetByInventarioItemKey(string byInventarioItemKey)
        {
            var dbAsync = new BaseOperations();
            try
            {
                return dbAsync.Connection.Table<InventarioItemCodigoBarrasVO>().Where(p => p.InventarioItemKey == byInventarioItemKey).ToListAsync().Result;
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

        public static List<InventarioItemCodigoBarrasVO> GetByInventarioId(int byInventarioId)
        {
            var dbAsync = new BaseOperations();
            try
            {
                return dbAsync.Connection.Table<InventarioItemCodigoBarrasVO>().Where(p => p.__inventarioItem__.InventarioId == byInventarioId).ToListAsync().Result;
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

        public static bool DeleteCodigoBarras(int idInventarioCodigoBarras)
        {
            var dbAsync = new BaseOperations();
            try
            {
                //await dbAsync.Connection.QueryAsync<InventarioItemVO>("delete from InventarioItemVO where inventarioId = ?", byInventarioId);
                //dbAsync.DeleteAsync(inventarioItem);
                dbAsync.Connection.DeleteAsync<InventarioItemCodigoBarrasVO>(idInventarioCodigoBarras);
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

        public static bool DeleteByItemId(string byInventarioItemKey)
        {
            var dbAsync = new BaseOperations();
            try
            {
                dbAsync.Connection.QueryAsync<InventarioItemCodigoBarrasVO>("delete from InventarioItemCodigoBarrasVO where inventarioItemKey = ?", byInventarioItemKey);
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

        public static bool GetByCodigoBarras(int inventarioId, string byCodigoBarras)
        {
            var dbAsync = new BaseOperations();
            try
            {
                var result = dbAsync.Connection.Table<InventarioItemCodigoBarrasVO>().FirstOrDefaultAsync(x => x.CodigoBarras == byCodigoBarras);

                if (result != null)
                {
                    if (result.Result != null)
                    {
                        return true;
                    }
                }

                return false;
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
        */
    }
}
