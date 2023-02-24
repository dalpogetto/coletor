using AutoMapper;
using CollectorQi.Models.ESCL017;
using CollectorQi.Models.ESCL029;
using CollectorQi.Resources;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MovimentoReparosOpcoesTransferenciaPage : ContentPage
    {
        public List<OpcoesTransferenciaMovimentoReparo> ListaOpcoesTransferenciaMovimentoReparo;
        public ObservableCollection<OpcoesTransferenciaMovimentoReparoViewModel> ObsOpcoesTransferenciaMovimentoReparo { get; set; }
        public ParametrosInventarioReparo Parametros { get; set; }

        public MovimentoReparosOpcoesTransferenciaPage(List<OpcoesTransferenciaMovimentoReparo> listaOpcoesTransferenciaMovimentoReparo, ParametrosInventarioReparo parametros)
        {
            InitializeComponent();

            lblCodEstabel.Text = "Estabelecimento: " + SecurityAuxiliar.Estabelecimento;
            ListaOpcoesTransferenciaMovimentoReparo = listaOpcoesTransferenciaMovimentoReparo;
            ObsOpcoesTransferenciaMovimentoReparo = new ObservableCollection<OpcoesTransferenciaMovimentoReparoViewModel>();
            Parametros = parametros;

            if (listaOpcoesTransferenciaMovimentoReparo != null)
            {
                foreach (var item in listaOpcoesTransferenciaMovimentoReparo)
                {
                    var modelView = Mapper.Map<OpcoesTransferenciaMovimentoReparo, OpcoesTransferenciaMovimentoReparoViewModel>(item);
                    ObsOpcoesTransferenciaMovimentoReparo.Add(modelView);
                }
            }

            cvOpcoesTransferenciaMovimentoReparo.BindingContext = this;
        }

        private void cvOpcoesTransferencia_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var current = (cvOpcoesTransferenciaMovimentoReparo.SelectedItem as OpcoesTransferenciaMovimentoReparo);

            int iOpcao = int.Parse(current.Opcao);

            Application.Current.MainPage = new NavigationPage(new MovimentoReparosLeituraEtiqueta(ListaOpcoesTransferenciaMovimentoReparo, Parametros, current.DescOpcao, iOpcao));
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new ArmazenagemMovimentoReparoPage(null));

            return true;
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                ToolBarPrint.IsEnabled = false;

                var pageProgress = new ProgressBarPopUp("Carregando...");
                var page = new ArmazenagemPrintPopUp(null, null);
                await PopupNavigation.Instance.PushAsync(page);
                await pageProgress.OnClose();
            }
            finally
            {
                ToolBarPrint.IsEnabled = true;
            }
        }

        public class OpcoesTransferenciaMovimentoReparoViewModel : OpcoesTransferenciaMovimentoReparo
        {

        }
        private void ToolBarVoltar_Clicked(object sender, EventArgs e)
        {
            base.OnBackButtonPressed();
            Xamarin.Forms.Application.Current.MainPage = new NavigationPage(new ArmazenagemPage());
        }
    }
}