using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CollectorQi.VO;
using SQLite;

namespace CollectorQi.Resources.DataBaseHelper
{
    public static class LeituraEtiquetaDB
    {

        public static async Task <bool> PrimeiraLeituraItemSerie(string byItCodigo, string bySerie)
        {
            LeituraEtiquetaVO leitura = null;
            var dbAsync = new BaseOperations();
            try
            {
                //Verificar se existe Item x Serie na tabela
                leitura = await dbAsync.Connection.Table<LeituraEtiquetaVO>().Where(p => p.ItCodigo == byItCodigo && p.Serie == bySerie).FirstOrDefaultAsync();

                //Se nao existir devolver True para prosseguir com o processo
                return (leitura == null);
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

        public static async Task GravarLeituraItemSerie(string byItCodigo, string bySerie)
        {
            LeituraEtiquetaVO leitura = null;
            var dbAsync = new BaseOperations();
            try
            {
                    leitura = new LeituraEtiquetaVO { ItCodigo = byItCodigo, Serie = bySerie };
                    var _idLeitura = await dbAsync.InsertAsync<LeituraEtiquetaVO>(leitura);
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
