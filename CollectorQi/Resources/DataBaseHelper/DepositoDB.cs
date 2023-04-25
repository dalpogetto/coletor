using System;
using System.Collections.Generic;
using CollectorQi.VO;
using SQLite;

namespace CollectorQi.Resources.DataBaseHelper
{
    public static class DepositoDB
    {
        /*
        private DataBase db;

        public DepositoDB()S
        {
            db = new DataBase();
        }*/

        public static DepositoVO GetDeposito(string byCodDepos)
        {
            DepositoVO deposito = null;
            var dbAsync = new BaseOperations();
            try
            {
                deposito = dbAsync.Connection.Table<DepositoVO>().Where(p => p.CodDepos == byCodDepos).FirstOrDefaultAsync().Result;
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

            return deposito;
        }

        public static List<DepositoVO> GetDeposito()
        {
            List<DepositoVO> result = new List<DepositoVO>();
            var dbAsync = new BaseOperations();
            try
            {
                return dbAsync.Connection.Table<DepositoVO>().OrderBy(i => i.CodDepos).ToListAsync().Result;
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


        public static bool DeletarDeposito(DepositoVO byDeposito)
        {
            var dbAsync = new BaseOperations();
            try
            {

                dbAsync.DeleteAsync(byDeposito);
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

        public static bool InserirDeposito(DepositoVO byDeposito)
        {
            var dbAsync = new BaseOperations();
            try
            {

                dbAsync.InsertAsync(byDeposito);
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
