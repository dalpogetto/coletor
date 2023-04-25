using System;
using CollectorQi.VO;
using SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace CollectorQi.Resources.DataBaseHelper
{
    public static class SecurityDB
    {
        public static async Task<SecurityVO> GetSecurityAsync()
        {
            var dbAsync = new BaseOperations();
            try
            {
                //var sec = await dbAsync.Connection.Table<SecurityVO>().FirstOrDefaultAsync();

                //var sec = await dbAsync.Table<SecurityVO>().FirstOrDefaultAsync();
                /* Victor Alves 22/11/2019 - alterado para buscar firstOf após retorno */
                var sec = await dbAsync.QueryAsync<SecurityVO>("SELECT * FROM SecurityVO", null);

                return sec.FirstOrDefault();
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

        public async static Task<bool> AtualizarSecurityLogin(SecurityVO bySecurityVO)
        {

            var dbAsync = new BaseOperations();
            try
            {
                var sec = await dbAsync.QueryAsync<SecurityVO>("SELECT * FROM SecurityVO", null);

                var securityVO = sec.FirstOrDefault();
                
                if (securityVO != null)
                {
                    bySecurityVO.DtUltIntegracao = securityVO.DtUltIntegracao;

                    /*
                    await dbAsync.Connection.QueryAsync<SecurityVO>("UPDATE SecurityVO set codUsuario       = ?, " +
                                                               " codSenha         = ?, " +
                                                               " autenticado      = ?  " +
                                                               " Where securityId = ?  ",
                                                             bySecurityVO.CodUsuario,
                                                             bySecurityVO.CodSenha,
                                                             bySecurityVO.Autenticado,
                                                             securityVO.SecurityId);*/

                    await dbAsync.UpdateAsync(securityVO);
                }
                else
                {
                    await dbAsync.InsertAsync(bySecurityVO);
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

        public async static Task<bool> AtualizarSecurityIntegracao()
        {
            var dbAsync = new BaseOperations();
            try
            {
                var sec = await dbAsync.QueryAsync<SecurityVO>("SELECT * FROM SecurityVO", null);

                var securityVO = sec.FirstOrDefault();

                if (securityVO != null)
                {
                    securityVO.DtUltIntegracao = DateTime.Now;

                    //securityVO.DtUltIntegracao = new DateTime(2019, 04, 28);

                    await dbAsync.UpdateAsync(securityVO);
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


        public async static Task<bool> AtualizarSecurityParametros(bool pCaixaCompleta)
        {
            var dbAsync = new BaseOperations();
            try
            {
                var sec = await dbAsync.QueryAsync<SecurityVO>("SELECT * FROM SecurityVO", null);

                var securityVO = sec.FirstOrDefault();

                if (securityVO != null)
                {
                    securityVO.CxCompleta = pCaixaCompleta;

                    //securityVO.DtUltIntegracao = new DateTime(2019, 04, 28);

                    await dbAsync.UpdateAsync(securityVO);
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


        public async static Task<bool> DeleteSecurity()
        {
            var dbAsync = new BaseOperations();
            try
            {
                await dbAsync.DeleteAllAsync<SecurityVO>();

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