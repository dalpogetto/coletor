using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;

using CollectorQi.Resources.DataBaseHelper.Batch;
using System.Collections.ObjectModel;
using CollectorQi.Models.Datasul;

namespace CollectorQi.Views
{
    public partial class IntegracaoMovtoInventarioPopUp : PopupPage
    {
        private BatchInventarioViewModel _batchInventarioViewModel;
        private ObservableCollection<BatchInventarioViewModel> _obsBatchInventarioModel;
        private Action _AtualizaStatusIntegracao;

        public IntegracaoMovtoInventarioPopUp(BatchInventarioViewModel pBatchInventarioViewModel, ObservableCollection<BatchInventarioViewModel> pObsBatchInventarioViewModel, Action pAtualizaStatusIntegracao)
        {
            InitializeComponent();

            _AtualizaStatusIntegracao = pAtualizaStatusIntegracao;

            if (pBatchInventarioViewModel != null)
            {
                // Situacao inventário
                edtSituaocaInventario.Text  = pBatchInventarioViewModel.StatusIntegracaoString;
                imgSituacaoInventario.Source = pBatchInventarioViewModel.Image;

                edtEstabelecimento.Text = pBatchInventarioViewModel.CodEstabel       + " (" + pBatchInventarioViewModel.__estabelec__.Nome.Trim() + ")" ;
                edtCodDepos.Text        = pBatchInventarioViewModel.CodDepos         + " (" + pBatchInventarioViewModel.__deposito__.Nome.Trim() + ")";
                edtDtInventario.Text    = pBatchInventarioViewModel.DtInventario.ToString("dd/MM/yyyy");   
                edtContagem.Text        = pBatchInventarioViewModel.Contagem.ToString();    
                edtErro.Text            = pBatchInventarioViewModel.MsgIntegracao;

                _batchInventarioViewModel = pBatchInventarioViewModel;
                _obsBatchInventarioModel = pObsBatchInventarioViewModel;
            }
        }

        async void OnClick_Cancelar(object sender, EventArgs e)
        {
            try
            {
               /* if (_batchInventarioViewModel.StatusIntegracao != eStatusIntegracao.EnviadoIntegracao)
                { */

                    var action = await DisplayAlert("Cancelar Integração", "Tem certeza que deseja cancelar o envio do inventário para o sistema?", "Sim", "Não");

                    if (action.ToString() == "True")
                    {
                        // Status atualizado (confirma se nao foi enviado por backend)
                        var bt = BatchInventarioDB.GetBatchInventario(_batchInventarioViewModel.InventarioId);


                    /*    if (bt.StatusIntegracao != eStatusIntegracao.EnviadoIntegracao)
                        {  */
                            IntegracaoOnlineBatch.CancelaInventarioMobile(_batchInventarioViewModel.InventarioId);

                            _obsBatchInventarioModel.Remove(_batchInventarioViewModel);

                            _AtualizaStatusIntegracao();

                            await PopupNavigation.Instance.PopAsync();
                  /*      }
                        else
                        {
                            await DisplayAlert("Erro!", "Integração enviada para o sistema, não pode ser cancelada.", "Cancelar");
                        } */
                    }
              /*  }
                else
                {
                    // Inventario efetivado e integrado com TOTVS
                    await DisplayAlert("Erro!", "Inventário integrado com o sistema, não pode ser alterado.", "Cancelar");

                } */
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
        }
    }
}
