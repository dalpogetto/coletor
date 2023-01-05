using System;
using System.Threading.Tasks;
using Matcha.BackgroundService;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Resources.DataBaseHelper.Batch;
using CollectorQi.VO.Batch;
using Plugin.LocalNotifications;
using Plugin.Connectivity;
using CollectorQi.Resources;
using CollectorQi.Models;
using CollectorQi.Resources.DataBaseHelper.Batch.ESCL018;
using CollectorQi.Services.ESCL018;

namespace CollectorQi.Services
{
    public class IntegracaoServiceMovto : IPeriodicTask
    {
        public TimeSpan Interval { get; set; }

        public IntegracaoServiceMovto(int v = 1)
        {
            Interval = TimeSpan.FromSeconds(v);
        }

        async Task<bool> IPeriodicTask.StartJob()
        {
            //App.CallNotification.callNotification(eTpNotificacao.Transferencia, "Teste", true);


            if (CrossConnectivity.Current.IsConnected)
            {
                /*
                App.CallNotification.callNotification(eTpNotificacao.Transferencia, "transferencia");
                App.CallNotification.callNotification(eTpNotificacao.Inventario, "inventario"); */

               // if (SecurityAuxiliar.Autenticado)
               // {

                    /* Transferencia */
                    #region Transferencia 

                //    try
                //    {
                //        /* Status Integracao pendente (Transferencia) */
                //        var lstBatchDepositoTransferePend = BatchDepositoTransfereDB.GetBatchDepositoTransfereByStatus(eStatusIntegracao.PendenteIntegracao);
                //
                //        if (lstBatchDepositoTransferePend.Count > 0)
                //        {
                //            var tplRetorno = Models.Datasul.IntegracaoOnlineBatch.DepositoTransfereOffiline(lstBatchDepositoTransferePend);
                //
                //            App.CallNotification.callNotification(eTpNotificacao.Transferencia, tplRetorno.Item2.ToString());
                //
                //        }
                //
                //        var lstBatchDepositoTransfereErro = BatchDepositoTransfereDB.GetBatchDepositoTransfereByStatus(eStatusIntegracao.ErroIntegracao);
                //
                //        // Integração com erro nao envia notificaçãoo
                //        if (lstBatchDepositoTransfereErro.Count > 0)
                //        {
                //            var tplRetorno = Models.Datasul.IntegracaoOnlineBatch.DepositoTransfereOffiline(lstBatchDepositoTransfereErro);
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        App.CallNotification.callNotification(eTpNotificacao.Transferencia, ex.Message, true);
                //    }

                    #endregion

                    #region Inventário

                    try
                    {
                        /* Status Integracao pendente  */
                       var lstBatchInventarioPend = BatchInventarioItemDB.GetBatchInventarioByStatus(eStatusIntegracao.PendenteIntegracao);
                   
                       if (lstBatchInventarioPend.Count > 0)
                       {
                        for (int i = 0; i < lstBatchInventarioPend.Count; i++)
                        {
                            if (!String.IsNullOrEmpty(SecurityAuxiliar.CodUsuario) && !String.IsNullOrEmpty(SecurityAuxiliar.CodSenha))
                            {
                                var tplRetorno = await ParametersLeituraEtiquetaService.SendInventarioBatchAsync(lstBatchInventarioPend[i], true);

                                App.CallNotification.callNotification(eTpNotificacao.Inventario, tplRetorno);
                            }
                        }
                       }
                    }
                    catch (Exception ex)
                    {
                        App.CallNotification.callNotification(eTpNotificacao.Inventario, ex.Message, true);
                    }

                    #endregion

                }
           // }
            return true;


        }
    }

    public class IntegracaoServiceCad : IPeriodicTask
    {
        public TimeSpan Interval { get; set; }

        public IntegracaoServiceCad(int v = 1)
        {
            Interval = TimeSpan.FromMinutes(v);
        }

        async Task<bool> IPeriodicTask.StartJob()
        {
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    if (SecurityAuxiliar.Autenticado)
                    {
                        /* Integracao de cadastro */
                        var result = await Models.ConnectService.ConectColetorAsync(SecurityAuxiliar.CodUsuario, SecurityAuxiliar.CodSenha);

                        if (result == "OK")
                        {
                            await SecurityDB.AtualizarSecurityIntegracao();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
                //App.CallNotification.callNotification(eTpNotificacao.Inventario, "Erro ao atualizar o estoque " + ex.Message, true);                
            }
            return true;
        }
    }

}