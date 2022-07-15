﻿using CollectorQi.VO;
using SQLite;
using System;
using System.Linq;

namespace CollectorQi.Resources.DataBaseHelper
{
    public static class FichasUsuarioDB
    {
        public static FichasUsuarioVO InserirFichasUsuarioItem(FichasUsuarioVO byFichasUsuarioItem)
        {
            var dbAsync = new BaseOperations();
            try
            {
                _ = dbAsync.InsertAsync(byFichasUsuarioItem);
                var inventarioItem = dbAsync.Connection.Table<FichasUsuarioVO>().ToListAsync().Result.LastOrDefault();
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
    }
}
