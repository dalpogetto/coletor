using CollectorQi.VO;
using SQLite;
using System;
using System.Collections.Generic;
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

        public static List<NotaFiscalVO> GetNotaFiscalByEstab(string byCodEstabel)
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

        public static NotaFiscalVO AtualizaNotaFiscal(NotaFiscalVO notaFiscalVO)
        {
            var dbAsync = new BaseOperations();
            try
            {
                dbAsync.Connection.QueryAsync<NotaFiscalVO>("update NotaFiscalVO set conferido  = ? " +
                                                            "where numRR      = ? ",
                                                             notaFiscalVO.Conferido,
                                                             notaFiscalVO.NumRR);

                var notaFiscal = dbAsync.Connection.Table<NotaFiscalVO>().FirstOrDefaultAsync().Result;

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
    }
}
