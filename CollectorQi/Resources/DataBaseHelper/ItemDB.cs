using System;
using System.Collections.Generic;
using CollectorQi.VO;
using SQLite;
using System.Threading.Tasks;

namespace CollectorQi.Resources.DataBaseHelper
{
    public  static class ItemDB
    {
        public static int GetMaxVersaoItem()
        {
            var dbAsync = new BaseOperations();
            try
            {
                //throw new Exception("1.1");

                int intVersaoInteg = 0;

                try
                {
                    var result = dbAsync.Connection.Table<ItemVO>().OrderByDescending(p => p.VersaoIntegracao).FirstOrDefaultAsync().Result;

                    if (result != null)
                        intVersaoInteg = result.VersaoIntegracao;

                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return intVersaoInteg;

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

        public static async Task<bool> AtualizarItemAsync(List<ItemVO> byLstItem)
        {

            var dbAsync = new BaseOperations();

            try
            {
                int intCount = await dbAsync.Connection.Table<ItemVO>().CountAsync();

                if (intCount <= 0)
                {
                    await dbAsync.InsertAllAsync(byLstItem);
                }
                else
                {
                    for (int i = 0; i < byLstItem.Count; i++)
                    {
                        List<ItemVO> itemVO = null;

                        try
                        {
                            itemVO = await dbAsync.Connection.QueryAsync<ItemVO>("SELECT * FROM ItemVO WHERE itCodigo = ? ", byLstItem[i].ItCodigo);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                        if (itemVO != null && itemVO.Count > 0)
                        {
                            await dbAsync.Connection.QueryAsync<ItemVO>("UPDATE ItemVO set descItem        =?, " +
                                                                        "depositoPad         =?, " +
                                                                        "codLocaliz          =?, " +
                                                                        "locUnica            =?, " +
                                                                        "tipoConEst          =?, " +
                                                                        "contrQualid         =?, " +
                                                                        "logOrigExt          =?, " +
                                                                        "fraciona            =?, " +
                                                                        "un                  =?, " +
                                                                        "versaoIntegracao    =?, " +
                                                                        "produtoNaoEncontrado=?, " +
                                                                        "logDataItem         =CURRENT_TIMESTAMP " +
                                                                        "Where ItCodigo      =?",
                                                                       byLstItem[i].DescItem,
                                                                       byLstItem[i].DepositoPad,
                                                                       byLstItem[i].CodLocaliz,
                                                                       byLstItem[i].LocUnica,
                                                                       byLstItem[i].TipoConEst,
                                                                       byLstItem[i].ContrQualid,
                                                                       byLstItem[i].LogOrigExt,
                                                                       byLstItem[i].Fraciona,
                                                                       byLstItem[i].Un,
                                                                       byLstItem[i].VersaoIntegracao,
                                                                       "S",
                                                                       //         byItem.LogDataItem,
                                                                       byLstItem[i].ItCodigo);
                        }
                        else
                        {
                            await dbAsync.InsertAsync(byLstItem[i]);
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

        public static async Task<bool> DeleteItemAsync(string byItCodigo)
        {
            var dbAsync = new BaseOperations();

            try
            {
                var item = await dbAsync.Connection.Table<ItemVO>().Where(p => p.ItCodigo == byItCodigo).FirstOrDefaultAsync();

                await dbAsync.DeleteAsync<ItemVO>(item);

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

       public static Task<List<ItemVO>> GetItemsAsync()
        {
            var dbAsync = new BaseOperations();

            try
            {
                return dbAsync.Connection.Table<ItemVO>().OrderBy(i => i.ItCodigo).ToListAsync();
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

        public static ItemVO GetItem(string byItCodigo)
        {
            var dbAsync = new BaseOperations();
            try
            {
                return dbAsync.Connection.Table<ItemVO>().FirstOrDefaultAsync(ItemVO => ItemVO.ItCodigo == byItCodigo).Result;
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



        public static int GetItemCount()
        {
            var dbAsync = new BaseOperations();

            try
            {
                var intCount = dbAsync.Connection.Table<ItemVO>().CountAsync();

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
