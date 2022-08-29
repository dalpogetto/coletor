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
using AutoMapper;
using Plugin.Connectivity;
using CollectorQi.Resources.DataBaseHelper.Batch.ESCL018;
using CollectorQi.VO.Batch.ESCL018;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IntegracaoMovtoInventarioPage : ContentPage
    {
        public ObservableCollection<BatchInventarioViewModel> ObsBatchInventario { get; }
        //public ObservableCollection<BatchDepositoTransfereVO> ObsSaldoEstoq { get; }

        public IntegracaoMovtoInventarioPage()
        {
            InitializeComponent();

            var lstBatch = BatchInventarioItemDB.GetBatchInventario();

            ObsBatchInventario = new ObservableCollection<BatchInventarioViewModel>();

            for (int i = 0; i < lstBatch.Count; i++)
            {
                var modelView = Mapper.Map<BatchInventarioItemVO, BatchInventarioViewModel>(lstBatch[i]);

                ObsBatchInventario.Add(modelView);
            }

            cvIntegracaoInventario.BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            AtualizaStatusIntegracao();

        }

        public void AtualizaStatusIntegracao()
        {
            var tplCount = BatchInventarioItemDB.GetBatchInventarioCount();

            lblIntegracaoPendente.Text = tplCount.Item1.ToString();
            lblIntegracaoSucesso.Text = tplCount.Item2.ToString();
            lblIntegracaoErro.Text = tplCount.Item3.ToString();
        }

        async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var current = (cvIntegracaoInventario.SelectedItem as BatchInventarioViewModel);

            if (current != null)
            {
                var pageProgress = new IntegracaoMovtoInventarioPopUp(current, ObsBatchInventario, AtualizaStatusIntegracao);

                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);
            }

            cvIntegracaoInventario.SelectedItem = null;
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new IntegracaoMovtoPage());

            return true;
        }

        async void OnClick_AtualizaInventario(object sender, System.EventArgs e)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Erro!", "Para atualizar o inventário no sistema o dispositivo deve estar conectado.", "CANCELAR");

                return;

            }

            BtnAtualizaInventario.IsEnabled = false;

            var pageProgress = new ProgressBarPopUp("Atualizando inventário, aguarde...");

            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                var lstBatchInventario = BatchInventarioItemDB.GetBatchInventarioByStatus(eStatusIntegracao.PendenteIntegracao);

                if (lstBatchInventario == null)
                    lstBatchInventario = new List<BatchInventarioItemVO>();

                lstBatchInventario.AddRange(BatchInventarioItemDB.GetBatchInventarioByStatus(eStatusIntegracao.ErroIntegracao));

             //  for (int i = 0; i < lstBatchInventario.Count; i++)
             //  {
             //
             //      var tplRetorno = Models.Datasul.IntegracaoOnlineBatch.EfetivaInventarioSistemaOnline(lstBatchInventario[i]);
             //
             //  }
             //
             //  var lstBatch = BatchInventarioDB.GetBatchInventario();
             //
             //  ObsBatchInventario.Clear();
             //
             //  for (int i = 0; i < lstBatch.Count; i++)
             //  {
             //      var modelView = Mapper.Map<BatchInventarioVO, BatchInventarioViewModel>(lstBatch[i]);
             //
             //      ObsBatchInventario.Add(modelView);
             //  }
             //
             //  AtualizaStatusIntegracao();

            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
            finally
            {
                BtnAtualizaInventario.IsEnabled = true;
                await pageProgress.OnClose();
            }
        }

    }

    public class BatchInventarioViewModel : BatchInventarioItemVO
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