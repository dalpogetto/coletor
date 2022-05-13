using CollectorQi.Models;
using CollectorQi.Resources;
using CollectorQi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CollectorQi.VO.Batch;
using CollectorQi.Resources.DataBaseHelper.Batch;
using AutoMapper;
using Plugin.Connectivity;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IntegracaoMovtoTransferenciaPage : ContentPage
    {
        public ObservableCollection<BatchDepositoTransfereViewModel> ObsBatchDepositoTransferencia { get; set; }
        //public ObservableCollection<BatchDepositoTransfereVO> ObsSaldoEstoq { get; }

        public IntegracaoMovtoTransferenciaPage()
        {
            InitializeComponent();

            var lstBatch = BatchDepositoTransfereDB.GetBatchDepositoTransfere();

            ObsBatchDepositoTransferencia = new ObservableCollection<BatchDepositoTransfereViewModel>();

            for (int i = 0; i < lstBatch.Count; i++)
            {
                var modelView = Mapper.Map<BatchDepositoTransfereVO, BatchDepositoTransfereViewModel>(lstBatch[i]);

                ObsBatchDepositoTransferencia.Add(modelView);
            }



            /*ObsBatchDepositoTransferencia = new ObservableCollection<BatchDepositoTransfereVO>(lst);

            ObservableCollection<TransferenciaCollectionModel> v = new ObservableCollection<TransferenciaCollectionModel>(ObsBatchDepositoTransferencia);

            var modelView = Mapper.Map<BatchDepositoTransfereVO, BatchDepositoTransfereViewModel>(ObsBatchDepositoTransferencia);
            */
            cvIntegracaoTransferencia.BindingContext = this;
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            AtualizaStatusIntegracao();

        }

        public void AtualizaStatusIntegracao()
        {
            var tplCount = BatchDepositoTransfereDB.GetBatchDepositoTransfereCount();

            lblIntegracaoPendente.Text = tplCount.Item1.ToString();
            lblIntegracaoSucesso.Text = tplCount.Item2.ToString();
            lblIntegracaoErro.Text = tplCount.Item3.ToString();
        }

        async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var current = (cvIntegracaoTransferencia.SelectedItem as BatchDepositoTransfereViewModel);

            if (current != null)
            {
                var pageProgress = new IntegracaoMovtoTransferenciaPopUp(current, ObsBatchDepositoTransferencia, AtualizaStatusIntegracao);

                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);
            }

            cvIntegracaoTransferencia.SelectedItem = null;
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new IntegracaoMovtoPage());

            return true;
        }

        async void OnClick_AtualizaTransferencia(object sender, System.EventArgs e)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Erro!", "Para atualizar as transferências no sistema o dispositivo deve estar conectado.", "CANCELAR");

                return;

            }

            BtnAtualizaTransferencia.IsEnabled = false;

            var pageProgress = new ProgressBarPopUp("Atualizando transferência, aguarde...");

            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                var lstBatchDepositoTransfere = BatchDepositoTransfereDB.GetBatchDepositoTransfereByStatus(eStatusIntegracao.PendenteIntegracao);

                if (lstBatchDepositoTransfere == null)
                    lstBatchDepositoTransfere = new List<BatchDepositoTransfereVO>();

                lstBatchDepositoTransfere.AddRange(BatchDepositoTransfereDB.GetBatchDepositoTransfereByStatus(eStatusIntegracao.ErroIntegracao));

                var tplRetorno = Models.Datasul.IntegracaoOnlineBatch.DepositoTransfereOffiline(lstBatchDepositoTransfere.OrderBy(p => p.DtTransferencia).ToList());

                var lstBatch = BatchDepositoTransfereDB.GetBatchDepositoTransfere();

                ObsBatchDepositoTransferencia.Clear();

                for (int i = 0; i < lstBatch.Count; i++)
                {
                    var modelView = Mapper.Map<BatchDepositoTransfereVO, BatchDepositoTransfereViewModel>(lstBatch[i]);

                    ObsBatchDepositoTransferencia.Add(modelView);
                }

                AtualizaStatusIntegracao();

            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
            finally
            {
                BtnAtualizaTransferencia.IsEnabled = true;
                await pageProgress.OnClose();
            }
        }
    }

    public class BatchDepositoTransfereViewModel : BatchDepositoTransfereVO
    {
        public string Image
        {
            get
            {
                if (this.StatusIntegracao == eStatusIntegracao.PendenteIntegracao)
                {
                    return "intPendenteMed.png";
                }
                else if (this.StatusIntegracao == eStatusIntegracao.EnviadoIntegracao)
                {
                    return "intSucessoMed.png";

                }
                else if (this.StatusIntegracao == eStatusIntegracao.ErroIntegracao)
                {
                    return "intErroMed.png";
                }
                return "";
            }
        }

        public string StatusIntegracaoString
        {
            get
            {
                return csAuxiliar.GetDescription(StatusIntegracao);
            }
        }
    }
}