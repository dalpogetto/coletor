using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using CollectorQi.Resources.DataBaseHelper.Batch;
using CollectorQi.Models.Datasul;
using System.Collections.ObjectModel;
using AutoMapper;

namespace CollectorQi.Views
{
    public partial class IntegracaoMovtoTransferenciaPopUp : PopupPage
    {
        private BatchDepositoTransfereViewModel                       _batchDepositoTransfereViewModel;
        private ObservableCollection<BatchDepositoTransfereViewModel> _obsBatchDepositoTransfereViewModel;
        private Action _AtualizaStatusIntegracao;

        public IntegracaoMovtoTransferenciaPopUp(BatchDepositoTransfereViewModel pBatchDepositoTransfereViewModel, ObservableCollection<BatchDepositoTransfereViewModel> pObsBatchDepositoTransfereViewModel, Action pAtualizaStatusIntegracao)
        {
            InitializeComponent();

            _AtualizaStatusIntegracao = pAtualizaStatusIntegracao;

            if (pBatchDepositoTransfereViewModel != null)
            {

                // Situacao inventário
                edtSituaocaTransferencia.Text = pBatchDepositoTransfereViewModel.StatusIntegracaoString;
                imgSituacaoTransferencia.Source = pBatchDepositoTransfereViewModel.Image;

                edtEstabelecimento.Text = pBatchDepositoTransfereViewModel.CodEstabel      + " (" + pBatchDepositoTransfereViewModel.__estabelec__.Nome.Trim() + ")" ;
                edtItCodigo.Text        = pBatchDepositoTransfereViewModel.ItCodigo;
                edtNroDocto.Text        = pBatchDepositoTransfereViewModel.NroDocto;
                edtDeposSaida.Text      = pBatchDepositoTransfereViewModel.CodDeposSaida   + " (" + pBatchDepositoTransfereViewModel.__depositoSaida__.Nome.Trim() + ")"; 
                edtDeposEntrada.Text    = pBatchDepositoTransfereViewModel.CodDeposEntrada + " (" + pBatchDepositoTransfereViewModel.__depositoEntrada__.Nome.Trim() + ")";
                edtLote.Text            = pBatchDepositoTransfereViewModel.CodLote;
                edtDtValiLote.Text      = pBatchDepositoTransfereViewModel.DtValiLote.HasValue ? pBatchDepositoTransfereViewModel.DtValiLote.Value.ToString("dd/MM/yyyy") : String.Empty;
                edtQuantidade.Text      = pBatchDepositoTransfereViewModel.QtidadeTransf.ToString();
                edtErro.Text            = pBatchDepositoTransfereViewModel.MsgIntegracao;

                edtDescItem.Text        = pBatchDepositoTransfereViewModel.__item__.DescItem;
                edtUnidade.Text         = pBatchDepositoTransfereViewModel.__item__.Un;
                edtTipoConEst.Text      = pBatchDepositoTransfereViewModel.__item__.__TipoConEst__;

                _batchDepositoTransfereViewModel = pBatchDepositoTransfereViewModel;
                _obsBatchDepositoTransfereViewModel = pObsBatchDepositoTransfereViewModel;
            }
        }

        async void OnClick_Cancelar(object sender, EventArgs e)
        {
            try
            {
                var action = await DisplayAlert("Cancelar Integração", "Tem certeza que deseja cancelar o envio de transferencia para o sistema?", "Sim", "Não");

                if (action.ToString() == "True")
                {
                    var bt = BatchDepositoTransfereDB.GetBatchDepositoTransfere(_batchDepositoTransfereViewModel.IntTransferenciaId);

                    if (bt.StatusIntegracao == eStatusIntegracao.EnviadoIntegracao)
                    {
                        await DisplayAlert("Erro!", "Integração enviada para o sistema, não pode ser cancelada.", "Cancelar");
                    }
                    else
                    {
                        IntegracaoOnlineBatch.CancelaTransferenciaMobile(_batchDepositoTransfereViewModel);

                        _obsBatchDepositoTransfereViewModel.Remove(_batchDepositoTransfereViewModel);

                        _AtualizaStatusIntegracao();

                        await PopupNavigation.Instance.PopAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
        }
    }
}
