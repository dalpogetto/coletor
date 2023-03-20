using System;
using System.Collections.Generic;
using CollectorQi.VO;
using SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace CollectorQi.Resources.DataBaseHelper
{

    public static class SaldoEstoqDB
    {
        public static List<SaldoEstoqVO> GetSaldoEstoqByItemAndEstab(string pItcodigo, string pCodEstabel)
        {
            List<SaldoEstoqVO> lstSaldoEstoq = new List<SaldoEstoqVO>();
            var dbAsync = new BaseOperations();
            try
            {
                lstSaldoEstoq = dbAsync.Connection.QueryAsync<SaldoEstoqVO>("select * from SaldoEstoqVO where codEstabel = ? and itCodigo = ? and (qtidadeAtu <> 0 or qtidadeMobile <> 0) ", pCodEstabel, pItcodigo).Result.ToList();

                return lstSaldoEstoq;
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

        public static Task<List<SaldoEstoqVO>> GetSaldoEstoqByItemAndEstabAsync(string pItcodigo, string pCodEstabel)
        {
            var dbAsync = new BaseOperations();
            try
            {
                return dbAsync.Connection.QueryAsync<SaldoEstoqVO>("select * from SaldoEstoqVO where codEstabel = ? and itCodigo = ? and (qtidadeAtu <> 0 or qtidadeMobile <> 0) ", pCodEstabel, pItcodigo);
            }
            catch (SQLiteException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int GetMaxVersaoNrTrans()
        {
            var dbAsync = new BaseOperations();
            try
            {
                //DateTime dtVersao = DateTime.MinValue;

                try
                {
                    //dtVersao = dbAsync.Connection.Table<SaldoEstoqVO>().OrderByDescending(p => p.DtVersaoSaldo).FirstOrDefaultAsync().Result.DtVersaoSaldo;
                    //var sdo = dbAsync.QueryAsync(new TableMapping(typeof(SaldoEstoqVO)), "SELECT MAX(dtVersaoSaldo) FROM SaldoEstoqVO ");

                    //  var result = dbAsync.Connection.Table<SaldoEstoqVO>().OrderByDescending(p => p.DtVersaoSaldo).FirstOrDefaultAsync().Result;


                    var result = dbAsync.Connection.Table<SaldoEstoqVO>().OrderByDescending(p => p.NrTransVersao).FirstOrDefaultAsync().Result;
                    if (result != null)
                        return result.NrTransVersao;

                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return 0;
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


        public static SaldoEstoqVO GetSaldoByKey(string byCodEstabel, string byItCodigo, string byCodDepos, string byCodRefer, string byCodLocaliz, string byCodLote)
        {
            var dbAsync = new BaseOperations();
            try
            {

                return dbAsync.Connection.QueryAsync<SaldoEstoqVO>("select * from SaldoEstoqVO where codEstabel = ? " +
                                                                                    "and itCodigo   = ? " +
                                                                                    "and codDepos   = ? " +
                                                                                    "and codRefer   = ? " +
                                                                                    "and codLocaliz = ? " +
                                                                                    "and codLote    = ?  ", byCodEstabel, byItCodigo, byCodDepos, byCodRefer, byCodLocaliz, byCodLote).Result.FirstOrDefault();


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

        public static async Task<bool> AtualizarSaldoEstoq(List<SaldoEstoqVO> byLstSaldoEstoq)
        {
            BaseOperations dbAsync = new BaseOperations();

            try
            {
                int intCount = GetSaldoEstoqCount();

                if (intCount <= 0)
                {
                    await dbAsync.InsertAllAsync(byLstSaldoEstoq);
                }
                else
                {
                    for (int i = 0; i < byLstSaldoEstoq.Count; i++)
                    {


                        //throw new Exception("Iniciando importação Saldo");

                        var saldoEstoqVO = await
                            dbAsync.Connection.QueryAsync<SaldoEstoqVO>
                            ("select * from SaldoEstoqVO where codEstabel = ? " +
                                                         " and itCodigo   = ? " +
                                                         " and codDepos   = ? " +
                                                         " and codRefer   = ? " +
                                                         " and codLocaliz = ? " +
                                                         " and codLote    = ? ",
                                                         byLstSaldoEstoq[i].CodEstabel,
                                                         byLstSaldoEstoq[i].ItCodigo,
                                                         byLstSaldoEstoq[i].CodDepos,
                                                         byLstSaldoEstoq[i].CodRefer,
                                                         byLstSaldoEstoq[i].CodLocaliz,
                                                         byLstSaldoEstoq[i].CodLote);

                        if (saldoEstoqVO != null && saldoEstoqVO.Count > 0)
                        {
                            await dbAsync.Connection.QueryAsync<SaldoEstoqVO>("update SaldoEstoqVO set  dtValiLote    = ?, " +
                                                                                 " qtidadeAtu    = ?, " +
                                                                                 " nrTransVersao = ?, " +
                                                                                 " qtidadeMobile = ?  " +
                                                                            "where codEstabel = ? " +
                                                                             " and itCodigo   = ? " +
                                                                             " and codDepos   = ? " +
                                                                             " and codRefer   = ? " +
                                                                             " and codLocaliz = ? " +
                                                                             " and codLote    = ? ",
                                                                              byLstSaldoEstoq[i].DtValiLote,
                                                                              byLstSaldoEstoq[i].QtidadeAtu,
                                                                              byLstSaldoEstoq[i].NrTransVersao,
                                                                              byLstSaldoEstoq[i].QtidadeMobile,
                                                                              byLstSaldoEstoq[i].CodEstabel,
                                                                              byLstSaldoEstoq[i].ItCodigo,
                                                                              byLstSaldoEstoq[i].CodDepos,
                                                                              byLstSaldoEstoq[i].CodRefer,
                                                                              byLstSaldoEstoq[i].CodLocaliz,
                                                                              byLstSaldoEstoq[i].CodLote);
                        }
                        else
                        {
                            await dbAsync.InsertAsync(byLstSaldoEstoq[i]);
                        }
                    }
                }
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

        public static int GetSaldoEstoqCount()
        {

            var dbAsync = new BaseOperations();

            try
            {
                var intCount = dbAsync.Connection.Table<SaldoEstoqVO>().CountAsync();

                return intCount.Result;
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
