using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
//using System.ServiceModel;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Resources.DataBaseHelper.Batch;
using CollectorQi.Models.Datasul;
using CollectorQi.Views;
using CollectorQi.Resources;

using System.Threading;
using System.Globalization;
using Xamarin.Forms;

namespace CollectorQi.Services.ESCL000
{

    public static class ConnectService
    {
        private static bool IsExecConnectColetor; 

        public async static Task<string> ConnectColetorAsync(string pStrUsuario, string pStrSenha, ProgressBarPopUp pProgressBarPopUp = null)
        {
            /* Verifica se está sendo feita execução em BackEnd */
            if (pProgressBarPopUp == null)
            {
                if (IsExecConnectColetor)
                    return string.Empty; 
            }

            try
            {
                if (IsExecConnectColetor)
                    return string.Empty;

                IsExecConnectColetor = true;

                var result = String.Empty;
              
                result = await ConnectColetorSync(pStrUsuario, pStrSenha, pProgressBarPopUp);
      
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (pProgressBarPopUp != null)
                        pProgressBarPopUp.OnAcompanhar("Autenticando...");
                });


                return result.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                IsExecConnectColetor = false;
            }   
        }

        public async static Task<string> ConnectColetorSync(string pStrUsuario, string pStrSenha, ProgressBarPopUp pProgressBarPopUp)
        {
            try
            {
                
                string strLogin = String.Empty;

                var lstFilial = await CollectorQi.Services.ESCL000.CadastrosFiliais.ObterListaFiliais(pStrUsuario, pStrSenha);

                Device.BeginInvokeOnMainThread(() =>
                {

                    if (pProgressBarPopUp != null)
                        pProgressBarPopUp.OnAcompanhar("Salvando Cadastros Banco de Dados...");
                });

                if (lstFilial != null && lstFilial.Count > 0)
                {

                    CriaEstabelecimento(lstFilial);
                }
                else
                {
                    // Elimina todos estab e retorna OK
                    CriaEstabelecimento(lstFilial);

                   // throw new Exception("Usuário sem acesso a nenhuma filial!");
                }

                return "OK";


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async static Task CarregarListaFilial()
        {
            try
            {
                var lstFilial = await CollectorQi.Services.ESCL000.CadastrosFiliais.ObterListaFiliais(SecurityAuxiliar.CodUsuario, SecurityAuxiliar.CodSenha);

                /*
                Device.BeginInvokeOnMainThread(() =>
                {

                    if (pProgressBarPopUp != null)
                        pProgressBarPopUp.OnAcompanhar("Salvando Cadastros Banco de Dados...");
                });*/

                if (lstFilial != null && lstFilial.Count > 0)
                {

                    CriaEstabelecimento(lstFilial);
                }
                else
                {
                    // Elimina todos estab e retorna OK
                    CriaEstabelecimento(lstFilial);

                    //throw new Exception("Usuário sem acesso a nenhuma filial!");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async static void CriaEstabelecimento(List<Models.ESCL000.Filial> pLstFilial)
        {
            #region Estabelecimento

            var dbAsync = new BaseOperations();

            var lstDbEstabelec = await dbAsync.Connection.Table<VO.EstabelecVO>().ToListAsync();

            await dbAsync.DeleteAllAsync<VO.EstabelecVO>();

            if (pLstFilial != null)
            {

                for (int i = 0; i < pLstFilial.Count; i++)
                {
                    await dbAsync.InsertAsync(new VO.EstabelecVO
                    {
                        CodEstabel = pLstFilial[i].CodEstabel,
                        Nome = pLstFilial[i].Nome
                    });
                } 
            }

            #endregion
        }
        
        public static T DeserializeJsonWS<T>(string pStrJson)
        {
            var jsonTempTable = JsonConvert.DeserializeObject<List<TempTable>>(pStrJson);

            var jsonTempTableValues = JsonConvert.DeserializeObject<TempTableValues>(jsonTempTable[0].value);

            return jsonTempTableValues.records.ToObject<T>();
        }

    }
}
