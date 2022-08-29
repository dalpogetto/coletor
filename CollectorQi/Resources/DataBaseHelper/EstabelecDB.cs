using System;
using System.Collections.Generic;
using System.Text;
using CollectorQi.VO;
using SQLite;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace CollectorQi.Resources.DataBaseHelper
{

    public static class EstabelecDB
    {       
        public static EstabelecVO GetEstabelec(string byCodEstabel)
        {
            var dbAsync = new BaseOperations();
            try
            {
                var estabelec = dbAsync.Connection.Table<EstabelecVO>().Where(p => p.CodEstabel == byCodEstabel).FirstOrDefaultAsync();

                return estabelec.Result;
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

        public async static Task<List<EstabelecVO>> GetEstabelec()
        {
            var dbAsync = new BaseOperations();
            try
            {
                var estabelec = dbAsync.Connection.Table<EstabelecVO>().ToListAsync();

                return await estabelec;
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
